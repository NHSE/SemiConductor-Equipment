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
    public class WaferProcessCoordinatorService : IWaferProcessCoordinator
    {
        #region FIELDS
        private readonly IChamberManager _chamberManager;
        private readonly ICleanManager _cleanManager;
        private readonly IRobotArmManager _robotArmManager;
        private readonly IMessageBox _messageBoxManager;
        private readonly RunningStateService _runningStateService;
        public event EventHandler<string> Process;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public WaferProcessCoordinatorService(IChamberManager chamberManager, ICleanManager cleanManager, IRobotArmManager robotArmManager, IMessageBox messageBoxManager, RunningStateService runningStateService)
        {
            this._chamberManager = chamberManager;
            this._cleanManager = cleanManager;
            this._robotArmManager = robotArmManager;
            this._messageBoxManager = messageBoxManager;
            this._runningStateService = runningStateService;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public async Task StartProcessAsync(Queue<Wafer> waferQueue, CancellationToken token)
        {
            bool isError = false;
            CancellationTokenSource cts = new CancellationTokenSource();
            string sender = "";
            _robotArmManager.StartProcessing(cts.Token);
            _chamberManager.ProcessStart();
            Process?.Invoke(this, "Start");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    bool isAllDone = waferQueue.Count == 0
                     && _chamberManager.IsAllChamberEmpty()
                     && _cleanManager.IsAllCleanChamberEmpty();

                    if (isAllDone)
                        break;

                    // 모든 챔버 내 Clean 용액 부족하다면 테스트 시작 안함
                    if(_cleanManager.IsAllDisableChamber() && waferQueue.Count > 0)
                    {
                        while(waferQueue.Count != 0)
                        {
                            var wafer = waferQueue.Dequeue();
                            wafer.Status = "Not Process";
                        }
                        this._messageBoxManager.Show("예외 발생", "Soultion이 부족합니다.\n설정 후 다시 진행해주세요.");
                        //메세지 박스
                        waferQueue.Clear();
                    }

                    // 1. 챔버에 빈 자리가 있는지 확인
                    string? emptyCleanChamber = _cleanManager.FindEmptySlot();

                    if (emptyCleanChamber != null && waferQueue.Count > 0)
                    {
                        var wafer = waferQueue.Peek(); // ❗일단 꺼내지 말고 Peek

                        if (wafer.Processing) { isError = true; break; }

                        if (wafer.CurrentLocation == $"LoadPort{wafer.LoadportId}")
                        {
                            waferQueue.Dequeue(); // 이제 꺼내도 됨

                            if (this._runningStateService.Get_State() != EquipmentStatusEnum.Running)
                            {
                                sender = wafer.CurrentLocation;
                                this._runningStateService.Change_State(sender, EquipmentStatusEnum.Running);
                            }

                            wafer.TargetLocation = emptyCleanChamber;
                            _robotArmManager.EnqueueCommand_RobotArm(new RobotCommand
                            {
                                CommandType = RobotCommandType.Place,
                                Wafer = wafer,
                                Location = emptyCleanChamber,
                                NextLocation = "Clean Chamber"
                            });

                            _cleanManager.AddWaferToBuffer(emptyCleanChamber, wafer);
                        }
                    }

                    // 2. 챔버 완료 → 버퍼
                    if (_robotArmManager.CommandSize_Chamber() > 0)
                    {
                        while (true)
                        {
                            if (_robotArmManager.CommandSize_Chamber() == 0) break;

                            string? emptyDryChamber = _chamberManager.FindEmptyChamber();
                            if (emptyDryChamber != null)
                            {
                                var Command = _robotArmManager.DequeueCommand_Chamber();
                                var completedInChamber = Command.Completed;

                                Command.Wafer.TargetLocation = emptyDryChamber;

                                _robotArmManager.EnqueueCommand_RobotArm(new RobotCommand
                                {
                                    CommandType = RobotCommandType.Place,
                                    Wafer = Command.Wafer,
                                    Location = Command.Wafer.TargetLocation,
                                    NextLocation = "Dry Chamber",
                                    Completed = completedInChamber
                                });
                                _chamberManager.AddWaferToChamber(emptyDryChamber, Command.Wafer);

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
                                NextLocation = "LoadPort",
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
                    this._runningStateService.Change_State(sender, EquipmentStatusEnum.Completed);
                else
                    this._runningStateService.Change_State(sender, EquipmentStatusEnum.Error);

                await _robotArmManager.StopProcessing();

                Process?.Invoke(this, "END");
            }
        }
        #endregion
    }
}
