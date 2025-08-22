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
        Dictionary<LoadPortWaferKey, ResultData> _reultCleanData = new();
        Dictionary<LoadPortWaferKey, ResultData> _reultDryData = new();
        private readonly string _logDirectory;
        #endregion

        #region PROPERTIES
        public string ProcessTime { get; set; }  // S14F9 받은 시점
        #endregion

        #region CONSTRUCTOR
        public ResultFileService(string logDirectory)
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public void ClearData()
        {
            _reultCleanData.Clear();
            _reultDryData.Clear();
        }

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

        public void SaveFile(bool isClean)
        {
            string fileName;
            if (isClean) fileName = ProcessTime + "_Clean_Result.csv";
            else fileName = ProcessTime + "_Dry_Result.csv";

            string folderPath = Path.Combine(_logDirectory, ProcessTime);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            SaveDictionaryToCsv(filePath, isClean);
        }

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
                        result.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        result.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        result.ProcessDuration.ToString(@"hh\:mm\:ss"),
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
                        result.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        result.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        result.ProcessDuration.ToString(@"hh\:mm\:ss"),
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

        public void SetProcessTime(string time)
        {
            this.ProcessTime = time;
        }
        #endregion
    }
}
