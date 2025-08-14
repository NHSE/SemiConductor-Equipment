using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.interfaces
{
    public interface ICleanManager
    {
        IDictionary<string, string> Clean_State { get; set; }
        Dictionary<string, bool> Unable_to_Process { get; set; }

        event EventHandler<CleanChamberStatus> DataEnqueued;
        event EventHandler<RobotCommand> Enque_Robot;
        event EventHandler<CleanChamberStatus> MultiCupChange;
        event EventHandler<ChemicalStatus> ChemicalChange;
        event EventHandler<ChemicalStatus> PreCleanChange;
        event EventHandler<ChamberData> CleanChamberChange;

        string? FindEmptySlot();

        (string ChamberName, Wafer Wafer)? PeekCompletedWafer();

        Task StartProcessingAsync(string chambername, Wafer wafer);

        (string ChamberName, Wafer Wafer)? FindCompletedWafer();

        void RemoveWaferFromCleanChamber(string chambername);
        void AddWaferToBuffer(string chambername, Wafer wafer);

        bool IsAllCleanChamberEmpty();
        bool IsAllDisableChamber();
        bool CleanChamberEmpty(string chambername);
    }
}
