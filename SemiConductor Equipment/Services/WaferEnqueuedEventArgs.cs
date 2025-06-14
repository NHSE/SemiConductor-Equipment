using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Services
{
    public class WaferEnqueuedEventArgs : EventArgs
    {
        public byte LoadPortId { get; }
        public string CarrierId { get; }
        public string WaferId { get; }
        public string SlotId { get; }
        public string LotId { get; }

        public WaferEnqueuedEventArgs(
            byte loadPortId,
            string carrierId
           // string waferId,
            //string slotId,
            //string lotId)
            )
        {
            LoadPortId = loadPortId;
            CarrierId = carrierId;
           // WaferId = waferId;
           // SlotId = slotId;
           // LotId = lotId;
        }
    }
}
