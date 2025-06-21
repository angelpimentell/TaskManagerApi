using Microsoft.EntityFrameworkCore;
using System;

namespace TaskManagerApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Task<string>> Tasks { get; set; }
        public DbSet<Models.User> Users { get; set; }
    }
}
