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
        /// <summary>
        /// DB 사용을 위한 서비스 레이어
        /// </summary>
        /// <param name="logDatabaseContext"></param>
        public LogtableService(LogDatabaseContext? logDatabaseContext)
        {
            this._logDatabaseContext = logDatabaseContext;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// DB 내 데이터 생성 메서드
        /// </summary>
        /// <param name="entity"></param>
        public void Create(Alarmlogtable entity)
        {
            this._logDatabaseContext.Alarmlogtables.Add(entity);
            this._logDatabaseContext.SaveChanges();
        }

        /// <exception cref="NotImplementedException"></exception>
        public void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DB 데이터 조회 메서드
        /// </summary>
        /// <returns></returns>
        public List<Alarmlogtable>? Get()
        {
            return this._logDatabaseContext?.Alarmlogtables.ToList();
        }

        public List<Alarmlogtable> Search(string? chamberName, DateTime? logTime = null)
        {
            List<Alarmlogtable> a = new List<Alarmlogtable>();
            return a;
        }


        /// <exception cref="NotImplementedException"></exception>
        public void Update(Alarmlogtable entity)
        {
            throw new NotImplementedException();
        }

        /// <exception cref="NotImplementedException"></exception>
        List<string>? IDatabase<Alarmlogtable>.SearchChamberField(string chamberFieldName)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
