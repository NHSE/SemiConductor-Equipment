using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class BufferService
    {
        private readonly DbLogHelper _logHelper;
        private readonly RobotArmService _robotArmService;
        private readonly object _lock = new();
        private readonly Dictionary<string, (Wafer? wafer, bool isProcessing)> _bufferSlots = new()
        {
            ["Buffer1"] = (null, false),
            ["Buffer2"] = (null, false),
            ["Buffer3"] = (null, false),
            ["Buffer4"] = (null, false),
        };

        public BufferService(DbLogHelper logHelper, RobotArmService robotArmService)
        {
            this._logHelper = logHelper;
            this._robotArmService = robotArmService;
        }

        public string? FindEmptySlot()
        {
            lock (_lock)
            {
                return _bufferSlots.FirstOrDefault(x => x.Value.wafer == null && x.Value.isProcessing == false).Key;
            }
        }

        public (string BufferName, Wafer Wafer)? PeekCompletedWafer()
        {
            lock (_lock)
            {
                var completed = _bufferSlots.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing != false);
                if (completed.Value.wafer == null) return null;
                return (completed.Key, completed.Value.wafer);
            }
        }

        public async Task StartProcessingAsync(string buffername, Wafer wafer)
        {
            lock (_lock)
            {
                // 웨이퍼 넣기 + 처리중 상태 표시
                _bufferSlots[buffername] = (wafer, false);
                this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "IN");
            }

            // 프로세스 시뮬레이션 (예: 3초)
            await Task.Delay(10000);

            lock (_lock)
            {
                // 처리 완료 상태로 변경 (processing = false)
                _bufferSlots[buffername] = (wafer, true);
                //wafer.TargetLocation = "LoadPort";
                //this._robotArmService.EnqueueWafer(wafer);
            }

            Console.WriteLine($"[Buffer] {wafer.SlotId} process done in {buffername}");
            this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "DONE");
        }

        public (string Buffername, Wafer Wafer)? FindCompletedWafer()
        {
            lock (_lock)
            {
                var completed = _bufferSlots.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing == false);
                if (completed.Value.wafer == null) return null;

                _bufferSlots[completed.Key] = (null, false);
                return (completed.Key, completed.Value.wafer);
            }
        }

        public void RemoveWaferFromBuffer(string buffername)
        {
            lock (_lock)
            {
                if (_bufferSlots.ContainsKey(buffername))
                {
                    this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "OUT");
                    _bufferSlots[buffername] = (null, false);
                }
            }
        }

        public bool IsAllBufferEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return _bufferSlots.Values.All(buffer => buffer.wafer == null);
        }
    }
}
