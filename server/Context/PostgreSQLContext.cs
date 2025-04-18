using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Context
{

    public class VendingMachineContext(DbContextOptions<VendingMachineContext> options) : DbContext(options)
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetSchema("public");
            }

            modelBuilder.Entity<Drink>()
                .HasOne(d => d.Brand)
                .WithMany(b => b.Drinks)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Drink>()
                .HasIndex(d => d.BrandId);
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Coin>().HasData(
                new Coin
                {
                    Id = 1,
                    Denomination = 1,
                    Amount = 25,
                    CoinType = "Regular"
                },
                new Coin
                {
                    Id = 2,
                    Denomination = 2,
                    Amount = 10,
                    CoinType = "Regular"
                },
                new Coin
                {
                    Id = 3,
                    Denomination = 5,
                    Amount = 30,
                    CoinType = "Regular"
                },
                new Coin
                {
                    Id = 4,
                    Denomination = 10,
                    Amount = 20,
                    CoinType = "Regular"
                }
            );
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Coca-Cola" },
                new Brand { Id = 2, Name = "Pepsi" },
                new Brand { Id = 3, Name = "Nestlé" },
                new Brand { Id = 4, Name = "Lifeline" },
                new Brand { Id = 5, Name = "Добрый" },
                new Brand { Id = 6, Name = "Lipton" },
                new Brand { Id = 7, Name = "Шишкин лес" }
            );
            modelBuilder.Entity<Drink>().HasData(
                new Drink { Id = 1, Name = "Coca-Cola Classic 333ml", Price = 103, Amount = 5, BrandId = 1, ImageUrl = "colao.png" },
                new Drink { Id = 2, Name = "Coca-Cola Classic 499ml", Price = 131, Amount = 2, BrandId = 1, ImageUrl = "colao.png" },
                new Drink { Id = 3, Name = "Coca-Cola Zero Sugar 333ml", Price = 97, Amount = 3, BrandId = 1, ImageUrl = "colaz.png" },
                new Drink { Id = 5, Name = "Pepsi 499ml", Price = 121, Amount = 3, BrandId = 2, ImageUrl = "pepsi.png" },
                new Drink { Id = 6, Name = "Pepsi Max 499ml", Price = 140, Amount = 4, BrandId = 2, ImageUrl = "pepsim.png" },
                new Drink { Id = 7, Name = "Pepsi Max 333ml", Price = 121, Amount = 2, BrandId = 2, ImageUrl = "pepsim.png" },
                new Drink { Id = 8, Name = "Pepsi 333ml", Price = 113, Amount = 7, BrandId = 2, ImageUrl = "pepsi.png" },
                new Drink { Id = 9, Name = "Nestea Green Tea 333ml", Price = 131, Amount = 0, BrandId = 3, ImageUrl = "nesteag.png" },
                new Drink { Id = 10, Name = "Nestea Lemon 333ml", Price = 121, Amount = 1, BrandId = 3, ImageUrl = "nesteal.png" },
                new Drink { Id = 11, Name = "Nestea Peach 333ml", Price = 124, Amount = 0, BrandId = 3, ImageUrl = "nesteap.png" },
                new Drink { Id = 12, Name = "Nestea Raspberry 333ml", Price = 146, Amount = 13, BrandId = 3, ImageUrl = "nestear.png" },
                new Drink { Id = 13, Name = "Lifeline Watermelon 499ml", Price = 151, Amount = 3, BrandId = 4, ImageUrl = "lifelinew.png" },
                new Drink { Id = 14, Name = "Lifeline Lichi 499ml", Price = 140, Amount = 2, BrandId = 4, ImageUrl = "lifelinel.png" },
                new Drink { Id = 15, Name = "Lifeline Peach 499ml", Price = 135, Amount = 0, BrandId = 4, ImageUrl = "lifelinep.png" },
                new Drink { Id = 16, Name = "Добрый Апельсин 499ml", Price = 190, Amount = 5, BrandId = 5, ImageUrl = "dobriyo.png" },
                new Drink { Id = 17, Name = "Добрый Яблоко 499ml", Price = 192, Amount = 4, BrandId = 5, ImageUrl = "dobriya.png" },
                new Drink { Id = 18, Name = "Добрый Мультифрукт 499ml", Price = 181, Amount = 3, BrandId = 5, ImageUrl = "dobriym.png" },
                new Drink { Id = 19, Name = "Lipton Ice Tea Lemon 499ml", Price = 148, Amount = 5, BrandId = 6, ImageUrl = "lipton.png" },
                new Drink { Id = 20, Name = "Lipton Ice Tea Peach 499ml", Price = 187, Amount = 9, BrandId = 6, ImageUrl = "liptonp.png" },
                new Drink { Id = 21, Name = "Lipton Green Tea 499ml", Price = 201, Amount = 2, BrandId = 6, ImageUrl = "liptong.png" },
                new Drink { Id = 22, Name = "Шишкин лес Газированная 333ml", Price = 64, Amount = 15, BrandId = 7, ImageUrl = "sl.png" },
                new Drink { Id = 23, Name = "Шишкин лес Негазированная 333ml", Price = 44, Amount = 4, BrandId = 7, ImageUrl = "sl.png" },
                new Drink { Id = 24, Name = "Шишкин лес Минеральная 333ml", Price = 32, Amount = 4, BrandId = 7, ImageUrl = "sl.png" }
                );
        }
    }
}