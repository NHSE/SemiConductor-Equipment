using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SemiConductor_Equipment.Services
{
    public class WaferProcessCoordinatorService
    {
        private readonly ChamberService _chamberService;
        private readonly BufferService _bufferService;
        private readonly RobotArmService _robotArmService;
        private readonly RunningStateService _runningStateService;

        public WaferProcessCoordinatorService(ChamberService chamberService, BufferService bufferService, RobotArmService robotArmService, RunningStateService runningStateService)
        {
            _chamberService = chamberService;
            _bufferService = bufferService;
            _robotArmService = robotArmService;
            _runningStateService = runningStateService;
        }

        public async Task StartProcessAsync(Queue<Wafer> waferQueue, CancellationToken token)
        {
            bool isError = false;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    bool isAllDone = waferQueue.Count == 0
                     && _chamberService.IsAllChamberEmpty()
                     && _bufferService.IsAllBufferEmpty();

                    if (isAllDone)
                        break;

                    // 1. 챔버에 빈 자리가 있는지 확인
                    string? emptyChamber = _chamberService.FindEmptyChamber();

                    if (emptyChamber != null && waferQueue.Count > 0)
                    {
                        var wafer = waferQueue.Peek(); // ❗일단 꺼내지 말고 Peek

                        if (wafer.Processing) { isError = true; break; }

                        if (wafer.CurrentLocation == $"LoadPort{wafer.LoadportId}")
                        {
                            waferQueue.Dequeue(); // 이제 꺼내도 됨

                            wafer.TargetLocation = "RobotArm";
                            _robotArmService.EnqueueWafer(wafer);

                            wafer.TargetLocation = emptyChamber;
                            _robotArmService.EnqueueWafer(wafer);

                            _chamberService.StartProcessingAsync(emptyChamber, wafer);
                            if (this._runningStateService.Get_State() != EquipmentStatusEnum.Running)
                            {
                                this._runningStateService.Change_State(EquipmentStatusEnum.Running);
                            }
                        }
                    }

                    // 2. 챔버 완료 → 버퍼
                    var completedInChamber = _chamberService.PeekCompletedWafer();
                    if (completedInChamber != null)
                    {
                        string? emptyBuffer = _bufferService.FindEmptySlot();
                        if (emptyBuffer != null)
                        {
                            var wafer = completedInChamber.Value.Wafer;

                            wafer.TargetLocation = "RobotArm";
                            _robotArmService.EnqueueWafer(wafer);

                            wafer.TargetLocation = emptyBuffer;
                            _robotArmService.EnqueueWafer(wafer);

                            _chamberService.RemoveWaferFromChamber(completedInChamber.Value.ChamberName);

                            _bufferService.StartProcessingAsync(emptyBuffer, wafer);
                        }
                    }

                    // 3. 버퍼 완료 → 로드포트
                    var completedInBuffer = _bufferService.PeekCompletedWafer();
                    if (completedInBuffer != null)
                    {
                        var wafer = completedInBuffer.Value.Wafer;

                        wafer.TargetLocation = "RobotArm";
                        _robotArmService.EnqueueWafer(wafer);

                        wafer.TargetLocation = $"LoadPort{wafer.LoadportId}";
                        _robotArmService.EnqueueWafer(wafer);

                        _bufferService.RemoveWaferFromBuffer(completedInBuffer.Value.BufferName);
                        wafer.Processing = true;
                    }

                    await Task.Delay(300);
                }
            }
            finally
            {
                waferQueue.Clear();
                if (!isError)
                    this._runningStateService.Change_State(EquipmentStatusEnum.Completed);
                else
                    this._runningStateService.Change_State(EquipmentStatusEnum.Error);
            }
        }
    }
}
