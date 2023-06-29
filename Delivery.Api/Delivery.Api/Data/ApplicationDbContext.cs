using Delivery.Api.Model;
using Delivery.Api.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Rating> Ratings { get; set; }
    }
}
