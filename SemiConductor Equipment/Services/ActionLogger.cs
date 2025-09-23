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
        #region FIELDS
        private readonly Action<string> _logAction;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// 시뮬레이터와 장비간 SECS/GEM 로그 저장 서비스 레이어
        /// </summary>
        /// <param name="logAction"></param>
        public ActionLogger(Action<string> logAction)
        {
            _logAction = logAction;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        public void Log(LogLevel level, string message)
        {
            _logAction?.Invoke($"[{level}] {message}");
        }
        #endregion
    }

}
