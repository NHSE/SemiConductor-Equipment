using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface ILogManager
    {
        #region PROPERTIES
        string LogDataTime { get; set; }
        #endregion

        #region METHODS
        void WriteLog(string logType, string messagetype, string message);

        void Subscribe(string logType, Action<string> handler);

        string GetLogFilePath(string logType);

        string GetLogPath(string logType);

        void SetTime(string time);
        #endregion

        #region EVENTS
        #endregion
    }
}
