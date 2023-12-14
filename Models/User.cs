using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class User
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
        [Required]
        [MaxLength(16)]
        public string Username { get; set; }
        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
        [Required]
        [MaxLength(5)]
        public string Role { get; set; }
        [MaxLength(24)]
        public string? UID { get; set; }
    }
}
