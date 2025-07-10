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
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class WaferProcessCoordinatorService
    {
        private readonly IChamberManager _chamberManager;
        private readonly IBufferManager _bufferManager;
        private readonly IRobotArmManager _robotArmManager;
        private readonly RunningStateService _runningStateService;

        public WaferProcessCoordinatorService(IChamberManager chamberManager, IBufferManager bufferManager, IRobotArmManager robotArmManager, RunningStateService runningStateService)
        {
            _chamberManager = chamberManager;
            _bufferManager = bufferManager;
            _robotArmManager = robotArmManager;
            _runningStateService = runningStateService;
        }

        public async Task StartProcessAsync(Queue<Wafer> waferQueue, CancellationToken token)
        {
            bool isError = false;
            CancellationTokenSource cts = new CancellationTokenSource();
            _robotArmManager.StartProcessing(cts.Token);
            try
            {
                this._runningStateService.Change_State(EquipmentStatusEnum.Wait);
                while (!token.IsCancellationRequested)
                {
                    bool isAllDone = waferQueue.Count == 0
                     && _chamberManager.IsAllChamberEmpty()
                     && _bufferManager.IsAllBufferEmpty();

                    if (isAllDone)
                        break;

                    // 1. 챔버에 빈 자리가 있는지 확인
                    string? emptyChamber = _chamberManager.FindEmptyChamber();

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

                            wafer.TargetLocation = emptyChamber;
                            _robotArmManager.EnqueueCommand_RobotArm(new RobotCommand
                            {
                                CommandType = RobotCommandType.Place,
                                Wafer = wafer,
                                Location = emptyChamber
                            });

                            _chamberManager.AddWaferToChamber(emptyChamber, wafer);
                        }
                    }

                    // 2. 챔버 완료 → 버퍼
                    if (_robotArmManager.CommandSize_Chamber() > 0)
                    {
                        while (true)
                        {
                            if (_robotArmManager.CommandSize_Chamber() == 0) break;

                            string? emptyBuffer = _bufferManager.FindEmptySlot();
                            if (emptyBuffer != null)
                            {
                                var Command = _robotArmManager.DequeueCommand_Chamber();
                                var completedInChamber = Command.Completed;

                                Command.Wafer.TargetLocation = emptyBuffer;

                                _robotArmManager.EnqueueCommand_RobotArm(new RobotCommand
                                {
                                    CommandType = RobotCommandType.Place,
                                    Wafer = Command.Wafer,
                                    Location = Command.Wafer.TargetLocation,
                                    Completed = completedInChamber
                                });
                                _bufferManager.AddWaferToBuffer(emptyBuffer, Command.Wafer);

                                await Task.Delay(300);
                            }
                            else break;
                        }
                    }

                    //버퍼 -> 로드포트
                    if (_robotArmManager.CommandSize_Buffer() > 0)
                    {
                        while (true)
                        {
                            if (_robotArmManager.CommandSize_Buffer() == 0) break;

                            var Command = _robotArmManager.DequeueCommand_Buffer();

                            Command.Wafer.TargetLocation = $"LoadPort{Command.Wafer.LoadportId}";
                            _robotArmManager.EnqueueCommand_RobotArm(new RobotCommand
                            {
                                CommandType = RobotCommandType.Place,
                                Wafer = Command.Wafer,
                                Location = Command.Wafer.TargetLocation,
                                Completed = Command.Completed
                            });
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

                await _robotArmManager.StopProcessing();
            }
        }
    }
}
