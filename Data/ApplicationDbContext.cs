﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Models;
using ServiceWorkerWebsite.Areas.Identity.Data; // Add this for your custom IdentityUser

namespace ServiceWorkerWebsite.Data
{
    // Specify the custom Identity user type
    public class ApplicationDbContext : IdentityDbContext<ServiceWorkerWebsiteUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Service> Services_List { get; set; }
        public DbSet<Worker> Worker_List { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<TimeSlot> TimeSlot_List { get; set; }
        public DbSet<WorkerService> WorkerServices { get; set; } // Add DbSet for the association table
        public DbSet<Applicationuser> Applicationusers { get; set; } // Consider removing this if not used
        public DbSet<UserAddress> UserAddress { get; set; }



        public DbSet<Reviews> Reviews
 {
     get; set;
 }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Make sure to call the base method for Identity

            // Configure relationships
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.Service_Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Worker)
                .WithMany()
                .HasForeignKey(b => b.Worker_Id);

            modelBuilder.Entity<TimeSlot>()
                .ToTable("TimeSlot_List")
                .HasOne(ts => ts.Worker)
                .WithMany(w => w.AvailableTimeSlots) // Ensure the Worker model has AvailableTimeSlots
                .HasForeignKey(ts => ts.Worker_Id)
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



            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Review)
                .HasForeignKey(r => r.Service_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Worker)
                .WithMany(w => w.Reviews)
                .HasForeignKey(r => r.Worker_Id);

            // Configure the relationship for the User foreign key in Worker
            modelBuilder.Entity<Worker>()
                .HasOne(w => w.User) // Navigation property
                .WithMany() // Assuming a user can have multiple workers
                .HasForeignKey(w => w.UserId) // Foreign key
                .OnDelete(DeleteBehavior.Cascade); // Configure delete behavior as needed

            // Configure CustomerAddress relationship
            modelBuilder.Entity<UserAddress>()
                .HasOne(ca => ca.User)
                .WithMany()
                .HasForeignKey(ca => ca.UserId);
        }
    }
}
