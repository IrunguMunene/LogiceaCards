using Microsoft.EntityFrameworkCore;

namespace LogiceaCardDomain
{
    public class LogiceaCardDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public LogiceaCardDbContext(DbContextOptions<LogiceaCardDbContext> dbContextOptions) 
            : base(dbContextOptions) 
        { 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Card>()
                .HasOne(c => c.User)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Card>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Cards)
                .HasForeignKey(c => c.StatusId);
        }
    }
}
