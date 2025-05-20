using WellnessCheck.API.Entities;
using WellnessCheck.API.Enums;

namespace WellnessCheck.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Users.Any()) return;

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Email = "admin@wellness.com",
                Password = "admin123",
                Role = Role.Admin
            };

            var joao = new User
            {
                Id = Guid.NewGuid(),
                Name = "João Silva",
                Email = "joao@empresa.com",
                Password = "123456",
                Role = Role.Employee
            };

            var maria = new User
            {
                Id = Guid.NewGuid(),
                Name = "Stella Lima",
                Email = "stella@empresa.com",
                Password = "123456",
                Role = Role.Employee
            };

            context.Users.AddRange(admin, joao, maria);

            var checkIns = new List<CheckIn>
            {
                // João - 3 check-ins
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = joao.Id,
                    Mood = 2,
                    Productivity = ProductivityLevel.Low,
                    Notes = "Feeling tired.",
                    CreatedAt = DateTime.UtcNow.Date.AddDays(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = joao.Id,
                    Mood = 1,
                    Productivity = ProductivityLevel.VeryLow,
                    Notes = "Worst day so far.",
                    CreatedAt = DateTime.UtcNow.Date.AddDays(-1)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = joao.Id,
                    Mood = 4,
                    Productivity = ProductivityLevel.High,
                    Notes = "Much better today.",
                    CreatedAt = DateTime.UtcNow.Date
                },

                // Maria - 3 check-ins
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = maria.Id,
                    Mood = 3,
                    Productivity = ProductivityLevel.Medium,
                    CreatedAt = DateTime.UtcNow.Date.AddDays(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = maria.Id,
                    Mood = 5,
                    Productivity = ProductivityLevel.VeryHigh,
                    Notes = "Great day!",
                    CreatedAt = DateTime.UtcNow.Date.AddDays(-1)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = maria.Id,
                    Mood = 2,
                    Productivity = ProductivityLevel.Low,
                    Notes = "Stressful.",
                    CreatedAt = DateTime.UtcNow.Date
                }
            };

            context.CheckIns.AddRange(checkIns);
            context.SaveChanges();
        }
    }
}
