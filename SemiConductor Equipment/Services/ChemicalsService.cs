using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class ChemicalsService : IChemicalManager
    {
        #region FIELDS
        private readonly string _configDirectory;
        public event Action ConfigRead;
        #endregion

        #region PROPERTIES
        public Dictionary<string, int> Chemical { get; set; } = new Dictionary<string, int>()
        {
            ["Chamber1"] = 0,
            ["Chamber2"] = 0,
            ["Chamber3"] = 0,
            ["Chamber4"] = 0,
            ["Chamber5"] = 0,
            ["Chamber6"] = 0,
        };
        #endregion

        #region CONSTRUCTOR
        public ChemicalsService(string configDirectory)
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
                if (line.StartsWith("Chamber1"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber1"] = chemical;
                }
                else if (line.StartsWith("Chamber2"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber2"] = chemical;
                }
                else if (line.StartsWith("Chamber3"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber3"] = chemical;
                }
                else if (line.StartsWith("Chamber4"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber4"] = chemical;
                }
                else if (line.StartsWith("Chamber5"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber5"] = chemical;
                }
                else if (line.StartsWith("Chamber6"))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int chemical))
                        Chemical["Chamber6"] = chemical;
                }
            }

            ConfigRead?.Invoke();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public bool ConsumeChemical(string chambername, int Value)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            // 파일의 모든 줄을 읽어옵니다.
            var lines = File.ReadAllLines(filePath);

            int newValue = this.Chemical[chambername] - Value;
            if (newValue <= 0)
            {
                newValue = 0;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                // 각 줄이 해당 key로 시작하는지 확인
                if (lines[i].StartsWith(chambername))
                {
                    // 구분자(: 또는 =)에 따라 새로운 값으로 줄을 만듭니다.
                    if (lines[i].Contains("="))
                    {
                        lines[i] = $"{chambername} = {newValue}";
                        break;
                    }
                }
            }

            // 수정된 내용을 파일에 다시 씁니다.
            File.WriteAllLines(filePath, lines);


            InitConfig();
            if (newValue == 0)
                return false;

            return true;
        }


        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void ModifyChemicalValue(string chambername, int Value)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            // 파일의 모든 줄을 읽어옵니다.
            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                // 각 줄이 해당 key로 시작하는지 확인
                if (lines[i].StartsWith(chambername))
                {
                    // 구분자(: 또는 =)에 따라 새로운 값으로 줄을 만듭니다.
                    if (lines[i].Contains("="))
                    {
                        lines[i] = $"{chambername} = {Value}";
                        break;
                    }
                }
            }

            // 수정된 내용을 파일에 다시 씁니다.
            File.WriteAllLines(filePath, lines);


            InitConfig();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public string GetFilePathAndCreateIfNotExists()
        {
            string fileName = $".Chemical.config";
            string filePath = Path.Combine(_configDirectory, fileName);

            // 파일이 없으면 생성하면서 내용도 쓴다
            if (!File.Exists(filePath))
            {
                string content = "Chamber1 = 100\nChamber2 = 100\nChamber3 = 100\nChamber4 = 100\nChamber5 = 100\nChamber6 = 100\n";
                File.WriteAllText(filePath, content);
            }

            return filePath;
        }

        public int GetValue(string chambername)
        {
            return this.Chemical[chambername];
        }
        #endregion
    }
}
