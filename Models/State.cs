using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public partial class State
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Stateid { get; set; } = default;

    public int Statevalue { get; set; }

    public DateTime? Timestamp { get; set; }

    public int Deviceid { get; set; }
}
