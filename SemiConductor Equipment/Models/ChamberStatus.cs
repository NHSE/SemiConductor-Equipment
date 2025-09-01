using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models
{ 
    public partial class ChamberStatus : EventArgs
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public string ChamberName { get; }
        public string State { get; }
        public int WaferName { get; }
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public ChamberStatus(string value1, string value2, int value3)
        {
            ChamberName = value1;
            State = value2;
            WaferName = value3;
        }
        #endregion

    }

    public partial class ChamberRPMValue
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public string ChamberName { get; }
        public double RPM { get; }
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public ChamberRPMValue(string value1, double value2)
        {
            ChamberName = value1;
            RPM = value2;
        }
        #endregion

    }
}
