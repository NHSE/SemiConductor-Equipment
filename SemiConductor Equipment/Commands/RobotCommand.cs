using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Commands
{
    public class RobotCommand
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        public RobotCommandType CommandType { get; set; }
        public Wafer Wafer { get; set; } = default!;
        public string Location { get; set; } = string.Empty;
        public string NextLocation { get; set; } = string.Empty;

        public string Completed { get; set; } = string.Empty;
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }
}
