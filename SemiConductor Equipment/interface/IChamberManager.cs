using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Models;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.interfaces
{
    public interface IChamberManager
    {
        bool IsAllChamberEmpty();
        string? FindEmptyChamber();

        (string ChamberName, Wafer Wafer)? PeekCompletedWafer();
        void RemoveWaferFromChamber(string chamberName);
        void AddWaferToChamber(string chamberName, Wafer wafer);
        Task StartProcessingAsync(string chamberName, Wafer wafer);
        void ProcessStart();

        IDictionary<string, string> Chamber_State { get; set; }

        event EventHandler<ChamberStatus> DataEnqueued;
        event EventHandler<RobotCommand> Enque_Robot;
        event EventHandler<Wafer> ChangeTempData;
        event Action ProcessHandled;
    }
}
