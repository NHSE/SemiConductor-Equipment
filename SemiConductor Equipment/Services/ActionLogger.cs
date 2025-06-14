using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Secs4Net;

namespace SemiConductor_Equipment.Services
{
    public class ActionLogger : ISecsGemLogger
    {
        private readonly Action<string> _logAction;

        public ActionLogger(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public void Log(LogLevel level, string message)
        {
            _logAction?.Invoke($"[{level}] {message}");
        }
    }

}
