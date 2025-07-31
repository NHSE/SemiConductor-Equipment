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
        private readonly IMessageBox messageBoxManager;
        public event Action ConfigRead;
        #endregion

        #region PROPERTIES
        public Dictionary<int, RPTIDInfo> RPTID { get; set; } = new();
        public Dictionary<int, CEIDInfo> CEID { get; set; } = new();
        public Dictionary<int, SVIDInfo> SVID { get; set; } = new();
        #endregion

        #region CONSTRUCTOR
        public EventMenuService(string configDirectory, IMessageBox messageBoxManager)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);

            InitCEIDConfig();
            InitRPTIDConfig();
            this.messageBoxManager = messageBoxManager;
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

                    string idStr = trimmed.Substring(7, trimmed.Length - 8);
                    if (int.TryParse(idStr, out int rptid))
                    {
                        currentRPTID = rptid;
                        currentRPTIDInfo = new RPTIDInfo { VIDs = new List<int>() };
                        currentRPTIDInfo.Number = rptid;
                        currentRPTIDInfo.Number_list.Add(rptid);
                    }
                }
                else if (trimmed.StartsWith("VID ="))
                {
                    string[] parts = trimmed.Substring(6).Split(',');
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out int svid))
                            currentRPTIDInfo.VIDs.Add(svid);
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

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void UpdateRPTIDSectionPartial(RPTIDInfo newData)
        {
            string filePath = GetFilePathAndCreateIfNotExists("RPTID.config");
            var lines = File.ReadAllLines(filePath).ToList();

            string targetSection = $"[RPTID_{newData.Number}]";
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
                    if (line.StartsWith("VID"))
                    {
                        lines[i] = $"VID = {newData.VIDsDisplay}"; // "1, 2, 3" 문자열로 넣기
                    }
                }
            }

            // 수정된 전체 라인을 파일에 다시 씀
            File.WriteAllLines(filePath, lines);
            InitRPTIDConfig();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void RemoveRPTIDSectionPartial(RPTIDInfo newData)
        {
            string filePath = GetFilePathAndCreateIfNotExists("RPTID.config");
            var lines = File.ReadAllLines(filePath).ToList();

            string targetSection = $"[RPTID_{newData.Number}]";
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    if (trimmed.Equals(targetSection, StringComparison.OrdinalIgnoreCase))
                    {
                        startIndex = i;
                        // endIndex는 기본적으로 startIndex + 1부터 시작
                        endIndex = i + 1;

                        // 다음 섹션이 나올 때까지 계속 endIndex 증가
                        for (int j = i + 1; j < lines.Count; j++)
                        {
                            string nextLine = lines[j].Trim();
                            if (nextLine.StartsWith("[") && nextLine.EndsWith("]"))
                            {
                                break;
                            }
                            endIndex = j;
                        }

                        break;
                    }
                }
            }

            if (startIndex != -1 && endIndex != -1)
            {
                // 삭제 범위: startIndex부터 endIndex까지
                lines.RemoveRange(startIndex, endIndex - startIndex + 1);

                // 삭제 후 startIndex에 공백 줄이 있다면 제거
                if (startIndex < lines.Count && string.IsNullOrWhiteSpace(lines[startIndex]))
                {
                    lines.RemoveAt(startIndex);
                }

                File.WriteAllLines(filePath, lines);
                InitRPTIDConfig();
            }
        }


        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void CreatedRPTIDSectionPartial(RPTIDInfo newData)
        {
            string filePath = GetFilePathAndCreateIfNotExists("RPTID.config");
            var lines = File.ReadAllLines(filePath).ToList();

            string targetSection = $"[RPTID_{newData.Number}]";
            bool sectionFound = false;
            bool inTargetSection = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inTargetSection = line.Equals(targetSection, StringComparison.OrdinalIgnoreCase);
                    if (inTargetSection)
                    {
                        sectionFound = true;
                        break;
                    }
                }
            }

            if (!sectionFound)
            {
                lines.Add("");
                lines.Add(targetSection);
                lines.Add($"VID = {newData.VIDsDisplay}");
            }
            else
            {
                //msg 박스 생성
                this.messageBoxManager.Show("예외 발생", "이미 존재하는 RPTID입니다.");

            }

            File.WriteAllLines(filePath, lines);
            InitRPTIDConfig();
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
                        currentCEIDInfo = new CEIDInfo { RPTIDs = new List<int>() };
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
                else if (trimmed.StartsWith("RPTID ="))
                {
                    string[] parts = trimmed.Substring(7).Split(',');
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out int svid))
                            currentCEIDInfo.RPTIDs.Add(svid);
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
                    else if (line.StartsWith("RPTID"))
                    {
                        lines[i] = $"RPTID = {newData.RPTIDsDisplay}"; // "1, 2, 3" 문자열로 넣기
                    }
                }
            }

            // 수정된 전체 라인을 파일에 다시 씀
            File.WriteAllLines(filePath, lines);
            InitCEIDConfig();
        }

        public void CEIDStateChange(int ceid, bool state)
        {
            if(ceid == 0)
            {
                foreach(int i in CEID.Keys)
                {
                    CEID[i].State = state;
                    UpdateCEIDSectionPartial(CEID[i]);
                }
            }
            else
            {
                CEID[ceid].State = state;
                UpdateCEIDSectionPartial(CEID[ceid]);
            }
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
                    content = @"
[RPTID_1]
VID = 1, 3, 1001

[RPTID_2]
VID = 1002, 1003

[RPTID_3]
VID = 7, 8, 1004

[RPTID_4]
VID = 10, 11, 1005

[RPTID_5]
VID = 9
";
                }
                else if(fileName.Contains("CEID"))
                {
                    content = @"
[CEID_100]
Name = Load Port Load Complete
State = False
RPTID = 1

[CEID_101]
Name = Load Port Unload Complete
State = False
RPTID = 2

[CEID_200]
Name = Wafer Move Start
State = False
RPTID = 3

[CEID_201]
Name = Wafer Move Complete
State = False
RPTID = 3

[CEID_300]
Name = Process Start
State = False
RPTID = 4

[CEID_301]
Name = Process Complete
State = False
RPTID = 4

[CEID_600]
Name = Alarm Occurred
State = False
RPTID = 5

[CEID_601]
Name = Alarm Cleared
State = False
RPTID = 5
";
                }
                    File.WriteAllText(filePath, content);
            }

            return filePath;
        }
        #endregion
    }
}
