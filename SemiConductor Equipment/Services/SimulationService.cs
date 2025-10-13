using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class SimulationService : ISimulationManager
    {
        #region FIELDS
        private readonly string _configDirectory;
        public event Action ConfigRead;
        #endregion

        #region PROPERTIES
        public bool State { get; set; }
        #endregion

        #region CONSTRUCTOR
        public SimulationService(string configDirectory)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);
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
                if (line.StartsWith("Simulation Mode"))
                {
                    string Button = line.Split('=')[1].Trim();

                    this.State = Button switch
                    {
                        "True" => true,
                        "False" => false,
                        _ => false
                    };
                }
            }

            ConfigRead?.Invoke();
        }

        /// <summary>
        /// 업데이트 메서드
        /// </summary>
        public void UpdateConfigValue(bool newValue)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            string Value = newValue ? "True" : "False";
            // 파일의 모든 줄을 읽어옵니다.
            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                // 각 줄이 해당 key로 시작하는지 확인 (예: "IP", "Port", "Device ID")
                if (lines[i].StartsWith("Simulation Mode"))
                {
                    // 구분자(: 또는 =)에 따라 새로운 값으로 줄을 만듭니다.
                    if (lines[i].Contains("="))
                        lines[i] = $"Simulation Mode = {Value}";
                }
            }

            // 수정된 내용을 파일에 다시 씁니다.
            File.WriteAllLines(filePath, lines);

            InitConfig();
        }

        private string GetFilePathAndCreateIfNotExists()
        {
            string fileName = $"Simulation.config";
            string filePath = Path.Combine(_configDirectory, fileName);

            // 파일이 없으면 생성하면서 내용도 쓴다
            if (!File.Exists(filePath))
            {
                string content = "Simulation Mode = False";
                File.WriteAllText(filePath, content);
            }

            return filePath;
        }
        #endregion
    }
}
