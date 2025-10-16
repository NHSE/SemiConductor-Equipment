using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IPLCManager
    {
        #region PROPERTIES
        bool bNotConnect { get; set; }
        public bool _State { get; set; }
        #endregion

        #region METHODS
        Task Initalize();
        Task Server_End(bool bClick);

        Task<int> PLC_Start(string chambername, int targetRpm, int currentRpm, bool bClean);

        Task<bool> PLC_Stop(string chambername, bool bClean);
        #endregion

        #region EVENTS
        event EventHandler<ChamberRPMValue> ChangeRPMData;
        event Action<bool> Server_Connect;
        #endregion
    }
}
