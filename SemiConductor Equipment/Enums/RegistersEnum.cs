using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Enums
{
    public class RegistersEnum
    {
        public enum Registers
        {
            #region ENUM
            Registers_CleanChamber1 = 1,
            Registers_CleanChamber2 = 3,
            Registers_CleanChamber3 = 5,
            Registers_CleanChamber4 = 7,
            Registers_CleanChamber5 = 9,
            Registers_CleanChamber6 = 11,

            Registers_DryChamber1 = 13,
            Registers_DryChamber2 = 15,
            Registers_DryChamber3 = 17,
            Registers_DryChamber4 = 19,
            Registers_DryChamber5 = 21,
            Registers_DryChamber6 = 23
            #endregion
        }
    }
}
