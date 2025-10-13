using System;
using System.Collections.Generic;
using System.IO;
using Secs4Net;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class LogService : ILogManager
    {
        #region FIELDS
        private readonly Dictionary<string, Action<string>?> _logUpdatedEvents = new();
        private readonly string _logDirectory;
        private static readonly object _logLock = new object();
        #endregion

        #region PROPERTIES
        public string LogDataTime { get; set; } // S14F9 받은 시점
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Chamber, Event 등 실행 로그를 저장하는 서비스 레이어
        /// </summary>
        /// <param name="logDirectory"></param>
        public LogService(string logDirectory)
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// 로그 기록 (날짜별 파일 자동 생성)
        /// </summary>
        public void WriteLog(string logType, string messagetype, string message)
        {
            string filePath;
            if (logType.Contains("Dry_Chamber") || logType.Contains("Clean_Chamber"))
            {
                filePath = GetLogFilePath(logType);
            }
            else
            {
                filePath = GetLogPath(logType);
            }
            string logLine = $"{DateTime.Now:HH:mm:ss} {messagetype} ▶ {message}";

            lock (_logLock)
            {
                File.AppendAllText(filePath, logLine + Environment.NewLine);

                // 파일 전체 내용을 읽어서 이벤트 발행 (실시간 뷰 갱신용)
                string newContent = File.ReadAllText(filePath);
                if (_logUpdatedEvents.ContainsKey(logType))
                    _logUpdatedEvents[logType]?.Invoke(newContent);
            }
        }

        /// <summary>
        /// 로그 구독 (실시간 뷰 갱신용)
        /// </summary>
        public void Subscribe(string logType, Action<string> handler)
        {
            if (!_logUpdatedEvents.ContainsKey(logType))
                _logUpdatedEvents[logType] = null;
            _logUpdatedEvents[logType] += handler;
        }

        /// <summary>
        /// 날짜/시간 폴더별 로그 파일 경로 반환
        /// </summary>
        public string GetLogFilePath(string logType)
        {
            string fileName = $"{LogDataTime}\\{logType}_{LogDataTime}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        /// <summary>
        /// 시간별 로그 파일 경로 반환
        /// </summary>
        public string GetLogPath(string logType)
        {
            string fileName = $"{logType}_{DateTime.Now:yyyyMMdd}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        /// <summary>
        /// 현재 시간에 해당하는 폴더 경로 확인 및 생성
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(string time)
        {
            this.LogDataTime = time;
            string processlogDir = Path.Combine(_logDirectory, LogDataTime);
            if (!Directory.Exists(processlogDir))
                Directory.CreateDirectory(processlogDir);
        }
        #endregion


    }
}
