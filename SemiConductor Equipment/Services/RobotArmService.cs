using System.Threading;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Messages;
using CommunityToolkit.Mvvm.Messaging;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;

namespace SemiConductor_Equipment.Services
{
    public class RobotArmService
    {
        private readonly Queue<RobotCommand> _EndChambercommandQueue = new();
        private readonly Queue<RobotCommand> _EndBuffercommandQueue = new();
        private readonly Queue<RobotCommand> _RobotArmcommandQueue = new();
        public event EventHandler<Wafer> CommandStarted;
        public event EventHandler<Wafer> CommandCompleted;

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

        public RobotCommand CommandPeek_Chamber => _EndChambercommandQueue.Peek();

        public RobotCommand CommandPeek_Buffer => _EndBuffercommandQueue.Peek();

        public async Task WaitForBufferCommandAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 1. 커맨드 큐에 명령이 있는지 확인
                if (CommandSize_Buffer() > 0)
                {
                    // 2. 명령 있음 → 반환
                    return;
                }

                // 3. 아직 없음 → 일정 시간 대기
                await Task.Delay(100, token); // 100ms 폴링
            }

            // 4. 종료 요청됨
            throw new OperationCanceledException();
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
                }
            }
            _isProcessing = false;
        }

        public bool IsBusy => _isProcessing;
    }
}