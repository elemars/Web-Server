using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public partial class Device
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Deviceid { get; set; } = default;

    public string Location { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal? Batterylevel { get; set; }

    public string Macadress { get; set; } = null!;

    public int Laststate { get; set; } = 4;
}
