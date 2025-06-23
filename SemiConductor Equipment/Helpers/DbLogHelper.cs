using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Helpers
{
    public class DbLogHelper
    {
        private readonly IDatabase<Chamberlogtable> _database;
        private readonly ILogManager _logManager;

        public DbLogHelper(IDatabase<Chamberlogtable> database, ILogManager logManager)
        {
            _database = database;
            _logManager = logManager;
        }

        public void WriteDbLog(string chamberName, Wafer wafer, string state)
        {
            if (wafer == null) return;

            string num = string.Empty;
            TimeZoneInfo koreaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            var log = new Chamberlogtable
            {
                ChamberName = chamberName,
                Time = TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.UtcNow.UtcDateTime, koreaTimeZone),
                WaferId = wafer.Wafer_Num.ToString(),
                Slot = short.TryParse(wafer.SlotId, out var slotValue) ? slotValue : (short?)null,
                LotId = wafer.LotId,
                State = state,
                Logdata = $"{wafer.CarrierId} : Wafer : {wafer.Wafer_Num} {state} {chamberName}"
            };
            _database.Create(log);
        }
    }
}
