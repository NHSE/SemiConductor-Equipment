using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;

namespace SemiConductor_Equipment.interfaces
{
    public interface IVIDManager
    {
        object? GetSVID(int svid, int array_data);
        object? GetDVID(int svid, int array_data);
        void SetSVID(int svid, object data, int array_data = 0);
        void SetDVID(int svid, object data, int array_data = 0);
        List<Item>? GetRPTID(int rptid, object wafer_number, int loadport_number);
        bool IsRPTID(uint rptid);
        bool IsVID(uint rptid, uint vid);
        bool IsCEID(uint ceid);
        bool IsRPTIDInCEID(uint ceid, uint rptid);

        string? EquipmentStatus { get; set; }
        int?[] WaferTemp { get; set; }
        int?[] LoadportWaferCount { get; set; }
        string? RecipeData { get; set; }
        string?[] LotId { get; set; }
        string?[] WaferId { get; set; }
        string? RobotStatus { get; set; }
        string?[] LoadportDoorStatus { get; set; }
        string? LastAlarmCode { get; set; }
        string? OperationMode { get; set; }
        string? SWVersion { get; set; }
        string?[] WaferPosition { get; set; }
    }
}
