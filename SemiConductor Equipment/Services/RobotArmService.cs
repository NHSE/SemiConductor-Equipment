using System.Threading;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Messages;
using CommunityToolkit.Mvvm.Messaging;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public class RobotArmService : IRobotArmManager
    {
        private readonly Queue<RobotCommand> _EndCleancommandQueue = new();
        private readonly Queue<RobotCommand> _EndDrycommandQueue = new();
        private readonly Queue<RobotCommand> _RobotArmcommandQueue = new();

        public event EventHandler<Wafer> CommandStarted;
        public event EventHandler<Wafer> CommandCompleted;
        public event EventHandler<Wafer> WaferMoveInfo;

        private bool _isProcessing = false;
        private CancellationTokenSource? _cts;
        private Task? _processingTask;
        private readonly object _lock = new();

        private readonly IChamberManager _chamberManager;
        private readonly ICleanManager _cleanManager;
        private readonly IVIDManager _vidManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly ILogManager _logManager;

        public RobotArmService(IChamberManager chamberManager, ICleanManager cleanManager, IVIDManager VIDManager, IEventMessageManager eventMessageManager, ILogManager logManager)
        {
            _chamberManager = chamberManager;
            _cleanManager = cleanManager;
            _vidManager = VIDManager;
            _eventMessageManager = eventMessageManager;
            _logManager = logManager;

            _chamberManager.Enque_Robot += OnQueDataInput;
            _cleanManager.Enque_Robot += OnQueDataInput;
        }

        public void EnqueueCommand_Chamber(RobotCommand command)
        {
            _EndCleancommandQueue.Enqueue(command);
        }

        public void EnqueueCommand_Buffer(RobotCommand command)
        {
            _EndDrycommandQueue.Enqueue(command);
        }

        public void EnqueueCommand_RobotArm(RobotCommand command)
        {
            _RobotArmcommandQueue.Enqueue(command);
        }

        public RobotCommand DequeueCommand_Chamber()
        {
            return _EndCleancommandQueue.Dequeue();
        }

        public RobotCommand DequeueCommand_Buffer()
        {
            return _EndDrycommandQueue.Dequeue();
        }

        public int CommandSize_Chamber()
        {
            return _EndCleancommandQueue.Count;
        }

        public int CommandSize_Buffer()
        {
            return _EndDrycommandQueue.Count;
        }

        private void OnQueDataInput(object? sender, RobotCommand e)
        {
            switch(e.NextLocation)
            {
                case string s when s.Contains("Chamber"):
                    _EndCleancommandQueue.Enqueue(e);
                    break;
                case string s when s.Contains("LoadPort"):
                    _EndDrycommandQueue.Enqueue(e);
                    break;
            }
        }

        public async Task ProcessCommandQueueAsync(CancellationToken token)
        {
            string prev_Loacation = string.Empty;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    RobotCommand? command = null;
                    lock (_lock)
                    {
                        if (_RobotArmcommandQueue.Count > 0)
                        {
                            command = _RobotArmcommandQueue.Dequeue();
                            if (command == null) continue;

                            prev_Loacation = command.Wafer.CurrentLocation;

                            command.Wafer.CurrentLocation = "RobotArm";

                            CommandStarted?.Invoke(this, command.Wafer);
                            WaferMoveInfo?.Invoke(this, command.Wafer);

                            _vidManager.SetDVID(1007, command.Wafer.CurrentLocation, command.Wafer.Wafer_Num);
                            CEIDInfo info = _eventMessageManager.GetCEID(200);
                            info.Wafer_number = command.Wafer.Wafer_Num;
                            info.Loadport_Number = command.Wafer.LoadportId;
                            _eventMessageManager.EnqueueEventData(info);

                            if(command.Wafer.Status == "Ready")
                            {
                                command.Wafer.Status = "Running";
                            }
                        }
                    }

                    if (command == null)
                    {
                        if (_vidManager.RobotStatus != "IDLE")
                            _vidManager.SetSVID(101, "IDLE");
                        await Task.Delay(100, token);
                        continue;
                    }

                    this._logManager.WriteLog("RobotArm", $"State", $"[RobotArm] {command.CommandType} {command.Wafer.Wafer_Num} → {command.Location}");
                    _vidManager.SetSVID(101, "Running");

                    await Task.Delay(500); // 모션 처리 시간 시뮬레이션

                    switch (command.NextLocation)
                    {
                        case string s when s.Contains("Clean Chamber"):
                            _vidManager.SetDVID(1007, command.Wafer.TargetLocation, command.Wafer.Wafer_Num);
                            CEIDInfo data = _eventMessageManager.GetCEID(201);
                            data.Wafer_number = command.Wafer.Wafer_Num;
                            data.Loadport_Number = command.Wafer.LoadportId;
                            _eventMessageManager.EnqueueEventData(data);

                            command.Wafer.CurrentLocation = command.NextLocation + "_" + command.Wafer.TargetLocation;

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _cleanManager.StartProcessingAsync(command.Wafer.TargetLocation, command.Wafer);
                                }
                                catch (Exception ex)
                                {
                                    this._logManager.WriteLog("Error", $"State", $"[Clean Chamber] {ex}");
                                }
                            });
                            break;

                        case string s when s.Contains("Dry Chamber"):
                            command.Wafer.Status = "Running";
                            _cleanManager.RemoveWaferFromCleanChamber(command.Completed);
                            _vidManager.SetDVID(1007, command.Wafer.TargetLocation, command.Wafer.Wafer_Num);
                            CEIDInfo info = _eventMessageManager.GetCEID(201);
                            info.Loadport_Number = command.Wafer.LoadportId;
                            info.Wafer_number = command.Wafer.Wafer_Num;
                            _eventMessageManager.EnqueueEventData(info);

                            command.Wafer.CurrentLocation = command.NextLocation + "_" + command.Wafer.TargetLocation;

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _chamberManager.StartProcessingAsync(command.Wafer.TargetLocation, command.Wafer);
                                }
                                catch (Exception ex)
                                {
                                    this._logManager.WriteLog("Error", $"State", $"[Dry Chamber] {ex}");
                                }
                            });
                            break;

                        case string s when s.Contains("LoadPort"):
                            if(prev_Loacation.Contains("Dry"))
                                _chamberManager.RemoveWaferFromChamber(command.Completed);
                            else
                                _cleanManager.RemoveWaferFromCleanChamber(command.Completed);
                             
                            _vidManager.SetDVID(1007, $"LoadPort{command.Wafer.LoadportId}", command.Wafer.Wafer_Num);
                            CEIDInfo ceid_data = _eventMessageManager.GetCEID(201);
                            ceid_data.Loadport_Number = command.Wafer.LoadportId;
                            ceid_data.Wafer_number = command.Wafer.Wafer_Num;
                            _eventMessageManager.EnqueueEventData(ceid_data);
                            command.Wafer.CurrentLocation = $"LoadPort{command.Wafer.LoadportId}";
                            CommandCompleted?.Invoke(this, command.Wafer);
                            break;
                    }

                    // 이동 완료 후 현재 위치 갱신
                    if (command.CommandType == RobotCommandType.Place)
                    {
                        _vidManager.SetSVID(101, "IDLE");
                        WaferMoveInfo?.Invoke(this, command.Wafer);
                    }
                    await Task.Delay(1000); // 모션 처리 시간 시뮬레이션
                }
            }
            finally
            {
            }
        }

        public bool IsBusy() => _isProcessing;

        public void StartProcessing(CancellationToken externalToken)
        {
            if (_isProcessing) return;

            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var token = _cts.Token;

            _isProcessing = true;

            Task.Run(async () =>
            {
                try
                {
                    // 예외 발생 가능 코
                    await ProcessCommandQueueAsync(token);
                }
                catch (Exception ex)
                {
                    this._logManager.WriteLog("Error", $"State", $"[RobotArm] {ex}");
                }
            });
        }

        public async Task StopProcessing()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                while (_RobotArmcommandQueue.Count() > 0)
                {
                    await Task.Delay(300); // 다른 Task에 영향 X
                }
                _cts.Cancel();
                _RobotArmcommandQueue.Clear();
                _isProcessing = false;
            }
        }
    }
}