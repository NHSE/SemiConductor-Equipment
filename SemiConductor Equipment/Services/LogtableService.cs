using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class LogtableService : IDatabase<Alarmlogtable>
    {
        private readonly LogDatabaseContext? _logDatabaseContext;

        public LogtableService(LogDatabaseContext? logDatabaseContext)
        {
            this._logDatabaseContext = logDatabaseContext;
        }
        public void Create(Alarmlogtable entity)
        {
            this._logDatabaseContext.Alarmlogtables.Add(entity);
            this._logDatabaseContext.SaveChanges();
        }

        public void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public List<Alarmlogtable>? Get()
        {
            return this._logDatabaseContext?.Alarmlogtables.ToList();
        }

        public List<Alarmlogtable> Search(string? chamberName, DateTime? logTime = null)
        {
            /*
            var query = _logDatabaseContext?.Alarmlogtables.AsQueryable();

            if(chamberName == "ALL")
                return query?.ToList() ?? new List<Alarmlogtable>();

            if (!string.IsNullOrWhiteSpace(chamberName))
                query = query.Where(c => c. == chamberName);

            if (logTime.HasValue)
                query = query.Where(c => c.Time == logTime.Value);

            return query?.ToList() ?? new List<Alarmlogtable>();
            */
            List<Alarmlogtable> a = new List<Alarmlogtable>();
            return a;
        }

        public void Update(Alarmlogtable entity)
        {
            throw new NotImplementedException();
        }

        List<string>? IDatabase<Alarmlogtable>.SearchChamberField(string chamberFieldName)
        {
            throw new NotImplementedException();
        }
    }
}
