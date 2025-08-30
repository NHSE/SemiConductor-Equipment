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
    public class DbLogHelper : IDBLogManager
    {
        private readonly IDatabase<Alarmlogtable> _database;
        private readonly ILogManager _logManager;

        public DbLogHelper(IDatabase<Alarmlogtable> database, ILogManager logManager)
        {
            _database = database;
            _logManager = logManager;
        }

        public void WriteDbLog(string Alarm_Msg)
        {
            try
            {
                if (_database == null)
                {
                    _logManager.WriteLog("Error", $"State", "Database instance is null. Log not written.");
                    return;
                }

                string Alarm_Time = DateTime.Now.ToString();
                var log = new Alarmlogtable
                {
                    AlarmTime = Alarm_Time,
                    AlarmMessage = Alarm_Msg
                };

                _database.Create(log); // DB 저장 시도
            }
            catch (Exception ex)
            {
                // DB 연결 실패 등 예외 발생 시 빠져나오기
                _logManager.WriteLog("Error", $"State", ex.Message);
            }
        }
    }
}
