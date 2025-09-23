using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Secs4Net;
using SemiConductor_Equipment.Services;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface ISecsGemServer
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void Start();
        void Stop();
        bool Initialize(Action<string> logger, IMessageManager messageHandler, IConfigManager configManager);
        #endregion

        #region EVENTS
        event EventHandler Connected;
        event EventHandler Disconnected;
        #endregion
    }
}
