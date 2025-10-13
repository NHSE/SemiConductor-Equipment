using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IAlarmMsgManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void AlarmMessage_IN(string alarmmsg);
        void AlarmMessage_OUT();

        bool IsAlarm { get; set; }
        #endregion

        #region EVENTS
        event EventHandler<string> AlarmData;
        #endregion
    }
}
