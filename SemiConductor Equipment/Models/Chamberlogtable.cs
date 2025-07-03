using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models;

public partial class Chamberlogtable
{
    public string? ChamberName { get; set; }

    public DateTime? Time { get; set; }

    public short? Slot { get; set; }

    public string? WaferId { get; set; }

    public string? LotId { get; set; }

    public string? State { get; set; }

    public string? Logdata { get; set; }

    public int Id { get; set; }
}
