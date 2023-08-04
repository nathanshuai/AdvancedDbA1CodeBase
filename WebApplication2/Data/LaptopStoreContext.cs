using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class LaptopStoreContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your SQL Server connection here
            // optionsBuilder.UseSqlServer("YourConnectionString");
            modelBuilder.Entity<Brand>().HasKey(b => b.Id);

            modelBuilder.Entity<StoreLocation>().HasKey(s => s.StoreNumber);

            modelBuilder.Entity<Laptop>()
                .HasKey(l => l.LaptopId);

            modelBuilder.Entity<Laptop>()
            .Property(l => l.Price)
            .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Laptop>()
                .HasOne(l => l.Brand)
                .WithMany(b => b.Laptops)
                .HasForeignKey(l => l.BrandId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StoreLaptop>().HasKey(sl => new { sl.LaptopId, sl.StoreNumber });
        }


        public LaptopStoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Laptop> Laptops { get; set; } = null!;
        public DbSet<StoreLocation> StoreLocations { get; set; } = null!;
        public DbSet<StoreLaptop> StoreLaptops { get; set; } = null!;

    }
}
