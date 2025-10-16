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
        #region PROPERTIES

        string? RobotStatus { get; set; }
        #endregion

        #region METHODS
        object? GetSVID(int svid);
        object? GetDVID(int svid, int array_data);
        object? GetDVID(int svid, int array_data, int slot_Number);
        void SetSVID(int svid, object data);
        void SetDVID(int svid, object data, int array_data = 0);
        void SetDVID(int svid, object data, int loadport_Number, int slot_Number);
        List<Item>? GetRPTID(int rptid, object wafer_number, int loadport_number);
        bool IsRPTID(uint rptid);
        bool IsVID(uint vid);
        bool IsCEID(uint ceid);
        bool IsRPTIDInCEID(uint ceid, uint rptid);
        #endregion

        #region EVENTS
        #endregion
    }
}
