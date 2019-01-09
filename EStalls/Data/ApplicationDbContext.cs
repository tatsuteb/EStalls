using System;
using System.Collections.Generic;
using System.Text;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EStalls.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Item> Item { get; set; }
        public DbSet<ItemDlInfo> ItemDlInfo { get; set; }

        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<PurchasedItem> PurchasedItem { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CartItem>()
                .HasKey(c => new {c.CartId, c.ItemId});
            builder.Entity<OrderItem>()
                .HasKey(o => new {o.OrderId, o.ItemId});
            builder.Entity<PurchasedItem>()
                .HasKey(p => new {p.UserId, p.ItemId});
        }
    }
}
