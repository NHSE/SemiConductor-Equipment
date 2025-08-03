using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.interfaces
{
    public interface IEventMessageManager
    {
        CEIDInfo GetCEID(int ceid_num);
        bool IsCEIDEnabled(int ceid);
        bool IsCEID(uint ceid);
        bool IsRPTIDInCEID(uint ceid, uint rptid);
        bool IsRPTID(uint rptid);
        bool IsVID(uint rptid, uint vid);
        void CreateRPTID(uint rptid, List<uint> vid);
        void LinkCEID(uint ceid, List<uint> rptid);
        void CEIDStateChange(int ceid, bool state);
        void EnqueueEventData(CEIDInfo eventData);
        void SetSecsGem(ISecsGem secsGem);
        void SetConnect(ISecsConnection connection);
        void DisConnect();
        void StartProcessing();
        Task StopProcessing();
        Task ProcessEventQueueAsync(CancellationToken token);
    }
}
