using System.Threading;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Messages;
using CommunityToolkit.Mvvm.Messaging;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;

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

        public async Task ProcessCommandQueueAsync()
        {
            if (_isProcessing)
                return;

            _isProcessing = true;

            while (true)
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
                    break;

                if(command.Location == $"LoadPort{command.Wafer.LoadportId}")
                {
                    CommandCompleted?.Invoke(this, command.Wafer);
                }

                Console.WriteLine($"[RobotArm] {command.CommandType} {command.Wafer.Wafer_Num} → {command.Location}");

                await Task.Delay(300); // 모션 처리 시간 시뮬레이션

                // 이동 완료 후 현재 위치 갱신
                if (command.CommandType == RobotCommandType.Place)
                {
                    command.Wafer.CurrentLocation = command.Location;
                    WaferMoveInfo?.Invoke(this, command.Wafer);
                }
            }
            _isProcessing = false;
        }

        public bool IsBusy() => _isProcessing;
    }
}