using System;
using System.Collections.Generic;

namespace FenstermonitoringAPI.Models;

public partial class State
{
    public int Stateid { get; set; }

    public int? Statevalue { get; set; }

    public DateTime? Timestamp { get; set; }

    public int Deviceid { get; set; }
}
