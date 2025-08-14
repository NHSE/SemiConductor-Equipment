using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class CleanChamberStatus
    {
        public string ChamberName { get; }
        public string State { get; }

        public CleanChamberStatus(string value1, string value2)
        {
            ChamberName = value1;
            State = value2;
        }
    }

    public partial class ChemicalStatus
    {
        public string ChamberName { get; }
        public int Solution { get; }

        public bool Result { get; set; }

        public ChemicalStatus(string value1, int value2)
        {
            ChamberName = value1;
            Solution = value2;
        }
    }

    public partial class ChamberData
    {
        public string ChamberName { get; }
        public Wafer wafer { get; }

        public ChamberData(string value1, Wafer value2)
        {
            ChamberName = value1;
            wafer = value2;
        }
    }
}
