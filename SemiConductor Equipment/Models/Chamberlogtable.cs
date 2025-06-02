using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models;

public partial class Chamberlogtable
{
    public string? ChamberName { get; set; }

    public DateTime? Time { get; set; }

    public string? Logdata { get; set; }
}
