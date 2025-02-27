using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserDb> Users { get; set; }
        public DbSet<AdminDb> Admins { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDb>().HasKey(x => x.Id);
            modelBuilder.Entity<UserDb>().ToTable("users");

            modelBuilder.Entity<AdminDb>().HasKey(x => x.Id);
            modelBuilder.Entity<AdminDb>().ToTable("admins");

            base.OnModelCreating(modelBuilder);
        }
    }
}
