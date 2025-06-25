using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;

namespace SemiConductor_Equipment.Services
{
    public class ChamberService
    {
        private readonly object _lock = new();
        private readonly ILogManager _logManager;
        private readonly DbLogHelper _logHelper;
        private readonly RobotArmService _robotArmService;
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

        public ChamberService(ILogManager logManager, DbLogHelper logHelper, RobotArmService robotArmService)
        {
            this._logManager = logManager;
            this._logHelper = logHelper;
            this._robotArmService = robotArmService;
        }

        public string? FindEmptyChamber()
        {
            lock (_lock)
            {
                return this._chambers.FirstOrDefault(x => x.Value.wafer == null && x.Value.isProcessing == false).Key;
            }
        }

        public (string ChamberName, Wafer Wafer)? PeekCompletedWafer()
        {
            lock (_lock)
            {
                var completed = this._chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing != false);
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
                    this._logManager.WriteLog(chamberName, $"State", $"{_chambers[chamberName].wafer.Wafer_Num} Out {chamberName}");
                    this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "OUT");

                    this._chambers[chamberName] = (null, false);
                    DataEnqueued?.Invoke(this, chamberName);
                }
            }
        }

        public async Task StartProcessingAsync(string chamberName, Wafer wafer)
        {
            try
            {
                lock (_lock)
                {
                    // 웨이퍼 넣기 + 처리중 상태 표시
                    this._chambers[chamberName] = (wafer, false);
                    DataEnqueued?.Invoke(this, chamberName);
                }

                this._logManager.WriteLog(chamberName, $"State", $"{wafer.Wafer_Num} in {chamberName}");
                this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "IN");

                // 프로세스 시뮬레이션 (예: 3초)
                await Task.Delay(10000);
                //Wafer 프로세싱 성공, 실패 로직
                if (wafer.Wafer_Num % 2 == 0)
                {
                    wafer.Status = "Error";
                }
                else wafer.Status = "Completed";

                lock (_lock)
                {
                    // 처리 완료 상태로 변경 (processing = false)
                    this._chambers[chamberName] = (wafer, true);
                }

                if (wafer.Status == "Completed")
                {
                    this._robotArmService.EnqueueCommand_Chamber(new RobotCommand
                    {
                        CommandType = RobotCommandType.MoveTo,
                        Wafer = wafer,
                        Location = wafer.TargetLocation
                    });
                }
                else if (wafer.Status == "Error")
                {
                    this._robotArmService.EnqueueCommand_Buffer(new RobotCommand
                    {
                        CommandType = RobotCommandType.Error,
                        Wafer = wafer,
                        Location = wafer.TargetLocation
                    });
                }

                this._logManager.WriteLog(chamberName, $"State", $"[{chamberName}] {wafer.SlotId} process done in {chamberName}");
                this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "DONE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartProcessingAsync 예외: " + ex);
                throw;
            }
        }

        public (string ChamberName, Wafer Wafer)? FindCompletedWafer()
        {
            lock (_lock)
            {
                var completed = this._chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing == false);
                if (completed.Value.wafer == null) return null;

                this._chambers[completed.Key] = (null, false);
                return (completed.Key, completed.Value.wafer);
            }
        }

        public bool TryInsertWafer(string chamberName, Wafer wafer)
        {
            lock (_lock)
            {
                if (this._chambers.ContainsKey(chamberName) && this._chambers[chamberName].wafer == null)
                {
                    this._chambers[chamberName] = (wafer, false);
                    return true;
                }
                return false;
            }
        }

        public bool IsAllChamberEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return this._chambers.Values.All(chamber => chamber.wafer == null);
        }
    }
}
