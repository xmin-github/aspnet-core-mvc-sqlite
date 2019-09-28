using Microsoft.EntityFrameworkCore;

namespace dotnetcoremvcclient.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
    }
}