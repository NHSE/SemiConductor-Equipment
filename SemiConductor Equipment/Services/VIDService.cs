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
        public string? EquipmentStatus { get; set; } = "Ready";
        public int?[] WaferTemp { get; set; } = new int?[26];
        public int?[] LoadportWaferCount { get; set; } = new int?[3] { 0, 0, 0 };
        public string? RecipeData { get; set; }
        public string?[] LotId { get; set; } = new string?[26];
        public string?[] WaferId { get; set; } = new string?[26];
        public string? RobotStatus { get; set; } = "IDLE";
        public string?[] LoadportDoorStatus { get; set; } = new string?[3] { "OPEN", "OPEN", "OPEN" };
        public string? LastAlarmCode { get; set; }
        public string? SWVersion { get; set; }
        public string?[] WaferPosition { get; set; } = new string?[26];

        public string?[] PJID { get; set; } = new string?[3] {"Nothing", "Nothing", "Nothing" };
        public string?[] CJID { get; set; } = new string?[3] { "Nothing", "Nothing", "Nothing" };
        public string?[] CarrierID { get; set; } = new string?[3] { "Nothing", "Nothing", "Nothing" };

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
            foreach (var vid in RPTID_data.VIDs)
            {
                object vidData;
                int arrNum = 0;

                if (vid != 1002 && vid != 102 && vid < 1008)
                {
                    if (wafer_number is int wNum)
                        arrNum = wNum;
                }
                else
                {
                    arrNum = loadport_number;
                }

                if (vid == 1001)
                {
                    // wafer_number가 List<int>인지 검사
                    if (wafer_number is IEnumerable<int> waferList)
                    {
                        foreach (var v in waferList)
                        {
                            vidData = this.GetDVID(vid, v);
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
                    else
                    {
                        vidData = this.GetDVID(vid, (int)wafer_number);
                        vidItems.Add(
                            L(
                                U4((uint)rptid),
                                L(
                                    A(vidData?.ToString() ?? "")
                                )
                            )
                        );
                    }
                        continue;
                }

                else if (vid != 1001 && vid < 1000)
                    vidData = this.GetSVID(vid, arrNum);
                else
                    vidData = this.GetDVID(vid, arrNum);

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
                100 => EquipmentStatus,
                101 => RobotStatus,
                102 => LoadportDoorStatus[array_data],
                103 => SWVersion,
                _ => null
            };
        }

        public object? GetDVID(int svid, int array_data)
        {
            return svid switch
            {
                1001 => WaferTemp[array_data],
                1002 => LoadportWaferCount[array_data],
                1003 => RecipeData,
                1004 => LotId[array_data],
                1005 => WaferId[array_data],
                1006 => LastAlarmCode,
                1007 => WaferPosition[array_data],
                1008 => PJID[array_data],
                1009 => CJID[array_data],
                1010 => CarrierID[array_data],

                _ => null
            };
        }

        public void SetSVID(int svid, object data, int array_data = 0)
        {
            switch (svid)
            {
                case 100:
                    EquipmentStatus = data.ToString();
                    break;

                case 101:
                    RobotStatus = data.ToString();
                    break;

                case 102:
                    LoadportDoorStatus[array_data] = data.ToString();
                    break;

                case 103:
                    SWVersion = data.ToString();
                    break;
            }
        }

        public void SetDVID(int svid, object data, int array_data = 0)
        {
            switch (svid)
            {
                case 1001:
                    WaferTemp[array_data] = Convert.ToInt32(data);
                    break;

                case 1002:
                    LoadportWaferCount[array_data] = Convert.ToInt32(data);
                    break;

                case 1003:
                    RecipeData = data.ToString();
                    break;

                case 1004:
                    LotId[array_data] = data.ToString();
                    break;

                case 1005:
                    WaferId[array_data] = data.ToString();
                    break;

                case 1006:
                    LastAlarmCode = data.ToString();
                    break;

                case 1007:
                    WaferPosition[array_data] = data.ToString();
                    break;

                case 1008:
                    PJID[array_data] = data.ToString();
                    break;

                case 1009:
                    CJID[array_data] = data.ToString();
                    break;

                case 1010:
                    CarrierID[array_data] = data.ToString();
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
