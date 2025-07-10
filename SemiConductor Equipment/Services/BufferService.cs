using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class BufferService : IBufferManager
    {
        private readonly DbLogHelper _logHelper;
        private readonly object _lock = new();
        private readonly Dictionary<string, (Wafer? wafer, bool isProcessing)> _bufferSlots = new()
        {
            ["Buffer1"] = (null, false),
            ["Buffer2"] = (null, false),
            ["Buffer3"] = (null, false),
            ["Buffer4"] = (null, false),
        };

        public event EventHandler<BufferStatus> DataEnqueued;
        public event EventHandler<RobotCommand> Enque_Robot;

        public IDictionary<string, string> Buffer_State { get; set; } = new Dictionary<string, string>()
        {
            ["Buffer1"] = "UN USE",
            ["Buffer2"] = "UN USE",
            ["Buffer3"] = "UN USE",
            ["Buffer4"] = "UN USE"
        };

        public BufferService(DbLogHelper logHelper)
        {
            this._logHelper = logHelper;
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
                this.Buffer_State[buffername] = "IN USE";
                DataEnqueued?.Invoke(this, new BufferStatus(buffername, this.Buffer_State[buffername]));
                this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "IN");
            }

            // 프로세스 시뮬레이션 (예: 3초)
            await Task.Delay(10000);

            lock (_lock)
            {
                // 처리 완료 상태로 변경 (processing = false)
                _bufferSlots[buffername] = (wafer, true);
            }

            Enque_Robot?.Invoke(this, new RobotCommand
            {
                CommandType = RobotCommandType.MoveTo,
                Wafer = wafer,
                Location = "LoadPort",
                Completed = buffername
            });

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
                    this.Buffer_State[buffername] = "UN USE";
                    DataEnqueued?.Invoke(this, new BufferStatus(buffername, this.Buffer_State[buffername]));
                    _bufferSlots[buffername] = (null, false);
                }
            }
        }

        public void AddWaferToBuffer(string buffername, Wafer wafer)
        {
            lock (_lock)
            {
                _bufferSlots[buffername] = (wafer, false);
            }
        }

        public bool IsAllBufferEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return _bufferSlots.Values.All(buffer => buffer.wafer == null);
        }
    }
}
