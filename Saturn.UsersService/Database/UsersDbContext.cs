using Microsoft.EntityFrameworkCore;
using Saturn.UsersService.Database.Models;

namespace Saturn.UsersService.Database
{
    public class UsersDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public UsersDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetValue<string>("DbConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserDbModel>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<UserDbModel> Users { get; set; }
    }
}
