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
        private readonly List<int> vid_list = new List<int> { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
                                                                                    1001, 1002, 1003, 1005, 1007, 1008, 1009, 1010 };
        #endregion

        #region PROPERTIES
        public string? EquipmentStatus { get; set; } = "Ready";
        public int?[] WaferTemp { get; set; } = new int?[26];
        public int?[] LoadportWaferCount { get; set; } = new int?[3] { 0, 0, 0 };
        public string? RecipeData { get; set; } = "To Be Update";
        public string?[] WaferId { get; set; } = new string?[26];
        public string? RobotStatus { get; set; } = "IDLE";
        public string? Loadport1_DoorStatus { get; set; } = "OPEN";
        public string? Loadport2_DoorStatus { get; set; } = "OPEN";
        public string? SWVersion { get; set; } = "20250830";
        public string?[] WaferPosition { get; set; } = new string?[26];

        public string?[] PJID { get; set; } = new string?[3] {"Nothing", "Nothing", "Nothing" };
        public string?[] CJID { get; set; } = new string?[3] { "Nothing", "Nothing", "Nothing" };
        public string?[] CarrierID { get; set; } = new string?[3] { "Nothing", "Nothing", "Nothing" };

        public int? Chamber1_Chemical { get; set; }
        public int? Chamber2_Chemical { get; set; }
        public int? Chamber3_Chemical { get; set; }
        public int? Chamber4_Chemical { get; set; }
        public int? Chamber5_Chemical { get; set; }
        public int? Chamber6_Chemical { get; set; }
        public int? Chamber1_Pre_Clean { get; set; }
        public int? Chamber2_Pre_Clean { get; set; }
        public int? Chamber3_Pre_Clean { get; set; }
        public int? Chamber4_Pre_Clean { get; set; }
        public int? Chamber5_Pre_Clean { get; set; }
        public int? Chamber6_Pre_Clean { get; set; }

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
            int arrNum = -1;
            foreach (var vid in RPTID_data.VIDs)
            {
                object vidData;
                bool solution_flag = false;

                if (vid != 1002 && vid != 102 && vid < 1008 && (vid != 104 && vid != 105))
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

                else if (vid >= 100 && vid <= 116)
                    vidData = this.GetSVID(vid);
             
                else
                    vidData = this.GetDVID(vid, arrNum);

                vidItems.Add(
                                L(
                                    U4((uint)rptid),
                                    U4((uint)vid),
                                    L(
                                        A(vidData?.ToString() ?? "")
                                    )
                                )
                            );
            }
            return vidItems;
        }

        public object? GetSVID(int svid)
        {
            return svid switch
            {
                100 => EquipmentStatus,
                101 => RobotStatus,
                102 => Loadport1_DoorStatus,
                103 => Loadport2_DoorStatus,
                104 => Chamber1_Chemical,
                105 => Chamber2_Chemical,
                106 => Chamber3_Chemical,
                107 => Chamber4_Chemical,
                108 => Chamber5_Chemical,
                109 => Chamber6_Chemical,
                110 => Chamber1_Pre_Clean,
                111 => Chamber2_Pre_Clean,
                112 => Chamber3_Pre_Clean,
                113 => Chamber4_Pre_Clean,
                114 => Chamber5_Pre_Clean,
                115 => Chamber6_Pre_Clean,
                116 => SWVersion,
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
                1005 => WaferId[array_data],
                1007 => WaferPosition[array_data],
                1008 => PJID[array_data],
                1009 => CJID[array_data],
                1010 => CarrierID[array_data],

                _ => null
            };
        }

        public void SetSVID(int svid, object data)
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
                    Loadport1_DoorStatus = data.ToString();
                    break;

                case 103:
                    Loadport2_DoorStatus = data.ToString();
                    break;

                case 104:
                    Chamber1_Chemical = Convert.ToInt32(data);
                    break;

                case 105:
                    Chamber2_Chemical = Convert.ToInt32(data);
                    break;

                case 106:
                    Chamber3_Chemical = Convert.ToInt32(data);
                    break;

                case 107:
                    Chamber4_Chemical = Convert.ToInt32(data);
                    break;

                case 108:
                    Chamber5_Chemical = Convert.ToInt32(data);
                    break;

                case 109:
                    Chamber6_Chemical = Convert.ToInt32(data);
                    break;

                case 110:
                    Chamber1_Pre_Clean = Convert.ToInt32(data);
                    break;

                case 111:
                    Chamber2_Pre_Clean = Convert.ToInt32(data);
                    break;

                case 112:
                    Chamber3_Pre_Clean = Convert.ToInt32(data);
                    break;

                case 113:
                    Chamber4_Pre_Clean = Convert.ToInt32(data);
                    break;

                case 114:
                    Chamber5_Pre_Clean = Convert.ToInt32(data);
                    break;

                case 115:
                    Chamber6_Pre_Clean = Convert.ToInt32(data);
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

                case 1005:
                    WaferId[array_data] = data.ToString();
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

        public bool IsVID(uint vid)
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
