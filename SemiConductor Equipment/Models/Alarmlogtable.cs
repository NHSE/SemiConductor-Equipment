using System;
using System.Collections.Generic;

namespace SemiConductor_Equipment.Models;

public partial class Alarmlogtable
{
    public int? AlarmNumber { get; set; }

    public string? AlarmTime { get; set; }

    public string? AlarmMessage { get; set; }
}
