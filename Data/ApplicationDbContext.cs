using Microsoft.EntityFrameworkCore;


namespace ServiceWorkerWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Service> Services_List { get; set; }
        public DbSet<Worker> Worker_List { get; set; }
    }
}
