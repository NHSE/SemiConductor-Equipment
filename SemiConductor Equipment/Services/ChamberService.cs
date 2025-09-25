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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.XPath;
using System.Diagnostics;

namespace SemiConductor_Equipment.Services
{
    public class ChamberService : IChamberManager
    {
        #region FIELDS
        private readonly object _lock = new();
        private readonly ILogManager _logManager;
        private readonly IEquipmentConfigManager _equiptempManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly IVIDManager _vIDManager;
        private readonly IResultFileManager _resultFileManager;
        private readonly IPLCManager _plcManager;

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
        public event EventHandler<ChamberRPMValue> ChangeRPMData;
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
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Dry Chamber의 동작 서비스 레이어
        /// </summary>
        /// <param name="logManager"></param>
        /// <param name="equiptempManager"></param>
        /// <param name="eventMessageManager"></param>
        /// <param name="vIDManager"></param>
        /// <param name="resultFileManager"></param>
        public ChamberService(ILogManager logManager, IEquipmentConfigManager equiptempManager, IEventMessageManager eventMessageManager, 
            IVIDManager vIDManager, IResultFileManager resultFileManager, IPLCManager plcManager)
        {
            this._logManager = logManager;
            this._equiptempManager = equiptempManager;
            this._eventMessageManager = eventMessageManager;
            this._vIDManager = vIDManager;
            this._resultFileManager = resultFileManager;
            this._plcManager = plcManager;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// Dry Chamber 내 웨이퍼가 들어왔을 때 공정 시작을 알리는 메서드 (이벤트 발생으로 각 Chamber의 온도 그래프 초기화)
        /// </summary>
        public void ProcessStart()
        {
            ProcessHandled?.Invoke();
        }

        /// <summary>
        /// 비어있는 Chamber 확인 메서드
        /// </summary>
        /// <returns>비어있는 Chamber 이름</returns>
        public string? FindEmptyChamber()
        {
            return this._chambers.FirstOrDefault(x => x.Value.wafer == null && x.Value.isProcessing == false).Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public (string ChamberName, Wafer Wafer)? PeekCompletedWafer()
        {
            var completed = this._chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing != false);
            if (completed.Value.wafer == null) return null;
            return (completed.Key, completed.Value.wafer);
        }


        /// <summary>
        /// 공정을 마친 웨이퍼의 정보를 Chamber 내 데이터에서 삭제
        /// </summary>
        /// <param name="chamberName"></param>
        public void RemoveWaferFromChamber(string chamberName)
        {
            if (_chambers.ContainsKey(chamberName))
            {
                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"{_chambers[chamberName].wafer.Wafer_Num} Out {chamberName}");

                this.Chamber_State[chamberName] = "IDLE";
                DataEnqueued?.Invoke(this, new ChamberStatus(chamberName, this.Chamber_State[chamberName], _chambers[chamberName].wafer.Wafer_Num));
                this._chambers[chamberName] = (null, false);
            }
        }

        /// <summary>
        /// Chamber 내 웨이퍼의 정보를 추가
        /// </summary>
        /// <param name="chamberName"></param>
        /// <param name="wafer"></param>
        public void AddWaferToChamber(string chamberName, Wafer wafer)
        {
            this._chambers[chamberName] = (wafer, false);
        }

        /// <summary>
        /// Dry 공정 프로세스 실행 메서드
        /// </summary>
        /// <param name="chamberName"></param>
        /// <param name="wafer"></param>
        /// <returns></returns>
        public async Task StartProcessingAsync(string chamberName, Wafer wafer)
        {
            try
            {
                ResultData result = new ResultData(); // 공정 결과 데이터
                var sw = Stopwatch.StartNew();
                lock (_lock)
                {
                    this.Chamber_State[chamberName] = "Running";
                    DataEnqueued?.Invoke(this, new ChamberStatus(chamberName, this.Chamber_State[chamberName], wafer.Wafer_Num));
                    ChangeTempData?.Invoke(this, wafer);

                    CEIDInfo info = this._eventMessageManager.GetCEID(300);
                    info.Wafer_number = wafer.Wafer_Num;
                    info.Loadport_Number = wafer.LoadportId;
                    this._eventMessageManager.EnqueueEventData(info);

                    result = new ResultData
                    {
                        StartTime = DateTime.Now,
                        SlotNo = wafer.Wafer_Num,
                        LoadPort = wafer.LoadportId.ToString(),
                        CarrierID = wafer.CarrierId,
                        CJID = wafer.CJId,
                        PJID = wafer.PJId,
                        ChamberName = chamberName,
                        TargetMinTemperature = this._equiptempManager.Min_Temp,
                        TargetMaxTemperature = this._equiptempManager.Max_Temp,
                    };
                }

                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"{wafer.Wafer_Num} in {chamberName}");

                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] Start Spin");

                int current_rpm = 0;
                int target_rpm = this._equiptempManager.Clean_RPM;
                current_rpm = Task.Run(() =>
                {
                    return this._plcManager.PLC_Start(chamberName, target_rpm, current_rpm, false).GetAwaiter().GetResult();
                }).Result;

                /*
                float current_rpm = 0;
                int target_rpm = this._equiptempManager.Dry_RPM;
                int max_random = this._equiptempManager.Dry_RPM / 10;
                int min_random = (this._equiptempManager.Dry_RPM / 50) == 0 ? 1 : this._equiptempManager.Dry_RPM / 50;
                Random rand = new Random();

                
                while (Math.Abs(current_rpm - target_rpm) > 1)
                {
                    if (current_rpm < target_rpm)
                    {
                        current_rpm += rand.Next(min_random, max_random);
                        if (current_rpm > target_rpm) current_rpm = target_rpm; // 오버런 방지
                    }
                    else if (current_rpm > target_rpm)
                    {
                        current_rpm -= rand.Next(min_random, max_random);
                        if (current_rpm < target_rpm) current_rpm = target_rpm;
                    }

                    this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] Rotational Speed : {(int)current_rpm} rpm");
                    ChangeRPMData?.Invoke(this, new ChamberRPMValue(chamberName, current_rpm));
                    await Task.Delay(1000);
                }
                */

                result.RPM = (int)current_rpm;
                Random rand = new Random();

                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] End Spin");

                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] Start Gas");
                for (int i = 0; i < this._equiptempManager.Chamber_Time; i++) // 설정된 시간 동안
                {
                    double prev_temp = wafer.RequiredTemperature;
                    wafer.RequiredTemperature += rand.Next(1, 6); // 1~5도 증가
                    wafer.RunningTime += 1;
                    this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"Wafer Temperature : {prev_temp} → {wafer.RequiredTemperature}");
                    await Task.Delay(1000); // 1초 대기
                    ChangeTempData?.Invoke(this, wafer);
                }

                result.ActualTemperature = (int)wafer.RequiredTemperature;
                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] End Gas");
                //Processing

                if (wafer.RequiredTemperature < this._equiptempManager.Min_Temp || wafer.RequiredTemperature > this._equiptempManager.Max_Temp)
                {
                    wafer.Status = "Error";
                    if (wafer.RequiredTemperature < this._equiptempManager.Min_Temp)
                        result.ErrorInfo = "Wafer temperature did not reach the target minimum temperature.";
                    else if (wafer.RequiredTemperature > this._equiptempManager.Max_Temp)
                        result.ErrorInfo = "Wafer temperature exceeded the target maximum temperature.";

                    result.HasAlarm = true;
                }
                else
                    wafer.Status = "Completed";

                /*
                while (current_rpm > 0)
                {
                    current_rpm -= rand.Next(min_random, max_random);
                    if (current_rpm < 0)
                    {
                        current_rpm = 0;
                    }
                    this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] Rotational Speed : {(int)current_rpm} rpm");
                    ChangeRPMData?.Invoke(this, new ChamberRPMValue(chamberName, current_rpm));
                    await Task.Delay(1000);
                }
                */
                await this._plcManager.PLC_Stop(chamberName, false);
                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] Spin Stop");

                lock (_lock)
                {
                    // 처리 완료 상태로 변경 (processing = false)
                    this._chambers[chamberName] = (wafer, true);
                    this._vIDManager.SetDVID(1001, wafer.RequiredTemperature, wafer.Wafer_Num);
                    CEIDInfo info = this._eventMessageManager.GetCEID(301);
                    info.Wafer_number = wafer.Wafer_Num;
                    info.Loadport_Number = wafer.LoadportId;
                    this._eventMessageManager.EnqueueEventData(info);
                }

                this.Chamber_State[chamberName] = "DONE";
                DataEnqueued?.Invoke(this, new ChamberStatus(chamberName, this.Chamber_State[chamberName], wafer.Wafer_Num));

                if (wafer.Status == "Error")
                {
                    Enque_Robot?.Invoke(this, new RobotCommand
                    {
                        CommandType = RobotCommandType.Error,
                        Wafer = wafer,
                        Location = "Dry",
                        NextLocation = "LoadPort",
                        Completed = chamberName
                    });
                }
                else
                {
                    Enque_Robot?.Invoke(this, new RobotCommand
                    {
                        CommandType = RobotCommandType.MoveTo,
                        Wafer = wafer,
                        Location = "Dry",
                        NextLocation = "LoadPort",
                        Completed = chamberName
                    });
                    result.Yield = true;
                }

                sw.Stop();
                result.EndTime = DateTime.Now;
                result.ProcessDuration = sw.Elapsed;

                this._logManager.WriteLog($"Dry_{chamberName}", $"State", $"[{chamberName}] {wafer.SlotId} process done in {chamberName}");

                this._resultFileManager.InsertData("Dry", new LoadPortWaferKey(wafer.LoadportId, wafer.Wafer_Num), result);
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
            var completed = this._chambers.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing == false);
            if (completed.Value.wafer == null) return null;

            this._chambers[completed.Key] = (null, false);
            return (completed.Key, completed.Value.wafer);
        }

        public bool TryInsertWafer(string chamberName, Wafer wafer)
        {
            if (this._chambers.ContainsKey(chamberName) && this._chambers[chamberName].wafer == null)
            {
                this._chambers[chamberName] = (wafer, false);
                return true;
            }
            return false;

        }

        /// <summary>
        /// 모든 챔버내 웨이퍼가 없는 지 확인하는 메서드
        /// </summary>
        /// <returns>웨이퍼 존재 여부</returns>
        public bool IsAllChamberEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return this._chambers.Values.All(chamber => chamber.wafer == null);
        }
        #endregion
    }
}
