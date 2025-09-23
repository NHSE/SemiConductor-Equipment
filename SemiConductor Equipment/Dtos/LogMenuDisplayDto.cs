using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Dtos
{
    public class ChamberlogDisplayDto
    {
        #region PROPERTIES
        public string? ChamberName { get; set; }

        public DateTime? Time { get; set; }

        public short? Slot { get; set; }

        public string? WaferId { get; set; }

        public string? LotId { get; set; }

        public string? State { get; set; }

        public string? Logdata { get; set; }
        #endregion
    }

    public class AlarmlogDisplayDto
    {
        #region PROPERTIES
        public int? Alarm_Number { get; set; }
        public string? Alarm_Time { get; set; }
        public string? Alarm_Message { get; set; }
        #endregion
    }
}
