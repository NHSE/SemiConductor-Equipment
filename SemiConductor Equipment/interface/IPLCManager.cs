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
        #endregion

        #region METHODS
        void Initalize();

        Task<int> PLC_Start(string chambername, int targetRpm, int currentRpm, bool bClean);

        Task PLC_Stop(string chambername, bool bClean);
        #endregion

        #region EVENTS
        event EventHandler<ChamberRPMValue> ChangeRPMData;
        #endregion
    }
}
