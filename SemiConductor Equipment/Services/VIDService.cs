using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Secs4Net;
using static Secs4Net.Item;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public partial class VIDService : IVIDManager
    {
        #region FIELDS
        private readonly IEventConfigManager _eventConfigManager;
        private readonly List<int> vid_list = new List<int> { 1, 3, 7, 8, 9, 10, 11, 1001, 1002, 1003, 1004, 1005 };
        #endregion

        #region PROPERTIES
        public string? EquipmentStatus { get; set; }
        public int?[] WaferTemp { get; set; } = new int?[26];
        public int?[] LoadportWaferCount { get; set; } = new int?[3] { 0, 0, 0 };
        public string? RecipeData { get; set; }
        public string?[] LotId { get; set; } = new string?[26];
        public string?[] WaferId { get; set; } = new string?[26];
        public string? RobotStatus { get; set; }
        public string?[] LoadportDoorStatus { get; set; } = new string?[3];
        public string? LastAlarmCode { get; set; }
        public string? OperationMode { get; set; }
        public string? SWVersion { get; set; }
        public string?[] WaferPosition { get; set; } = new string?[26];

        #endregion

        #region CONSTRUCTOR
        public VIDService(IEventConfigManager eventConfigManager)
        {
            this._eventConfigManager = eventConfigManager;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        public List<Item>? GetRPTID(int rptid, object wafer_number, int loadport_number)
        {
            var RPTID_data = this._eventConfigManager.RPTID[rptid];

            var vidItems = new List<Item>();
            foreach (var svid in RPTID_data.VIDs)
            {
                object vidData;
                int arrNum = 0;

                if (svid != 8 && svid != 3)
                {
                    if (wafer_number is int wNum)
                        arrNum = wNum;
                }
                else
                {
                    arrNum = loadport_number;
                }

                if (svid == 1001)
                {
                    // wafer_number가 List<int>인지 검사
                    if (wafer_number is IEnumerable<int> waferList)
                    {
                        foreach (var v in waferList)
                        {
                            vidData = this.GetDVID(svid, v);
                            vidItems.Add(
                                L(
                                    U4((uint)rptid),
                                    L(
                                        A(vidData?.ToString() ?? "")
                                    )
                                )
                            );
                        }
                    }
                    continue;
                }

                else if (svid != 1001 && svid < 1000)
                    vidData = this.GetSVID(svid, arrNum);
                else
                    vidData = this.GetDVID(svid, arrNum);

                vidItems.Add(
                    L(
                        U4((uint)rptid),
                        L(
                            A(vidData?.ToString() ?? "")
                        )
                    )
                );
            }
            return vidItems;
        }

        public object? GetSVID(int svid, int array_data)
        {
            return svid switch
            {
                1 => EquipmentStatus,
                3 => LoadportWaferCount[array_data],
                7 => RobotStatus,
                8 => LoadportDoorStatus[array_data],
                9 => LastAlarmCode,
                10 => OperationMode,
                11 => SWVersion,
                _ => null
            };
        }

        public object? GetDVID(int svid, int array_data)
        {
            return svid switch
            {
                1001 => WaferTemp[array_data],
                1002 => RecipeData,
                1003 => LotId[array_data],
                1004 => WaferId[array_data],
                1005 => WaferPosition[array_data],
                _ => null
            };
        }

        public void SetSVID(int svid, object data, int array_data = 0)
        {
            switch (svid)
            {
                case 1:
                    EquipmentStatus = data.ToString();
                    break;

                case 3:
                    LoadportWaferCount[array_data] = (int)data;
                    break;

                case 7:
                    RobotStatus = data.ToString();
                    break;

                case 8:
                    LoadportDoorStatus[array_data] = data.ToString();
                    break;

                case 9:
                    LastAlarmCode = data.ToString();
                    break;

                case 10:
                    OperationMode = data.ToString();
                    break;

                case 11:
                    SWVersion = data.ToString();
                    break;
            }
        }

        public void SetDVID(int svid, object data, int array_data = 0)
        {
            switch (svid)
            {
                case 1001:
                    WaferTemp[array_data] = (int)data;
                    break;

                case 1002:
                    RecipeData = data.ToString();
                    break;

                case 1003:
                    LotId[array_data] = data.ToString();
                    break;

                case 1004:
                    WaferId[array_data] = data.ToString();
                    break;

                case 1005:
                    WaferPosition[array_data] = data.ToString();
                    break;
            }
        }

        public bool IsRPTID(uint rptid)
        {
            if (!this._eventConfigManager.RPTID.ContainsKey((int)rptid))
                return false;
            else
                return true;
        }

        public bool IsVID(uint rptid, uint vid)
        {
            return this.vid_list.Contains((int)vid);
        }

        public bool IsCEID(uint ceid)
        {
            if (!this._eventConfigManager.CEID.ContainsKey((int)ceid))
                return false;
            else
                return true;
        }

        public bool IsRPTIDInCEID(uint ceid, uint rptid)
        {
            if (!this._eventConfigManager.CEID[(int)ceid].RPTIDs.Contains((int)rptid))
                return false;
            else
                return true;
        }
        #endregion
    }
}
