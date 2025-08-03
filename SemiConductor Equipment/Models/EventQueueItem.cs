using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Models
{
    public partial class EventQueueItem : ObservableObject
    {
        [ObservableProperty]
        private CEIDInfo cEID;
        [ObservableProperty]
        private List<Item> vIDItems;
    }
}
