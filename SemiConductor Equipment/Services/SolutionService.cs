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
    public class SolutionService : ISolutionManager
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

        public Dictionary<string, int> PreClean { get; set; } = new Dictionary<string, int>()
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
        public SolutionService(string configDirectory)
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

            Chemical = new Dictionary<string, int>();
            PreClean = new Dictionary<string, int>();

            for (int i = 0; i < lines.Length; i += 3)
            {
                string chamberName = lines[i].Trim();

                var chemicalParts = lines[i + 1].Split('=');
                if (chemicalParts.Length == 2 && int.TryParse(chemicalParts[1].Trim(), out int chemical))
                {
                    Chemical[chamberName] = chemical;
                }

                // 3. Pre Clean 값 파싱
                var preCleanParts = lines[i + 2].Split('=');
                if (preCleanParts.Length == 2 && int.TryParse(preCleanParts[1].Trim(), out int preClean))
                {
                    PreClean[chamberName] = preClean;
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

            ModifyChamberValue(chambername, "Chemical", newValue);

            if (newValue == 0)
                return false;

            return true;
        }

        public bool ConsumePreClean(string chambername, int Value)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            // 파일의 모든 줄을 읽어옵니다.
            var lines = File.ReadAllLines(filePath);

            int newValue = this.PreClean[chambername] - Value;
            if (newValue <= 0)
            {
                newValue = 0;
            }

            ModifyChamberValue(chambername, "Pre Clean", newValue);

            if (newValue == 0)
                return false;

            return true;
        }


        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public void ModifyChemicalValue(string chambername, int Value)
        {
            ModifyChamberValue(chambername, "Chemical", Value);
        }

        public void ModifyPreCleanValue(string chambername, int Value)
        {
            ModifyChamberValue(chambername, "Pre Clean", Value);
        }

        public void ModifyChamberValue(string chamberName, string propertyName, int value)
        {
            string filePath = GetFilePathAndCreateIfNotExists();
            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals(chamberName, StringComparison.OrdinalIgnoreCase))
                {
                    // 다음 두 줄: Chemical, Pre Clean 값
                    for (int j = i + 1; j <= i + 2 && j < lines.Length; j++)
                    {
                        if (lines[j].Trim().StartsWith(propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            lines[j] = $"{propertyName} = {value}";
                            break;
                        }
                    }
                    break;
                }
            }

            File.WriteAllLines(filePath, lines);
            InitConfig();
        }

        /// <summary>
        /// Config 파일 경로 반환 없을 경우 생성
        /// </summary>
        public string GetFilePathAndCreateIfNotExists()
        {
            string fileName = $".Solution.config";
            string filePath = Path.Combine(_configDirectory, fileName);

            // 파일이 없으면 생성하면서 내용도 쓴다
            if (!File.Exists(filePath))
            {
                string content = "Chamber1\nChemical = 100\nPre Clean = 100\nChamber2\nChemical = 100\nPre Clean = 100" +
                    "\nChamber3\nChemical = 100\nPre Clean = 100\nChamber4\nChemical = 100\nPre Clean = 100\nChamber5\nChemical = 100\nPre Clean = 100\n" +
                    "Chamber6\nChemical = 100\nPre Clean = 100\n";
                File.WriteAllText(filePath, content);
            }

            return filePath;
        }

        public int GetValue(string chambername)
        {
            return this.Chemical[chambername];
        }

        public int GetPreCleanValue(string chambername)
        {
            return this.PreClean[chambername];
        }
        #endregion
    }
}
