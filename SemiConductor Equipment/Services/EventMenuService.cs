using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public class EventMenuService : IEventConfigManager
    {
        #region FIELDS
        private readonly string _configDirectory;
        public event Action ConfigRead;
        #endregion

        #region PROPERTIES
        public Dictionary<int, RPTIDInfo> RPTID { get; set; } = new();
        public Dictionary<int, CEIDInfo> CEID { get; set; } = new();
        public Dictionary<int, SVIDInfo> SVID { get; set; } = new();
        #endregion

        #region CONSTRUCTOR
        public EventMenuService(string configDirectory)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);

            InitCEIDConfig();
            InitRPTIDConfig();
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public void InitRPTIDConfig()
        {
            string filePath = GetFilePathAndCreateIfNotExists("RPTID.config");
            string[] lines = File.ReadAllLines(filePath);

            Dictionary<int, RPTIDInfo> rptidDict = new();

            int currentRPTID = -1;
            RPTIDInfo currentRPTIDInfo = null;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                if (trimmed.StartsWith("[RPTID_") && trimmed.EndsWith("]"))
                {
                    // 새 CEID 항목 시작
                    if (currentRPTID != -1 && currentRPTIDInfo != null)
                        rptidDict[currentRPTID] = currentRPTIDInfo;

                    string idStr = trimmed.Substring(7, trimmed.Length - 8); // "CEID_100" -> "100"
                    if (int.TryParse(idStr, out int rptid))
                    {
                        currentRPTID = rptid;
                        currentRPTIDInfo = new RPTIDInfo { CEIDs = new List<int>() };
                        currentRPTIDInfo.Number = rptid;
                    }
                }
                else if (trimmed.StartsWith("State ="))
                {
                    if (currentRPTIDInfo != null && bool.TryParse(trimmed.Substring(7).Trim(), out bool state))
                        currentRPTIDInfo.State = state;
                }
                else if (trimmed.StartsWith("CEID ="))
                {
                    string[] parts = trimmed.Substring(6).Split(',');
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out int svid))
                            currentRPTIDInfo.CEIDs.Add(svid);
                    }
                }
            }

            // 마지막 CEID 저장
            if (currentRPTID != -1 && currentRPTIDInfo != null)
                rptidDict[currentRPTID] = currentRPTIDInfo;

            // 서비스에 할당 (예시)
            this.RPTID = rptidDict;
            ConfigRead?.Invoke();
        }

        public void InitCEIDConfig()
        {
            string filePath = GetFilePathAndCreateIfNotExists("CEID.config");
            string[] lines = File.ReadAllLines(filePath);

            Dictionary<int, CEIDInfo> ceidDict = new ();

            int currentCEID = -1;
            CEIDInfo currentCEIDInfo = null;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                if (trimmed.StartsWith("[CEID_") && trimmed.EndsWith("]"))
                {
                    // 새 CEID 항목 시작
                    if (currentCEID != -1 && currentCEIDInfo != null)
                        ceidDict[currentCEID] = currentCEIDInfo;

                    string idStr = trimmed.Substring(6, trimmed.Length - 7);
                    if (int.TryParse(idStr, out int ceid))
                    {
                        currentCEID = ceid;
                        currentCEIDInfo = new CEIDInfo { SVIDs = new List<int>() };
                        currentCEIDInfo.Number = ceid;
                    }
                }
                else if (trimmed.StartsWith("Name ="))
                {
                    currentCEIDInfo.Name = trimmed.Substring(6).Trim();
                }
                else if (trimmed.StartsWith("State ="))
                {
                    if (currentCEIDInfo != null && bool.TryParse(trimmed.Substring(7).Trim(), out bool state))
                        currentCEIDInfo.State = state;
                }
                else if (trimmed.StartsWith("SVID ="))
                {
                    string[] parts = trimmed.Substring(6).Split(',');
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out int svid))
                            currentCEIDInfo.SVIDs.Add(svid);
                    }
                }
            }

            // 마지막 CEID 저장
            if (currentCEID != -1 && currentCEIDInfo != null)
                ceidDict[currentCEID] = currentCEIDInfo;

            // 서비스에 할당 (예시)
            this.CEID = ceidDict;
            ConfigRead?.Invoke();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void UpdateCEIDSectionPartial(CEIDInfo newData)
        {
            string filePath = GetFilePathAndCreateIfNotExists("CEID.config");
            var lines = File.ReadAllLines(filePath).ToList();

            string targetSection = $"[CEID_{newData.Number}]";
            bool inTargetSection = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                // 섹션 시작 라인 탐색
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inTargetSection = line.Equals(targetSection, StringComparison.OrdinalIgnoreCase);
                    continue; // 섹션 헤더는 그대로 유지
                }

                if (inTargetSection)
                {
                    // 섹션 내 키-값 라인 교체
                    if (line.StartsWith("Name"))
                    {
                        lines[i] = $"Name = {newData.Name}";
                    }
                    else if (line.StartsWith("State"))
                    {
                        lines[i] = $"State = {newData.State}";
                    }
                    else if (line.StartsWith("SVID"))
                    {
                        lines[i] = $"SVID = {newData.SVIDsDisplay}"; // "1, 2, 3" 문자열로 넣기
                    }
                }
            }

            // 수정된 전체 라인을 파일에 다시 씀
            File.WriteAllLines(filePath, lines);
            InitCEIDConfig();
        }


        public string GetFilePathAndCreateIfNotExists(string configname)
        {
            string fileName = configname;
            string filePath = Path.Combine(_configDirectory, fileName);

            // 파일이 없으면 생성하면서 내용도 쓴다
            if (!File.Exists(filePath))
            {
                string content = string.Empty;
                if (fileName.Contains("RPTID"))
                {
                    content = "";
                }
                else if(fileName.Contains("CEID"))
                {
                    content = @"
[CEID_100]
Name = Load Port Load Complete
State = False
SVID = 1, 2, 3

[CEID_101]
Name = Load Port Unload Complete
State = False
SVID = 4, 5

[CEID_200]
Name = Wafer Move Start
State = False
SVID = 6, 7, 8

[CEID_201]
Name = Wafer Move Complete
State = False
SVID = 6, 7, 8

[CEID_300]
Name = Process Start
State = False
SVID = 10, 11, 12

[CEID_301]
Name = Process Complete
State = False
SVID = 10, 11, 12

[CEID_600]
Name = Alarm Occurred
State = False
SVID = 9

[CEID_601]
Name = Alarm Cleared
State = False
SVID = 9
";
                }
                else
                {
                    content = "IP = 127.0.0.1\nPort = 5000\nDevice ID : 1";
                }
                    File.WriteAllText(filePath, content);
            }

            return filePath;
        }
        #endregion
    }
}
