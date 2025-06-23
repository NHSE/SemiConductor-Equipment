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
    public class LogtableService : IDatabase<Chamberlogtable>
    {
        private readonly LogDatabaseContext? _logDatabaseContext;

        public LogtableService(LogDatabaseContext? logDatabaseContext)
        {
            this._logDatabaseContext = logDatabaseContext;
        }
        public void Create(Chamberlogtable entity)
        {
            this._logDatabaseContext.Chamberlogtables.Add(entity);
            this._logDatabaseContext.SaveChanges();
        }

        public void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public List<Chamberlogtable>? Get()
        {
            return this._logDatabaseContext?.Chamberlogtables.ToList();
        }

        public List<Chamberlogtable> Search(string? chamberName, DateTime? logTime = null)
        {
            var query = _logDatabaseContext?.Chamberlogtables.AsQueryable();

            if(chamberName == "ALL")
                return query?.ToList() ?? new List<Chamberlogtable>();

            if (!string.IsNullOrWhiteSpace(chamberName))
                query = query.Where(c => c.ChamberName == chamberName);

            if (logTime.HasValue)
                query = query.Where(c => c.Time == logTime.Value);

            return query?.ToList() ?? new List<Chamberlogtable>();
        }

        public void Update(Chamberlogtable entity)
        {
            throw new NotImplementedException();
        }

        List<string>? IDatabase<Chamberlogtable>.SearchChamberField(string chamberFieldName)
        {
            throw new NotImplementedException();
        }
    }
}
