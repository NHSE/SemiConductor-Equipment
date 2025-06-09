using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;

namespace SemiConductor_Equipment.interfaces
{
    public interface IChamberService
    {
        void RunChamber(int chamberNo);

        void SetStatus(int chamberNo, int state);

        event Action<string>? ErrorOccurred;
    }
}
