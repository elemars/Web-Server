using System;
using System.Collections.Generic;

namespace FenstermonitoringAPI.Models;

public partial class Device
{
    public int Deviceid { get; set; }

    public string Location { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal? Batterylevel { get; set; }

    public string Macadress { get; set; } = null!;
}
