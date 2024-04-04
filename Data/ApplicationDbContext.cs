using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Models;

namespace ServiceWorkerWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Service> Services_List { get; set; }
        public DbSet<Worker> Worker_List { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<TimeSlot> TimeSlot_List { get; set; }
        public DbSet<WorkerService> WorkerServices { get; set; } // Add DbSet for the association table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.Service_Id);

            modelBuilder.Entity<TimeSlot>()
                .ToTable("TimeSlot_List")
                .HasOne(ts => ts.Worker)
                .WithMany(w => w.AvailableTimeSlots)
                .HasForeignKey(ts => ts.Worke_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-many relationship between Worker and Service
            modelBuilder.Entity<WorkerService>()
                .HasKey(ws => new { ws.Worker_Id, ws.Service_Id });

            modelBuilder.Entity<WorkerService>()
                .HasOne(ws => ws.Worker)
                .WithMany(w => w.WorkerServices)
                .HasForeignKey(ws => ws.Worker_Id);

            modelBuilder.Entity<WorkerService>()
                .HasOne(ws => ws.Service)
                .WithMany(s => s.WorkerServices)
                .HasForeignKey(ws => ws.Service_Id);
        }
    }
}
