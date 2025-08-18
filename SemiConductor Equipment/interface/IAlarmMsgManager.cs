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
        event EventHandler<string> AlarmData;
        void AlarmMessage_IN(string alarmmsg);
        void AlarmMessage_OUT();

        bool IsAlarm { get; set; }
    }
}
