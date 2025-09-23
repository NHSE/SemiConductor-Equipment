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
    public class AlarmMessageService : IAlarmMsgManager
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
        /// <summary>
        /// 커스텀으로 제작한 메세지 박스를 보여주기 위한 서비스 레이어
        /// </summary>
        /// <param name="dBLogManager"></param>
        /// <param name="logManager"></param>
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
        /// <summary>
        /// 알람에 대한 내용을 이벤트로 전달
        /// </summary>
        /// <param name="alarmmsg"></param>
        public void AlarmMessage_IN(string alarmmsg)
        {
            AlarmData?.Invoke(this, alarmmsg);
            this._dblogManager.WriteDbLog(alarmmsg);
            this._logManager.WriteLog("Alarm", $"State", alarmmsg);
            IsAlarm = true;
        }

        /// <summary>
        /// 알람 내용 초기화
        /// </summary>
        public void AlarmMessage_OUT()
        {
            AlarmData?.Invoke(this, string.Empty);
            IsAlarm = false;
        }
        #endregion
    }
}
