using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IWaferProcessCoordinator
    {
        Task StartProcessAsync(Queue<Wafer> waferQueue, CancellationToken token);
        event EventHandler<string> Process;
    }
}
