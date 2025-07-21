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

namespace SemiConductor_Equipment.Services
{
    public class RobotArmService : IRobotArmManager
    {
        private readonly Queue<RobotCommand> _EndChambercommandQueue = new();
        private readonly Queue<RobotCommand> _EndBuffercommandQueue = new();
        private readonly Queue<RobotCommand> _RobotArmcommandQueue = new();

        public event EventHandler<Wafer> CommandStarted;
        public event EventHandler<Wafer> CommandCompleted;
        public event EventHandler<Wafer> WaferMoveInfo;

        private bool _isProcessing = false;
        private CancellationTokenSource? _cts;
        private Task? _processingTask;

        private readonly IChamberManager _chamberManager;
        private readonly IBufferManager _bufferManager;

        public RobotArmService(IChamberManager chamberManager, IBufferManager bufferManager)
        {
            _chamberManager = chamberManager;
            _bufferManager = bufferManager;

            _chamberManager.Enque_Robot += OnQueDataInput;
            _bufferManager.Enque_Robot += OnQueDataInput;
        }

        public void EnqueueCommand_Chamber(RobotCommand command)
        {
            _EndChambercommandQueue.Enqueue(command);
        }

        public void EnqueueCommand_Buffer(RobotCommand command)
        {
            _EndBuffercommandQueue.Enqueue(command);
        }

        public void EnqueueCommand_RobotArm(RobotCommand command)
        {
            _RobotArmcommandQueue.Enqueue(command);
        }

        public RobotCommand DequeueCommand_Chamber()
        {
            return _EndChambercommandQueue.Dequeue();
        }

        public RobotCommand DequeueCommand_Buffer()
        {
            return _EndBuffercommandQueue.Dequeue();
        }

        public int CommandSize_Chamber()
        {
            return _EndChambercommandQueue.Count;
        }

        public int CommandSize_Buffer()
        {
            return _EndBuffercommandQueue.Count;
        }

        private void OnQueDataInput(object? sender, RobotCommand e)
        {
            switch(e.Location)
            {
                case "Buffer":
                    _EndChambercommandQueue.Enqueue(e);
                    break;
                case "LoadPort":
                    _EndBuffercommandQueue.Enqueue(e);
                    break;
            }
        }

        public async Task ProcessCommandQueueAsync(CancellationToken token)
        {
            if (_isProcessing)
                return;

            _isProcessing = true;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    RobotCommand? command = null;

                    if (_RobotArmcommandQueue.Count > 0)
                    {
                        command = _RobotArmcommandQueue.Dequeue();
                        CommandStarted?.Invoke(this, command.Wafer);

                        command.Wafer.CurrentLocation = "RobotArm";
                        WaferMoveInfo?.Invoke(this, command.Wafer);
                    }

                    if (command == null)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }

                    if (command.Location == $"LoadPort{command.Wafer.LoadportId}")
                    {
                        CommandCompleted?.Invoke(this, command.Wafer);
                    }

                    Console.WriteLine($"[RobotArm] {command.CommandType} {command.Wafer.Wafer_Num} → {command.Location}");

                    await Task.Delay(300); // 모션 처리 시간 시뮬레이션

                    switch (command.Location)
                    {
                        case string s when s.Contains("Chamber"):
                            command.Wafer.Status = "Running";
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _chamberManager.StartProcessingAsync(command.Wafer.TargetLocation, command.Wafer);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[Chamber 예외] " + ex);
                                }
                            });
                            break;

                        case string s when s.Contains("Buffer"):
                            _chamberManager.RemoveWaferFromChamber(command.Completed);
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _bufferManager.StartProcessingAsync(command.Wafer.TargetLocation, command.Wafer);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[Buffer 예외] " + ex);
                                }
                            });
                            break;

                        case string s when s.Contains("LoadPort"):
                            if (command.Completed.Contains("Chamber"))
                                _chamberManager.RemoveWaferFromChamber(command.Completed);
                            else
                                _bufferManager.RemoveWaferFromBuffer(command.Completed);
                            break;
                    }

                    // 이동 완료 후 현재 위치 갱신
                    if (command.CommandType == RobotCommandType.Place)
                    {
                        command.Wafer.CurrentLocation = command.Location;
                        WaferMoveInfo?.Invoke(this, command.Wafer);
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public bool IsBusy() => _isProcessing;

        public void StartProcessing(CancellationToken externalToken)
        {
            if (_isProcessing) return;

            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var token = _cts.Token;

            //_processingTask = Task.Run(() => ProcessCommandQueueAsync(token), token);
            Task.Run(async () =>
            {
                try
                {
                    // 예외 발생 가능 코
                    await ProcessCommandQueueAsync(token);
                }
                catch (Exception ex)
                {
                    // 예외 로깅
                    Console.WriteLine(ex.ToString());
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
            }
        }
    }
}