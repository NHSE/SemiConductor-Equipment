using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IChamberManager
    {
        bool IsAllChamberEmpty();
        string? FindEmptyChamber();

        (string ChamberName, Wafer Wafer)? PeekCompletedWafer();
        void RemoveWaferFromChamber(string chamberName);
        Task StartProcessingAsync(string chamberName, Wafer wafer);

        IDictionary<string, string> Chamber_State { get; set; }

        event EventHandler<ChamberStatus> DataEnqueued;
    }
}
