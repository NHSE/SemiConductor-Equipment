using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors.Media;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public class CleanService : ICleanManager
    {
        //private readonly DbLogHelper _logHelper;
        private readonly ILogManager _logManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly IEquipmentConfigManager _equiptempManager;
        public event EventHandler<CleanChamberStatus> DataEnqueued;
        public event EventHandler<CleanChamberStatus> MultiCupChange;
        public event EventHandler<RobotCommand> Enque_Robot;
        public event EventHandler<ChemicalStatus> ChemicalChange;
        public event EventHandler<ChemicalStatus> PreCleanChange;
        public event EventHandler<ChamberData> CleanChamberChange;

        private readonly object _lock = new();

        private readonly Dictionary<string, (Wafer? wafer, bool isProcessing)> _chamberSlots = new()
        {
            ["Chamber1"] = (null, false),
            ["Chamber2"] = (null, false),
            ["Chamber3"] = (null, false),
            ["Chamber4"] = (null, false),
            ["Chamber5"] = (null, false),
            ["Chamber6"] = (null, false),
        };
        public Dictionary<string, bool> Unable_to_Process { get; set; } = new Dictionary<string, bool>()
        {
            ["Chamber1"] = false,
            ["Chamber2"] = false,
            ["Chamber3"] = false,
            ["Chamber4"] = false,
            ["Chamber5"] = false,
            ["Chamber6"] = false,
        };
        public IDictionary<string, string> Clean_State { get; set; } = new Dictionary<string, string>()
        {
            ["Chamber1"] = "IDLE",
            ["Chamber2"] = "IDLE",
            ["Chamber3"] = "IDLE",
            ["Chamber4"] = "IDLE",
            ["Chamber5"] = "IDLE",
            ["Chamber6"] = "IDLE",
        };

        public CleanService(IEventMessageManager eventMessageManager, IEquipmentConfigManager equiptempManager, ILogManager logManager)
        {
            //this._logHelper = logHelper;
            this._eventMessageManager = eventMessageManager;
            this._equiptempManager = equiptempManager;
            this._logManager = logManager;
        }

        public string? FindEmptySlot()
        {
            return _chamberSlots
                .Where(x => x.Value.wafer == null
                         && x.Value.isProcessing == false
                         && Unable_to_Process.ContainsKey(x.Key)
                         && Unable_to_Process[x.Key] == false)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        public (string ChamberName, Wafer Wafer)? PeekCompletedWafer()
        {
            var completed = _chamberSlots.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing != false);
            if (completed.Value.wafer == null) return null;
            return (completed.Key, completed.Value.wafer);
        }

        public async Task StartProcessingAsync(string chambername, Wafer wafer)
        {
            lock (_lock)
            {
                // 웨이퍼 넣기 + 처리중 상태 표시
                this.Clean_State[chambername] = "Running";
                DataEnqueued?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
                CleanChamberChange?.Invoke(this, new ChamberData(chambername, wafer));
                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"{wafer.Wafer_Num} in {chambername}");
                //this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "IN");
            }
            //멀티컵 Up
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Start MultiCup Up");
            await Task.Delay(1500);
            MultiCupChange?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] END MultiCup Up");

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Start Spin");
            float current_rpm = 0;
            int target_rpm = this._equiptempManager.RPM;
            Random rand = new Random();

            while (Math.Abs(current_rpm - target_rpm) > 1)  // 목표와 1 이하 차이면 종료
            {
                if (current_rpm < target_rpm)
                {
                    current_rpm += rand.Next(1, 5);
                    if (current_rpm > target_rpm) current_rpm = target_rpm; // 오버런 방지
                }
                else if (current_rpm > target_rpm)
                {
                    current_rpm += rand.Next(1, 5);
                    if (current_rpm < target_rpm) current_rpm = target_rpm;
                }

                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)current_rpm} rpm");
                await Task.Delay(1000);
            }

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] End Spin");

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Start Cleaning");

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Pre-Clean] Start Cleaning");

            //Pre-Clean
            bool Error_flag = false;
            for (int time = 0; time < this._equiptempManager.PreClean_Spray_Time; time++)
            {
                await Task.Delay(1000); // 1 sec
                // 세정액 감소
                var Status = new ChemicalStatus(chambername, this._equiptempManager.PreClean_Flow_Rate);
                PreCleanChange?.Invoke(this, Status);

                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Pre-Clean] Cleaning Time : {time + 1}");

                if (Status.Result) // 다 떨어졌을 시
                {
                    // 테스트 강제 종료
                    Unable_to_Process[chambername] = true; // 사용 금지 락
                    wafer.Status = "Error";
                    Error_flag = true;
                    this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Pre-Clean] An error has occurred due to insufficient Pre-Clean Supply");
                    break;
                    // 에러 로그 발생
                }
            }
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Pre-Clean] END Cleaning");
            await Task.Delay(1000); // 1 sec

            if (!Error_flag)
            {
                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Chemical] Start Cleaning");
                //Chemical
                for (int time = 0; time < this._equiptempManager.Spray_Time; time++)
                {
                    await Task.Delay(1000); // 1 sec
                                            // 세정액 감소
                    var Status = new ChemicalStatus(chambername, this._equiptempManager.Flow_Rate);
                    ChemicalChange?.Invoke(this, Status);

                    this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Chemical] Cleaning Time : {time + 1}");

                    if (Status.Result) // 다 떨어졌을 시
                    {
                        // 테스트 강제 종료
                        Unable_to_Process[chambername] = true; // 사용 금지 락
                        wafer.Status = "Error";
                        Error_flag = true;
                        this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Chemical] An error has occurred due to insufficient Chemical Supply");
                        break;
                        // 에러 로그 발생
                    }
                }
                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}][Chemical] END Cleaning");
            }

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] END Cleaning");
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Initiate Spin Stop");
            //RPM 감소
            while (current_rpm > 0)
            {
                current_rpm -= rand.Next(1, 5);
                if (current_rpm < 0)
                {
                    current_rpm = 0;
                }
                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)current_rpm} rpm");
                await Task.Delay(1000);
            }
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Spin Stop");

            //멀티컵 Down
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Start MultiCup Down");
            await Task.Delay(1500);
            MultiCupChange?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] END MultiCup Down");

            if (!Error_flag)
            {
                ProcessComplete(chambername, wafer, "Dry Chamber");
                this.Clean_State[chambername] = "DONE";
            }
            else
            {
                ProcessComplete(chambername, wafer, "LoadPort");
                this.Clean_State[chambername] = "DISAB";
            }

            DataEnqueued?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));

            this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] {wafer.SlotId} process done in {chambername}");
            //this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "DONE");
        }

        private void ProcessComplete(string chambername, Wafer wafer, string next)
        {
            lock (_lock)
            {
                // 처리 완료 상태로 변경 (processing = false)
                _chamberSlots[chambername] = (wafer, true);
                CEIDInfo info = this._eventMessageManager.GetCEID(301);
                info.Wafer_number = wafer.Wafer_Num;
                info.Loadport_Number = wafer.LoadportId;
                this._eventMessageManager.EnqueueEventData(info);
            }

            Enque_Robot?.Invoke(this, new RobotCommand
            {
                CommandType = RobotCommandType.MoveTo,
                Wafer = wafer,
                Location = "Clean",
                NextLocation = next,
                Completed = chambername
            });
        }

        public (string ChamberName, Wafer Wafer)? FindCompletedWafer()
        {
            var completed = _chamberSlots.FirstOrDefault(x => x.Value.wafer != null && x.Value.isProcessing == false);
            if (completed.Value.wafer == null) return null;

            _chamberSlots[completed.Key] = (null, false);
            return (completed.Key, completed.Value.wafer);
        }

        public void RemoveWaferFromCleanChamber(string chambername)
        {
            if (_chamberSlots.ContainsKey(chambername))
            {
                //this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "OUT");
                if (!Unable_to_Process[chambername])
                    this.Clean_State[chambername] = "IDLE";

                DataEnqueued?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
                this._logManager.WriteLog($"Clean_{chambername}", $"State", $"{_chamberSlots[chambername].wafer.Wafer_Num} Out {chambername}");
                _chamberSlots[chambername] = (null, false);
            }
        }

        public void AddWaferToBuffer(string chambername, Wafer wafer)
        {
            _chamberSlots[chambername] = (wafer, false);
        }

        public bool IsAllCleanChamberEmpty()
        {
            // 모든 챔버에 Wafer가 없으면 true
            return _chamberSlots.Values.All(chamber => chamber.wafer == null);
        }

        public bool IsAllDisableChamber()
        {
            return Unable_to_Process.Values.All(value => value);
        }

        public bool CleanChamberEmpty(string chambername)
        {
            return _chamberSlots[chambername].wafer == null;
        }
    }
}
