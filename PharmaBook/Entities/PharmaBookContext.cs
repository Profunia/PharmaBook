﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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


    }
}
