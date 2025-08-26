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
        object? GetSVID(int svid);
        object? GetDVID(int svid, int array_data);
        void SetSVID(int svid, object data);
        void SetDVID(int svid, object data, int array_data = 0);
        List<Item>? GetRPTID(int rptid, object wafer_number, int loadport_number);
        bool IsRPTID(uint rptid);
        bool IsVID(uint vid);
        bool IsCEID(uint ceid);
        bool IsRPTIDInCEID(uint ceid, uint rptid);

        string? RobotStatus { get; set; }
    }
}
