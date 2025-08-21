using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models
{ 
    public partial class ChamberStatus : EventArgs
    {
        public string ChamberName { get; }
        public string State { get; }
        public int WaferName { get; }

        public ChamberStatus(string value1, string value2, int value3)
        {
            ChamberName = value1;
            State = value2;
            WaferName = value3;
        }
    }

    public partial class ChamberRPMValue
    {
        public string ChamberName { get; }
        public double RPM { get; }

        public ChamberRPMValue(string value1, double value2)
        {
            ChamberName = value1;
            RPM = value2;
        }
    }
}
