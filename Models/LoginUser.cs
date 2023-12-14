using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class LoginUser
    {
        [Required]
        [MaxLength(16)]
        public string Username { get; set; }
        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
    }
}
