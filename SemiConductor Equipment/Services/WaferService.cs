using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Services
{
    public class WaferService
    {
        private Queue<Wafer> _waferQueue = new();
        public event Action<Wafer> WaferEnqueued;

        public void Enqueue(Wafer wafer)
        {
            _waferQueue.Enqueue(wafer);
        }

        public Wafer? Dequeue()
        {
            return _waferQueue.Count > 0 ? _waferQueue.Dequeue() : null;
        }

        public void Clear()
        {
            _waferQueue.Clear();
        }
    }

    public partial class Wafer : ObservableObject
    {
        [ObservableProperty]
        private int loadportId;

        [ObservableProperty]
        private int wafer_Num;

        [ObservableProperty]
        private string carrierId = "";

        [ObservableProperty]
        private string slotId = "";

        [ObservableProperty]
        private string lotId = "";

        [ObservableProperty]
        private string pJId = "";

        [ObservableProperty]
        private string cJId = "";
    }
}
