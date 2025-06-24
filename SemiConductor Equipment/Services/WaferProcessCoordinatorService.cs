using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SemiConductor_Equipment.Commands;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                this._runningStateService.Change_State(EquipmentStatusEnum.Wait);
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

                            if (this._runningStateService.Get_State() != EquipmentStatusEnum.Running)
                            {
                                this._runningStateService.Change_State(EquipmentStatusEnum.Running);
                            }

                            _robotArmService.EnqueueCommand_RobotArm(new RobotCommand
                            {
                                CommandType = RobotCommandType.Place,
                                Wafer = wafer,
                                Location = emptyChamber
                            });

                            await _robotArmService.ProcessCommandQueueAsync();
                            wafer.TargetLocation = emptyChamber;
                            _chamberService.StartProcessingAsync(emptyChamber, wafer);
                        }
                    }

                    // 2. 챔버 완료 → 버퍼
                    if (_robotArmService.CommandSize_Chamber() > 0)
                    {
                        while (true)
                        {
                            if (_robotArmService.CommandSize_Chamber() == 0) break;

                            string? emptyBuffer = _bufferService.FindEmptySlot();
                            if (emptyBuffer != null)
                            {
                                var Command = _robotArmService.DequeueCommand_Chamber();
                                var completedInChamber = Command.Location;

                                Command.Wafer.TargetLocation = emptyBuffer;
                                _robotArmService.EnqueueCommand_RobotArm(new RobotCommand
                                {
                                    CommandType = RobotCommandType.Place,
                                    Wafer = Command.Wafer,
                                    Location = Command.Wafer.TargetLocation,
                                });

                                _chamberService.RemoveWaferFromChamber(completedInChamber);
                                await _robotArmService.ProcessCommandQueueAsync();
                                _bufferService.StartProcessingAsync(emptyBuffer, Command.Wafer);

                                await Task.Delay(300);
                            }
                            else break;
                        }
                    }

                    //버퍼 -> 로드포트
                    if (_robotArmService.CommandSize_Buffer() > 0)
                    {
                        while (true)
                        {
                            if (_robotArmService.CommandSize_Buffer() == 0) break;

                            var Command = _robotArmService.DequeueCommand_Buffer();
                            var completedInBuffer = Command.Location;

                            Command.Wafer.TargetLocation = $"LoadPort{Command.Wafer.LoadportId}";
                            _robotArmService.EnqueueCommand_RobotArm(new RobotCommand
                            {
                                CommandType = RobotCommandType.Place,
                                Wafer = Command.Wafer,
                                Location = Command.Wafer.TargetLocation,
                            });

                            await _robotArmService.ProcessCommandQueueAsync();
                            _bufferService.RemoveWaferFromBuffer(completedInBuffer);

                            await Task.Delay(300);
                        }
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
