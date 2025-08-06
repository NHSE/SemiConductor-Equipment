using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
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
        private readonly IEventMessageManager _eventMessageManager;
        private readonly IEquipmentConfigManager _equiptempManager;
        public event EventHandler<CleanChamberStatus> DataEnqueued;
        public event EventHandler<CleanChamberStatus> MultiCupChange;
        public event EventHandler<RobotCommand> Enque_Robot;
        public event EventHandler<ChemicalStatus> ChemicalChange;

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
            ["Chamber1"] = "UN USE",
            ["Chamber2"] = "UN USE",
            ["Chamber3"] = "UN USE",
            ["Chamber4"] = "UN USE",
            ["Chamber5"] = "UN USE",
            ["Chamber6"] = "UN USE",
        };

        public CleanService(IEventMessageManager eventMessageManager, IEquipmentConfigManager equiptempManager)
        {
            //this._logHelper = logHelper;
            this._eventMessageManager = eventMessageManager;
            this._equiptempManager = equiptempManager;
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
                this.Clean_State[chambername] = "IN USE";
                DataEnqueued?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
                //this._logHelper.WriteDbLog(buffername, _bufferSlots[buffername].wafer, "IN");
            }
            //멀티컵 Up
            await Task.Delay(1500);
            MultiCupChange?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
            //ok 사인 받아야함

            //RPM 증가 RPM은 100 이상이 되게끔

            Console.WriteLine($"[CleanChamber | {chambername}] Start Spin");
            float current_rpm = 0;
            int target_rpm = this._equiptempManager.RPM;
            float increment = (float)(target_rpm / 100.0);

            while (Math.Abs(current_rpm - target_rpm) > 1)  // 목표와 1 이하 차이면 종료
            {
                if (current_rpm < target_rpm)
                {
                    current_rpm += increment;
                    if (current_rpm > target_rpm) current_rpm = target_rpm; // 오버런 방지
                }
                else if (current_rpm > target_rpm)
                {
                    current_rpm -= 1;
                    if (current_rpm < target_rpm) current_rpm = target_rpm;
                }

                await Task.Delay(100);
            }

            Console.WriteLine($"[CleanChamber | {chambername}] End Spin");
            Console.WriteLine($"[CleanChamber | {chambername}] Start Cleaning");

            //RPM 증가 후 세정액 분사
            bool Error_flag = false;
            for (int time = 0; time < this._equiptempManager.Spray_Time; time++)
            {
                await Task.Delay(60000); // 1 min
                // 세정액 감소
                var Status = new ChemicalStatus(chambername, this._equiptempManager.Flow_Rate);
                ChemicalChange?.Invoke(this, Status);

                if(Status.Result) // 다 떨어졌을 시
                {
                    // 테스트 강제 종료
                    Unable_to_Process[chambername] = true; // 사용 금지 락
                    wafer.Status = "Error";
                    ProcessComplete(chambername, wafer, "LoadPort");
                    Error_flag = true;
                    // 에러 로그 발생
                }
            }

            //RPM 감소
            while (current_rpm > 0)
            {
                current_rpm -= 10;

                await Task.Delay(100);
            }

            //멀티컵 Down
            await Task.Delay(1500);
            MultiCupChange?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));

            Console.WriteLine($"[CleanChamber | {chambername}] End Cleaning");

            if(!Error_flag)
                ProcessComplete(chambername, wafer, "Dry Chamber");

            Console.WriteLine($"[CleanChamber] {wafer.SlotId} process done in {chambername}");
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
                if (Unable_to_Process[chambername])
                    this.Clean_State[chambername] = "DISAB";
                else
                    this.Clean_State[chambername] = "UN USE";
                DataEnqueued?.Invoke(this, new CleanChamberStatus(chambername, this.Clean_State[chambername]));
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
    }
}
