using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

namespace Delivery.Api.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            context.Database.Migrate();

            if (context.Users.Any())
            {
                return;
            }

            context.Users.AddRange(
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "SuHao",
                    Email = "suhaoxb@gmail.com",
                    Password = "admin_123",
                    Address = "Tomsk",
                    BirthDate = DateTime.Now.ToString(),
                    Gender = Gender.Male,
                    PhoneNumber = "0123456789",
                });
            context.SaveChanges();
        }
    }
}