using Microsoft.EntityFrameworkCore;
using Server.Models;
using WebPush;

namespace Server.Database
{
    public class PushSubscriptionContext : DbContext
    {
        public PushSubscriptionContext()
        {

        }
        public PushSubscriptionContext(DbContextOptions<PushSubscriptionContext> options) : base(options)
        {

        }
        public virtual DbSet<PushSubscriptionEntity> PushSubscriptionEntities { get; set; }

    }
}
