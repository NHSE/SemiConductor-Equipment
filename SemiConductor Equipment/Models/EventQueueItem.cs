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
        #region FIELDS
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private CEIDInfo cEID;
        [ObservableProperty]
        private List<Item> vIDItems;
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }
}
