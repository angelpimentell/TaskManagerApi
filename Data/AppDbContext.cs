using Microsoft.EntityFrameworkCore;
using System;
using TaskManagerApi.Models.Tasks;

namespace TaskManagerApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Tasks.Task<string>> Tasks { get; set; }
    }
}
