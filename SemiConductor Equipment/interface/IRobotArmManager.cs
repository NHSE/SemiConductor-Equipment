using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IRobotArmManager
    {
        event EventHandler<Wafer> CommandStarted;
        event EventHandler<Wafer> CommandCompleted;
        event EventHandler<Wafer> WaferMoveInfo;

        void EnqueueCommand_Chamber(RobotCommand command);

        void EnqueueCommand_Buffer(RobotCommand command);

        void EnqueueCommand_RobotArm(RobotCommand command);

        RobotCommand DequeueCommand_Chamber();

        RobotCommand DequeueCommand_Buffer();

        int CommandSize_Chamber();

        int CommandSize_Buffer();

        Task ProcessCommandQueueAsync(CancellationToken token);

        bool IsBusy();

        void StartProcessing(CancellationToken externalToken);

        Task StopProcessing();
    }
}
