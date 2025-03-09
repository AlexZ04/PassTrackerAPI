using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserDb> Users { get; set; }
        public DbSet<AdminDb> Admins { get; set; }
        public DbSet<UserRoleDb> UserRoles { get; set; }
        public DbSet<BlacklistTokenDb> BlacklistTokens { get; set; }
        public DbSet<RequestDB> Requests { get; set; }
        public DbSet<RefreshTokenDb> RefreshTokens { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDb>().HasKey(x => x.Id);
            modelBuilder.Entity<UserDb>().ToTable("users");

            modelBuilder.Entity<AdminDb>().HasKey(x => x.Id);
            modelBuilder.Entity<AdminDb>().ToTable("admins");

            modelBuilder.Entity<UserRoleDb>().HasKey(x => x.Id);
            modelBuilder.Entity<UserRoleDb>().ToTable("roles");

            modelBuilder.Entity<BlacklistTokenDb>().HasKey(x => x.Token);
            modelBuilder.Entity<BlacklistTokenDb>().ToTable("blacklistTokens");

            modelBuilder.Entity<RequestDB>().HasKey(x => x.Id);
            modelBuilder.Entity<RequestDB>().ToTable("requests");

            modelBuilder.Entity<RefreshTokenDb>().HasKey(x => x.Id);
            modelBuilder.Entity<RefreshTokenDb>().ToTable("refreshTokens");

            base.OnModelCreating(modelBuilder);
        }
    }
}
