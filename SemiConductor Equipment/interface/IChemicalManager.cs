using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IChemicalManager
    {
        Dictionary<string, int> Chemical { get; set; }
        void InitConfig();
        bool UpdateConfigValue(string chambername, int Value);
        string GetFilePathAndCreateIfNotExists();
        int GetValue(string chambername);
    }
}
