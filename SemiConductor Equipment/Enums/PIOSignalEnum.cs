using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Enums
{
    public class PIOSignalEnum
    {

        public enum PioSignal : byte
        {
            CS = 0b0000_0001,
            VALID = 0b0000_0010,
            REQ = 0b0000_0011,
            TR_REQ = 0b0000_0100,
            READY = 0b0000_0101,
            BUSY = 0b0000_0111,
            COMPT = 0b0000_1000
        }

        public enum TransferAction : byte
        {
            Load = 0b0000_0001,
            UnLoad = 0b0000_0010,
            Connect = 0b0000_0011
        }

    }
}
