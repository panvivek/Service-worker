﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServiceWorkerWebsite.Data;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240404223637_UpdateDatabase")]
    partial class UpdateDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Service", b =>
                {
                    b.Property<int>("Service_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Service_Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Service_Id");

                    b.ToTable("Services_List");
                });

            modelBuilder.Entity("ServiceWorkerWebsite.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AgreeToTerms")
                        .HasColumnType("bit");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("BookingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Service_Id")
                        .HasColumnType("int");

                    b.Property<int>("Worker_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Service_Id");

                    b.ToTable("Booking");
                });

            modelBuilder.Entity("TimeSlot", b =>
                {
                    b.Property<int>("TimeSlotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TimeSlotId"));

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Worker_Id")
                        .HasColumnType("int");

                    b.HasKey("TimeSlotId");

                    b.HasIndex("Worker_Id");

                    b.ToTable("TimeSlot_List", (string)null);
                });

            modelBuilder.Entity("Worker", b =>
                {
                    b.Property<int>("Worker_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Worker_Id"));

                    b.Property<string>("Availability_Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePic_Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Ratings")
                        .HasColumnType("float");

                    b.Property<string>("Reviews")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Worker_Id");

                    b.ToTable("Worker_List");
                });

            modelBuilder.Entity("WorkerService", b =>
                {
                    b.Property<int>("Worker_Id")
                        .HasColumnType("int");

                    b.Property<int>("Service_Id")
                        .HasColumnType("int");

                    b.Property<int>("WorkerServiceId")
                        .HasColumnType("int");

                    b.HasKey("Worker_Id", "Service_Id");

                    b.HasIndex("Service_Id");

                    b.ToTable("WorkerServices");
                });

            modelBuilder.Entity("ServiceWorkerWebsite.Models.Booking", b =>
                {
                    b.HasOne("Service", "Service")
                        .WithMany()
                        .HasForeignKey("Service_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("TimeSlot", b =>
                {
                    b.HasOne("Worker", "Worker")
                        .WithMany("AvailableTimeSlots")
                        .HasForeignKey("Worker_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("WorkerService", b =>
                {
                    b.HasOne("Service", "Service")
                        .WithMany("WorkerServices")
                        .HasForeignKey("Service_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Worker", "Worker")
                        .WithMany("WorkerServices")
                        .HasForeignKey("Worker_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("Service", b =>
                {
                    b.Navigation("WorkerServices");
                });

            modelBuilder.Entity("Worker", b =>
                {
                    b.Navigation("AvailableTimeSlots");

                    b.Navigation("WorkerServices");
                });
#pragma warning restore 612, 618
        }
    }
}
