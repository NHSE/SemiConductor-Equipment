using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IPLCManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void Initalize();

        Task<int> Start(string chambername, int targetRpm, int currentRpm, bool bClean);

        Task Stop(string chambername, bool bClean);

        byte Get_CleanRegisters(string chambername);
        byte Get_DryRegisters(string chambername);
        #endregion

        #region EVENTS
        #endregion
    }
}
