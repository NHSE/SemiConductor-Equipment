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
    public interface IBufferManager
    {
        IDictionary<string, string> Buffer_State { get; set; }

        event EventHandler<BufferStatus> DataEnqueued;

        string? FindEmptySlot();

        (string BufferName, Wafer Wafer)? PeekCompletedWafer();

        Task StartProcessingAsync(string buffername, Wafer wafer);

        (string Buffername, Wafer Wafer)? FindCompletedWafer();

        void RemoveWaferFromBuffer(string buffername);

        bool IsAllBufferEmpty();
    }
}
