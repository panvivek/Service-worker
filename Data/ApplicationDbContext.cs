using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Models;


namespace ServiceWorkerWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Service> Services_List { get; set; }
        public DbSet<Worker> Worker_List { get; set; }
        public DbSet<Booking> Booking { get; set; }

    //    public DbSet<ServiceWorkerWebsite.Models.Booking> Booking { get; set; } = default!;

        public DbSet<TimeSlot> TimeSlot_List { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service) // Indicate that Booking has one Service
                .WithMany() // Specify the inverse relationship if applicable
                .HasForeignKey(b => b.Service_Id); // Explicitly set Service_Id as the foreign key

            modelBuilder.Entity<TimeSlot>()
    .ToTable("TimeSlot_List") // If you want to specify the table name explicitly
    .HasOne(ts => ts.Worker)
    .WithMany(w => w.AvailableTimeSlots)
    .HasForeignKey(ts => ts.Worke_Id)
    .OnDelete(DeleteBehavior.Cascade); // For cascade delete, if that's desired


            


        }






    }
}
