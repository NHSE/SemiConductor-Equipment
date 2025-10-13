using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SemiConductor_Equipment.Enums.PIOSignalEnum;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IOHTManager
    {
        #region PROPERTIES
        bool _isRunning { get; set; }
        bool _isWafer {  get; set; }

        bool _State { get; set; }
        #endregion

        #region METHODS
        void Initalize();

        void Start();

        Task Server_Start(CancellationToken token);

        void Stop();
        #endregion

        #region EVENTS
        event EventHandler<OHTCarrierInfo> Insert_Wafer;
        event Action<int> Remove_Wafer;
        event Action Server_Connect;
        #endregion
    }
}
