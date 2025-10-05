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
            Registers_CleanChamber1 = 0,
            Registers_CleanChamber2 = 1,
            Registers_CleanChamber3 = 2,
            Registers_CleanChamber4 = 3,
            Registers_CleanChamber5 = 4,
            Registers_CleanChamber6 = 5,

            Registers_DryChamber1 = 6,
            Registers_DryChamber2 = 7,
            Registers_DryChamber3 = 8,
            Registers_DryChamber4 = 9,
            Registers_DryChamber5 = 10,
            Registers_DryChamber6 = 11,

            Registers_Master_Connect = 12,
            Registers_Slave_Connect = 13,
            #endregion
        }
    }
}
