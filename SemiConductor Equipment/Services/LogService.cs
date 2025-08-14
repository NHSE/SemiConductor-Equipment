using System;
using System.Collections.Generic;
using System.IO;
using Secs4Net;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class LogService : ILogManager
    {
        // 로그별로 이벤트 제공 (예시: 사전으로 관리)
        private readonly Dictionary<string, Action<string>?> _logUpdatedEvents = new();
        private readonly string _logDirectory;

        public string LogDataTime { get; set; }

        public LogService(string logDirectory)
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        /// <summary>
        /// 로그 기록 (날짜별 파일 자동 생성)
        /// </summary>
        public void WriteLog(string logType, string messagetype, string message)
        {
            string filePath;
            if (logType.Contains("Dry_Chamber"))
            {
                filePath = GetDryLogFilePath(logType);
            }
            else if (logType.Contains("Clean_Chamber"))
            {
                filePath = GetCleanLogFilePath(logType);
            }
            else if(logType.Contains("Event"))
            {
                filePath = GetEventLogLogPath(logType);
            }
            else
            {
                filePath = GetSecsGemLogPath(logType);
            }
            string logLine = $"{DateTime.Now:HH:mm:ss} {messagetype} ▶ {message}";
            File.AppendAllText(filePath, logLine + Environment.NewLine);

            // 파일 전체 내용을 읽어서 이벤트 발행 (실시간 뷰 갱신용)
            string newContent = File.ReadAllText(filePath);
            if (_logUpdatedEvents.ContainsKey(logType))
                _logUpdatedEvents[logType]?.Invoke(newContent);
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
        /// 날짜별 로그 파일 경로 반환 (예: Chamber1_20240605.log)
        /// </summary>
        public string GetDryLogFilePath(string logType)
        {
            string fileName = $"{logType}_{LogDataTime}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        /// <summary>
        /// 날짜별 로그 파일 경로 반환 (예: Chamber1_20240605.log)
        /// </summary>
        public string GetCleanLogFilePath(string logType)
        {
            string fileName = $"{logType}_{LogDataTime}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        /// <summary>
        /// 날짜별 로그 파일 경로 반환 (예: Chamber1_20240605.log)
        /// </summary>
        public string GetSecsGemLogPath(string logType)
        {
            string fileName = $"{logType}_{DateTime.Now:yyyyMMdd}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        public string GetEventLogLogPath(string logType)
        {
            string fileName = $"{logType}_{DateTime.Now:yyyyMMdd}.log";
            return Path.Combine(_logDirectory, fileName);
        }
    }
}
