using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Database
{
	public class UserContext : DbContext
	{
		public UserContext(DbContextOptions<UserContext> dbContextOptions)
		: base(dbContextOptions)
		{
		}
		public DbSet<User>? Users { get; set; }
        public DbSet<AlarmVerlauf> AlarmVerlaufs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("PRIMARY");

				entity.ToTable("users");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.Username).HasMaxLength(16).HasColumnName("username");
				entity.Property(e => e.Role).HasMaxLength(5).HasColumnName("role");
				entity.Property(e => e.Password).HasMaxLength(128).HasColumnName("password");
                entity.Property(e => e.UID).HasMaxLength(24).HasColumnName("UID");
            });
		}
	}
}
