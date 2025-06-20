using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class ChamberService
    {
        private readonly object _lock = new();
        private readonly ILogManager _logManager;
        public event EventHandler<string> DataEnqueued;

        private readonly Dictionary<string, (Wafer? wafer, bool isProcessing)> _chambers = new()
        {
            ["Chamber1"] = (null, false),
            ["Chamber2"] = (null, false),
            ["Chamber3"] = (null, false),
            ["Chamber4"] = (null, false),
            ["Chamber5"] = (null, false),
            ["Chamber6"] = (null, false)
        };

        public ChamberService(ILogManager logManager)
        {
            this._logManager = logManager;
        }

        public string? FindEmptyChamber()
        {
            lock (_lock)
            {
                return _chambers.FirstOrDefault(x => x.Value.wafer == null && x.Value.isProcessing == false).Key;
            }
        }

        public (string ChamberName, Wafer Wafer)? PeekCompletedWafer()
        {
            lock (_lock)
            {
                var completed = _chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing != false);
                if (completed.Value.wafer == null) return null;
                return (completed.Key, completed.Value.wafer);
            }
        }

        public void RemoveWaferFromChamber(string chamberName)
        {
            lock (_lock)
            {
                if (_chambers.ContainsKey(chamberName))
                {
                    _logManager.WriteLog(chamberName, $"State", $"{_chambers[chamberName].wafer.Wafer_Num} Out {chamberName}");
                    _chambers[chamberName] = (null, false);
                    DataEnqueued?.Invoke(this, chamberName);
                }
            }
        }

        public async Task StartProcessingAsync(string chamberName, Wafer wafer)
        {
            lock (_lock)
            {
                // 웨이퍼 넣기 + 처리중 상태 표시
                _chambers[chamberName] = (wafer, false);
                DataEnqueued?.Invoke(this, chamberName);
            }

            _logManager.WriteLog(chamberName, $"State", $"{wafer.Wafer_Num} in {chamberName}");
            // 프로세스 시뮬레이션 (예: 3초)
            await Task.Delay(10000);

            lock (_lock)
            {
                // 처리 완료 상태로 변경 (processing = false)
                _chambers[chamberName] = (wafer, true);
            }

            //Console.WriteLine($"[Chamber] {wafer.SlotId} process done in {chamberName}");
            _logManager.WriteLog(chamberName, $"State", $"[{chamberName}] {wafer.SlotId} process done in {chamberName}");
        }

        public (string ChamberName, Wafer Wafer)? FindCompletedWafer()
        {
            lock (_lock)
            {
                var completed = _chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing == false);
                if (completed.Value.wafer == null) return null;

                _chambers[completed.Key] = (null, false);
                return (completed.Key, completed.Value.wafer);
            }
        }

        public bool TryInsertWafer(string chamberName, Wafer wafer)
        {
            lock (_lock)
            {
                if (_chambers.ContainsKey(chamberName) && _chambers[chamberName].wafer == null)
                {
                    _chambers[chamberName] = (wafer, false);
                    return true;
                }
                return false;
            }
        }

        public bool IsAllChamberEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return _chambers.Values.All(chamber => chamber.wafer == null);
        }
    }
}
