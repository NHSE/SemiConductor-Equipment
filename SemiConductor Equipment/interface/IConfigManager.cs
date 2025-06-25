using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface IConfigManager
    {
        public void InitConfig();

        public void UpdateConfigValue(string key, string newValue);

        public string GetFilePathAndCreateIfNotExists();

        string IP { get; set; }
        int Port { get; set; }
        ushort DeviceID { get; set; }

        public event Action ConfigRead;
    }
}
