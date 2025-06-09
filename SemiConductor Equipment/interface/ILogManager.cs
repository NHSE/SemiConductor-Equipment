using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface ILogManager
    {
        public void WriteLog(string logType, string messagetype, string message);

        public void Subscribe(string logType, Action<string> handler);

        public string GetLogFilePath(string logType);
    }
}
