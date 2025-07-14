using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    class EquipmentSettingService : IEquipmentConfigManager
    {
        #region FIELDS
        private readonly string _configDirectory;
        public event Action ConfigRead;
        #endregion

        #region PROPERTIES
        public int Max_Temp { get; set; }
        public int Min_Temp { get; set; }
        public int Allow { get; set; }
        public int Chamber_Time { get; set; }
        #endregion

        #region CONSTRUCTOR
        public EquipmentSettingService(string configDirectory)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);

            InitConfig();
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        /// <summary>
        /// 파일 파싱
        /// </summary>
        public void InitConfig()
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // 각 줄에서 '=' 또는 ':' 기준으로 키와 값을 분리
                if (line.StartsWith("Max Temperature"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int tempValue))
                        Max_Temp = tempValue;
                }
                else if (line.StartsWith("Min Temperature"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int tempValue))
                        Min_Temp = tempValue;
                }
                else if (line.StartsWith("Allowable"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int allowValue))
                        Allow = allowValue;
                }
                else if (line.StartsWith("Chamber Time"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chamberTime))
                        Chamber_Time = chamberTime;
                }
            }

            ConfigRead?.Invoke();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void UpdateConfigValue(string key, int newValue)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            // 파일의 모든 줄을 읽어옵니다.
            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                // 각 줄이 해당 key로 시작하는지 확인
                if (lines[i].StartsWith(key))
                {
                    // 구분자(: 또는 =)에 따라 새로운 값으로 줄을 만듭니다.
                    if (lines[i].Contains("="))
                        lines[i] = $"{key} = {newValue}";
                    else if (lines[i].Contains(":"))
                        lines[i] = $"{key} : {newValue}";
                }
            }

            // 수정된 내용을 파일에 다시 씁니다.
            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public string GetFilePathAndCreateIfNotExists()
        {
            string fileName = $"Equipment.config";
            string filePath = Path.Combine(_configDirectory, fileName);

            // 파일이 없으면 생성하면서 내용도 쓴다
            if (!File.Exists(filePath))
            {
                string content = "Min Temperature = 30\nMax Temperature = 120\nAllow = 5\nChamber Time = 10";
                File.WriteAllText(filePath, content);
            }

            return filePath;
        }
        #endregion
    }
}
