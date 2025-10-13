using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface IConfigManager
    {
        #region PROPERTIES
        string IP { get; set; }
        int Port { get; set; }
        ushort DeviceID { get; set; }
        #endregion

        #region METHODS
        void InitConfig();

        void UpdateConfigValue(string key, string newValue);

        string GetFilePathAndCreateIfNotExists();
        #endregion

        #region EVENTS
        event Action ConfigRead;
        #endregion
    }
}
