using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public class WaferProcessSequence
    {
        public int WaferId { get; set; }
        public string CurrentPosition { get; set; }
        public Queue<string> Route { get; } = new();

        public WaferProcessSequence(int id)
        {
            WaferId = id;
            CurrentPosition = "LoadPort";
            Route.Enqueue("Robot");
            Route.Enqueue("Chamber");
            Route.Enqueue("Robot");
            Route.Enqueue("Buffer");
            Route.Enqueue("Robot");
            Route.Enqueue("UnloadPort");
        }

        public string MoveNext()
        {
            if (Route.Count == 0) return "Done";
            CurrentPosition = Route.Dequeue();
            return CurrentPosition;
        }

        public bool IsFinished => Route.Count == 0;
    }
}
