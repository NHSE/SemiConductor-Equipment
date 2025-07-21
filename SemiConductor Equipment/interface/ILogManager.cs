using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface ILogManager
    {
        void WriteLog(string logType, string messagetype, string message);

        void Subscribe(string logType, Action<string> handler);

        string GetLogFilePath(string logType);

        string GetSecsGemLogPath(string logType);

        string LogDataTime { get; set; }
    }
}
