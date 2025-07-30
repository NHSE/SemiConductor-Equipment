using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using System.Net.Http.Headers;
using static SemiConductor_Equipment.Models.EventInfo;
using Secs4Net;

namespace SemiConductor_Equipment.Services
{
    public class ChamberService : IChamberManager
    {
        private readonly object _lock = new();
        private readonly ILogManager _logManager;
        //private readonly DbLogHelper _logHelper;
        private readonly IEquipmentConfigManager _equiptempManager;
        private readonly IEventMessageManager _eventMessageManager;

        private readonly Dictionary<string, (Wafer? wafer, bool isProcessing)> _chambers = new()
        {
            ["Chamber1"] = (null, false),
            ["Chamber2"] = (null, false),
            ["Chamber3"] = (null, false),
            ["Chamber4"] = (null, false),
            ["Chamber5"] = (null, false),
            ["Chamber6"] = (null, false)
        };

        public event EventHandler<ChamberStatus> DataEnqueued;
        public event EventHandler<Wafer> ChangeTempData;
        public event EventHandler<RobotCommand> Enque_Robot;
        public event Action ProcessHandled;

        public IDictionary<string, string> Chamber_State { get; set; } = new Dictionary<string, string>()
        {
            ["Chamber1"] = "IDLE",
            ["Chamber2"] = "IDLE",
            ["Chamber3"] = "IDLE",
            ["Chamber4"] = "IDLE",
            ["Chamber5"] = "IDLE",
            ["Chamber6"] = "IDLE"
        };

        public ChamberService(ILogManager logManager, IEquipmentConfigManager equiptempManager, IEventMessageManager eventMessageManager)
        {
            this._logManager = logManager;
            //this._logHelper = logHelper;
            this._equiptempManager = equiptempManager;
            this._eventMessageManager = eventMessageManager;
        }

        public void ProcessStart()
        {
            ProcessHandled?.Invoke();
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
                    //this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "OUT");

                    this.Chamber_State[chamberName] = "IDLE";
                    lock (_lock)
                    {
                        DataEnqueued?.Invoke(this, new ChamberStatus(chamberName, this.Chamber_State[chamberName], _chambers[chamberName].wafer.Wafer_Num));
                    }
                    this._chambers[chamberName] = (null, false);
                }
            }
        }

        public void AddWaferToChamber(string chamberName, Wafer wafer)
        {
            lock (_lock)
            {
                // 웨이퍼 넣기 + 처리중 상태 표시
                this._chambers[chamberName] = (wafer, false);
            }
        }

        public async Task StartProcessingAsync(string chamberName, Wafer wafer)
        {
            try
            {
                lock (_lock)
                {
                    this.Chamber_State[chamberName] = "Running";
                    DataEnqueued?.Invoke(this, new ChamberStatus(chamberName, this.Chamber_State[chamberName], wafer.Wafer_Num));
                    ChangeTempData?.Invoke(this, wafer);

                    CEIDInfo info = this._eventMessageManager.GetCEID(300);
                    info.Wafer_number = wafer.Wafer_Num;
                    info.Loadport_Number = wafer.LoadportId;
                    this._eventMessageManager.EnqueueEventData(info);
                }

                this._logManager.WriteLog(chamberName, $"State", $"{wafer.Wafer_Num} in {chamberName}");
                //this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "IN");

                Random rand = new Random();

                for (int i = 0; i < this._equiptempManager.Chamber_Time; i++) // 설정된 시간 동안
                {
                    double prev_temp = wafer.RequiredTemperature;
                    wafer.RequiredTemperature += rand.Next(1, 6); // 1~5도 증가
                    wafer.RunningTime += 1;
                    this._logManager.WriteLog(chamberName, $"State", $"Wafer Temperature : {prev_temp} → {wafer.RequiredTemperature}");
                    await Task.Delay(1000); // 1초 대기
                    ChangeTempData?.Invoke(this, wafer);
                }
                //Processing

                if (wafer.RequiredTemperature < this._equiptempManager.Min_Temp || wafer.RequiredTemperature > this._equiptempManager.Max_Temp)
                {
                    wafer.Status = "Error";
                }
                else
                {
                    wafer.Status = "Completed";
                }

                    lock (_lock)
                    {
                        // 처리 완료 상태로 변경 (processing = false)
                        this._chambers[chamberName] = (wafer, true);
                        CEIDInfo info = this._eventMessageManager.GetCEID(301);
                        info.Wafer_number = wafer.Wafer_Num;
                        info.Loadport_Number = wafer.LoadportId;
                        this._eventMessageManager.EnqueueEventData(info);
                    }

                if (wafer.Status == "Completed")
                {
                    Enque_Robot?.Invoke(this, new RobotCommand
                    {
                        CommandType = RobotCommandType.MoveTo,
                        Wafer = wafer,
                        Location = "Buffer",
                        Completed = chamberName
                    });
                }
                else if (wafer.Status == "Error")
                {
                    Enque_Robot?.Invoke(this, new RobotCommand
                    {
                        CommandType = RobotCommandType.Error,
                        Wafer = wafer,
                        Location = "LoadPort",
                        Completed = chamberName
                    });
                }

                this._logManager.WriteLog(chamberName, $"State", $"[{chamberName}] {wafer.SlotId} process done in {chamberName}");
                //this._logHelper.WriteDbLog(chamberName, _chambers[chamberName].wafer, "DONE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartProcessingAsync 예외: " + ex);
                throw;
            }
        }

        public bool Processing(string ChamberName, Wafer Wafer)
        {
            bool ret = false;

            return ret;
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
