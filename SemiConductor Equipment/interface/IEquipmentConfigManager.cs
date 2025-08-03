using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface IEquipmentConfigManager
    {
        void InitConfig();

        void UpdateConfigValue(string key, int newValue);
        string GetFilePathAndCreateIfNotExists();

        int Max_Temp { get; set; }
        int Min_Temp { get; set; }
        int Allow { get; set; }

        int Chamber_Time { get; set; }

        event Action ConfigRead;
    }
}
