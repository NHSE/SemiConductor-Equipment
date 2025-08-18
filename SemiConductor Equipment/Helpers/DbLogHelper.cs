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
            string Alarm_Time = DateTime.Now.ToString(); // alarm num는 serial4 형식으로 바꾸고 시퀀스로 자동 증가하도록 설정해야함
            var log = new Alarmlogtable
            {
                AlarmTime = Alarm_Time,
                AlarmMessage = Alarm_Msg
            };
            _database.Create(log);
        }
    }
}
