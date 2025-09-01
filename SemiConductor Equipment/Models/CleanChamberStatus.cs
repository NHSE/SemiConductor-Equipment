using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class CleanChamberStatus
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public string ChamberName { get; }
        public string State { get; }
        #endregion

        #region CONSTRUCTOR
        public CleanChamberStatus(string value1, string value2)
        {
            ChamberName = value1;
            State = value2;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion

    }

    public partial class ChemicalStatus
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public string ChamberName { get; }
        public int Solution { get; }

        public bool Result { get; set; }
        #endregion

        #region CONSTRUCTOR
        public ChemicalStatus(string value1, int value2)
        {
            ChamberName = value1;
            Solution = value2;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion

    }

    public partial class ChamberData
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public string ChamberName { get; }
        public Wafer wafer { get; }
        #endregion

        #region CONSTRUCTOR
        public ChamberData(string value1, Wafer value2)
        {
            ChamberName = value1;
            wafer = value2;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }
}
