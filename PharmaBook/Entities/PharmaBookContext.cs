using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PharmaBook.Entities
{
    public class PharmaBookContext :IdentityDbContext<User>
    {
        public PharmaBookContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Vendor> vendors { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<PurchasedHistory> purchasedHistory { get; set; }

        public DbSet<MasterInvoice> InvMaster { get; set; }
        public DbSet<ChildInvoice> InvChild { get; set; }

        public DbSet<MasterPO> MasterPO { get; set; }
        public DbSet<ChildPO> ChildPO { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Notes> Notes { get; set; }
    }
}

