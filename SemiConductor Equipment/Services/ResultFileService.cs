using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class ResultFileService : IResultFileManager
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        Dictionary<LoadPortWaferKey, ResultData> _reultCleanData = new();
        Dictionary<LoadPortWaferKey, ResultData> _reultDryData = new();
        private readonly string _logDirectory;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// 공정 결과 파일을 csv로 저장하는 서비스 레이어
        /// </summary>
        /// <param name="logManager"></param>
        public ResultFileService(ILogManager logManager)
        {
            this._logManager = logManager;
            _logDirectory = @"C:\Logs";
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// 이전에 저장된 데이터를 초기화
        /// </summary>
        public void ClearData()
        {
            _reultCleanData.Clear();
            _reultDryData.Clear();
        }

        /// <summary>
        /// Clean, Dry에 따라 결과 데이터 삽입
        /// </summary>
        /// <param name="ChamberType"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void InsertData(string ChamberType, LoadPortWaferKey key, ResultData value)
        {
            if(ChamberType == "Clean")
            {
                _reultCleanData[key] = value;
            }
            else
            {
                _reultDryData[key] = value;
            }
        }

        /// <summary>
        /// Clean, Dry에 따라 csv 결과 파일 저장
        /// </summary>
        /// <param name="isClean"></param>
        public void SaveFile(bool isClean)
        {
            string fileName;
            if (isClean) fileName = this._logManager.LogDataTime + "_Clean_Result.csv";
            else fileName = this._logManager.LogDataTime + "_Dry_Result.csv";

            string folderPath = Path.Combine(_logDirectory, this._logManager.LogDataTime);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            SaveDictionaryToCsv(filePath, isClean);
        }

        /// <summary>
        /// csv 파일 chamber에 따라 column 지정 및 파일 생성
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isClean"></param>
        private void SaveDictionaryToCsv(string filePath, bool isClean)
        {
            var sb = new StringBuilder();

            string[] headers;
            if(isClean)
            {
                headers = new string[] 
                { 
                    "Load Port", 
                    "Slot ID", 
                    "Carrier ID", 
                    "ControlJob ID", 
                    "ProcessJob ID", 
                    "Chamber Name", 
                    "Process Start Time", 
                    "Process End Time",
                    "Process Duration",
                    "Pre-Clean Flow Rate",
                    "Chemical Flow Rate",
                    "RPM",
                    "Yield",
                    "Has Alarm",
                    "ErrorInfo"
                };
            }
            else
            {
                headers = new string[]
                {
                    "Load Port",
                    "Slot ID",
                    "Carrier ID",
                    "ControlJob ID",
                    "ProcessJob ID",
                    "Chamber Name",
                    "Process Start Time",
                    "Process End Time",
                    "Process Duration",
                    "Target Min Temp",
                    "Target Max Temp",
                    "Actual Temp",
                    "RPM",
                    "Yield",
                    "Has Alarm",
                    "ErrorInfo"
                };
            }
            // 헤더 작성
            sb.AppendLine(string.Join(",", headers));

            if (isClean)
            {
                var sortedData = _reultCleanData
                .OrderBy(kvp => kvp.Key.LoadPort)
                .ThenBy(kvp => kvp.Key.WaferId)
                .ToList();

                // 데이터 작성
                foreach (var kvp in sortedData)
                {
                    var result = kvp.Value;

                    string line = string.Join(",", new string[]
                    {
                        result.LoadPort,
                        result.SlotNo.ToString(),
                        result.CarrierID,
                        result.CJID,
                        result.PJID,
                        result.ChamberName,
                        result.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        result.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        result.ProcessDuration?.ToString(@"hh\:mm\:ss") ?? "",
                        result.PreClean_Flow.ToString(),
                        result.Chemical_Flow.ToString(),
                        result.RPM.ToString(),
                        result.Yield ? "Success" : "Error",
                        result.HasAlarm ? "Y" : "N",
                        $"\"{result.ErrorInfo}\""
                    });

                    sb.AppendLine(line);
                }
            }
            else
            {
                var sortedData = _reultCleanData
                .OrderBy(kvp => kvp.Key.LoadPort)
                .ThenBy(kvp => kvp.Key.WaferId)
                .ToList();

                foreach (var kvp in _reultDryData)
                {
                    var result = kvp.Value;

                    string line = string.Join(",", new string[]
                    {
                        result.LoadPort,
                        result.SlotNo.ToString(),
                        result.CarrierID,
                        result.CJID,
                        result.PJID,
                        result.ChamberName,
                        result.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        result.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        result.ProcessDuration?.ToString(@"hh\:mm\:ss") ?? "",
                        result.TargetMinTemperature.ToString(),
                        result.TargetMaxTemperature.ToString(),
                        result.ActualTemperature.ToString(),
                        result.RPM.ToString(),
                        result.Yield ? "Success" : "Error",
                        result.HasAlarm ? "Y" : "N",
                        $"\"{result.ErrorInfo}\""
                    });

                    sb.AppendLine(line);
                }
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
        #endregion
    }
}
