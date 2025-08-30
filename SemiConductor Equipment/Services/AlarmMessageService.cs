using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using static System.Object;

namespace SemiConductor_Equipment.Services
{
    public partial class AlarmMessageService : IAlarmMsgManager
    {
        #region FIELDS
        public event EventHandler<string> AlarmData;
        private readonly IDBLogManager _dblogManager;
        private readonly ILogManager _logManager;
        public bool IsAlarm { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public AlarmMessageService(IDBLogManager dBLogManager, ILogManager logManager) 
        {
            this._dblogManager = dBLogManager;
            this._logManager = logManager;
            this.IsAlarm = false;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public void AlarmMessage_IN(string alarmmsg)
        {
            AlarmData?.Invoke(this, alarmmsg);
            this._dblogManager.WriteDbLog(alarmmsg);
            this._logManager.WriteLog("Alarm", $"State", alarmmsg);
            IsAlarm = true;
        }

        public void AlarmMessage_OUT()
        {
            AlarmData?.Invoke(this, string.Empty);
            IsAlarm = false;
        }
        #endregion
    }
}
