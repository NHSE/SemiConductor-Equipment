using System.Threading;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace SemiConductor_Equipment.Services
{
    public class RobotArmService
    {
        private readonly Queue<Wafer> _moveQueue = new();

        private bool _isMoving = false;

        public void EnqueueWafer(Wafer wafer)
        {
            lock (_moveQueue)
            {
                _moveQueue.Enqueue(wafer);
            }
            ProcessQueueAsync();
        }

        private async Task ProcessQueueAsync()
        {
            if (_isMoving)
                return;

            _isMoving = true;
            while (true)
            {
                Wafer? wafer = null;
                lock (_moveQueue)
                {
                    if (_moveQueue.Count > 0)
                        wafer = _moveQueue.Dequeue();
                }

                if (wafer == null)
                    break;

                // 실제 이동 처리
                Console.WriteLine($"RobotArm 이동: {wafer.Wafer_Num} → {wafer.TargetLocation}");
                await Task.Delay(100); // 모션 시뮬레이션

                wafer.CurrentLocation = wafer.TargetLocation;
            }
            _isMoving = false;
        }
    }
}