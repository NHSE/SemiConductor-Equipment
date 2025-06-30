using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Secs4Net;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.interfaces
{
    public partial interface ISecsGemServer
    {
        void Start();
        void Stop();
        event EventHandler Connected;
        event EventHandler Disconnected;

        bool Initialize(Action<string> logger,
                   MessageHandlerService messageHandler,
                   IConfigManager configManager);
    }
}
