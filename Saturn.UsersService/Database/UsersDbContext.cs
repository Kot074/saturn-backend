using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Saturn.UsersService.Common;
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
#if DOCKER
            var connectionString = Environment.GetEnvironmentVariable(CommonConst.ConnectionStringVariable);
            if (connectionString != null)
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                throw new NullReferenceException($"Переменная окружения {CommonConst.ConnectionStringVariable} не существует!");
            }
#else
            options.UseSqlServer(_configuration.GetValue<string>(CommonConst.ConfigurationConnectionStringField));
#endif
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
