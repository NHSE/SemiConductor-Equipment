using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models;

public partial class ChamberStatus : EventArgs
{
    public string ChamberName { get; }
    public string State { get; }

    public ChamberStatus(string value1, string value2)
    {
        ChamberName = value1;
        State = value2;
    }
}
