using Microsoft.EntityFrameworkCore;

namespace LogiceaCardDomain
{
    public static class LogiceaDbInitializer
    {
        public static void Seed(LogiceaCardDbContext context)
        {
            context.Database.Migrate();

            SeedRoles(context);
            SeedStatuses(context);
            SeedUsers(context);
            SeedCards(context);
        }

        private static void SeedRoles(LogiceaCardDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Member" },
                    new Role { Name = "Admin" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }

        private static void SeedStatuses(LogiceaCardDbContext context)
        {
            if (!context.Statuses.Any())
            {
                var statuses = new List<Status>
                {
                    new Status { Name = "To Do" },
                    new Status { Name = "In Progress" },
                    new Status { Name = "Done" }
                };

                context.Statuses.AddRange(statuses);
                context.SaveChanges();
            }
        }

        private static void SeedUsers(LogiceaCardDbContext context)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Email = "user1@example.com", Password = "hashed_password1", RoleId = 2 },
                    new User { Email = "user2@example.com", Password = "hashed_password2", RoleId = 1 },
                    new User { Email = "user3@example.com", Password = "hashed_password3", RoleId = 1 },
                    new User { Email = "user4@example.com", Password = "hashed_password4", RoleId = 1 },
                    new User { Email = "user5@example.com", Password = "hashed_password5", RoleId = 1 },
                    new User { Email = "user6@example.com", Password = "hashed_password6", RoleId = 1 },
                    new User { Email = "user7@example.com", Password = "hashed_password7", RoleId = 1 },
                    new User { Email = "user8@example.com", Password = "hashed_password8", RoleId = 1 },
                    new User { Email = "user9@example.com", Password = "hashed_password9", RoleId = 1 },
                    new User { Email = "user10@example.com", Password = "hashed_password10", RoleId = 1 },
                    new User { Email = "user11@example.com", Password = "hashed_password11", RoleId = 1 }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }

        private static void SeedCards(LogiceaCardDbContext context)
        {
            if (!context.Cards.Any())
            {
                var cards = new List<Card>
                {
                    new Card { Name = "Task 1", Description = "Description 1", Color = "#123456", StatusId = 1, UserId = 1, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 2", Description = "Description 2", Color = "#759abc", StatusId = 2, UserId = 2, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 3", Description = "Description 2", Color = "#783abc", StatusId = 2, UserId = 2, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 4", Description = "Description 2", Color = "#489abc", StatusId = 2, UserId = 3, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 5", Description = "Description 2", Color = "#189abc", StatusId = 2, UserId = 3, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 6", Description = "Description 2", Color = "#689abc", StatusId = 2, UserId = 4, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 7", Description = "Description 2", Color = "#129abc", StatusId = 2, UserId = 4, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 8", Description = "Description 2", Color = "#234abc", StatusId = 2, UserId = 5, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 9", Description = "Description 2", Color = "#978abc", StatusId = 2, UserId = 5, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 10", Description = "Description 2", Color = "#879abc", StatusId = 2, UserId = 6, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 11", Description = "Description 2", Color = "#091abc", StatusId = 2, UserId = 6, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 12", Description = "Description 2", Color = "#078abc", StatusId = 2, UserId = 7, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 13", Description = "Description 2", Color = "#012abc", StatusId = 2, UserId = 7, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 14", Description = "Description 2", Color = "#000abc", StatusId = 2, UserId = 8, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 15", Description = "Description 2", Color = "#111abc", StatusId = 2, UserId = 8, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 16", Description = "Description 2", Color = "#122abc", StatusId = 2, UserId = 9, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 17", Description = "Description 2", Color = "#222abc", StatusId = 2, UserId = 9, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 18", Description = "Description 2", Color = "#333abc", StatusId = 2, UserId = 10, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 19", Description = "Description 2", Color = "#555abc", StatusId = 2, UserId = 10, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 20", Description = "Description 2", Color = "#999abc", StatusId = 2, UserId = 11, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 21", Description = "Description 2", Color = "#242abc", StatusId = 2, UserId = 11, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 22", Description = "Description 2", Color = "#343abc", StatusId = 2, UserId = 1, CreatedAt = DateTime.UtcNow },
                    new Card { Name = "Task 23", Description = "Description 2", Color = "#242abc", StatusId = 2, UserId = 7, CreatedAt = DateTime.UtcNow }
                };

                context.Cards.AddRange(cards);
                context.SaveChanges();
            }
        }
    }
}
