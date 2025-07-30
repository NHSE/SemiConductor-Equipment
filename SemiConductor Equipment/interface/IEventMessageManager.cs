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
        void EnqueueEventData(CEIDInfo eventData);
        void SetSecsGem(ISecsGem secsGem);
        void SetConnect(ISecsConnection connection);
        void DisConnect();
        void StartProcessing();
        Task StopProcessing();
        Task ProcessEventQueueAsync(CancellationToken token);
    }
}
