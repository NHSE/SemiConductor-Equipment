using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface ISolutionManager
    {
        Dictionary<string, int> Chemical { get; set; }
        Dictionary<string, int> PreClean { get; set; }
        void InitConfig();
        bool ConsumeChemical(string chambername, int Value);
        bool ConsumePreClean(string chambername, int Value);
        void ModifyChemicalValue(string chambername, int Value);
        void ModifyPreCleanValue(string chambername, int Value);
        string GetFilePathAndCreateIfNotExists();
        int GetValue(string chambername);
        int GetPreCleanValue(string chambername);

        event Action ConfigRead;
    }
}
