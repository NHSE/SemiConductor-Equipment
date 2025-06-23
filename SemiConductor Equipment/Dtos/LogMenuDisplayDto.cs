using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Dtos
{
    public class ChamberlogDisplayDto
    {
        public string? ChamberName { get; set; }

        public DateTime? Time { get; set; }

        public short? Slot { get; set; }

        public string? WaferId { get; set; }

        public string? LotId { get; set; }

        public string? State { get; set; }

        public string? Logdata { get; set; }
    }
}
