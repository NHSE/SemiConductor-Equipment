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
        #region FIELDS
        private readonly LogDatabaseContext? _logDatabaseContext;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public LogtableService(LogDatabaseContext? logDatabaseContext)
        {
            this._logDatabaseContext = logDatabaseContext;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
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
        #endregion

    }
}
