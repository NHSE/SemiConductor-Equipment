using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public enum WaferLocationType
    {
        LoadPort,
        RobotArm,
        ProcessChamber,
        Buffer
    }

    public class WaferLocation
    {
        public int WaferId { get; set; }
        public WaferLocationType Location { get; set; }
        public string TargetId { get; set; } // e.g., "CH1", "B2", etc.
    }
}
