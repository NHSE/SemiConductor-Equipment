using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface IEventConfigManager
    {
        event Action ConfigRead;
        Dictionary<int, RPTIDInfo> RPTID { get; set; }

        Dictionary<int, CEIDInfo> CEID { get; set; }

        Dictionary<int, SVIDInfo> SVID { get; set; }

        void InitCEIDConfig();
        void InitRPTIDConfig();

        void UpdateCEIDSectionPartial(CEIDInfo newData);
        void UpdateRPTIDSectionPartial(RPTIDInfo newData);
        void CreatedRPTIDSectionPartial(RPTIDInfo newData);
        void RemoveRPTIDSectionPartial(RPTIDInfo newData);
        void CEIDStateChange(int ceid, bool state);

    }
}
