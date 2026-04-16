using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(v => v.VenueId);
                entity.Property(v => v.VenueName).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Location).IsRequired().HasMaxLength(200);
                entity.Property(v => v.Capacity).IsRequired();
                entity.Property(v => v.ImageUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.HasOne(e => e.Venue)
                      .WithMany(v => v.Events)
                      .HasForeignKey(e => e.VenueId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);
                entity.Property(b => b.BookingReference).IsRequired().HasMaxLength(20);
                entity.Property(b => b.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(b => b.CustomerEmail).IsRequired().HasMaxLength(100);

                entity.HasOne(b => b.Event)
                      .WithMany(e => e.Bookings)
                      .HasForeignKey(b => b.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Venue)
                      .WithMany(v => v.Bookings)
                      .HasForeignKey(b => b.VenueId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Venue>().HasData(
                new Venue { VenueId = 1, VenueName = "The Grand Hall", Location = "123 Main Street, Johannesburg", Capacity = 500, ImageUrl = "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800" },
                new Venue { VenueId = 2, VenueName = "Riverside Conference Centre", Location = "45 River Road, Cape Town", Capacity = 200, ImageUrl = "https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=800" },
                new Venue { VenueId = 3, VenueName = "Skyline Rooftop Venue", Location = "78 High Street, Durban", Capacity = 150, ImageUrl = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?w=800" }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event { EventId = 1, EventName = "Annual Tech Summit 2026", Description = "A premier technology conference bringing together industry leaders.", StartDate = new DateTime(2026, 7, 15, 9, 0, 0), EndDate = new DateTime(2026, 7, 15, 17, 0, 0), VenueId = 1, ImageUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800" },
                new Event { EventId = 2, EventName = "Corporate Strategy Workshop", Description = "Full-day workshop for senior management teams.", StartDate = new DateTime(2026, 8, 20, 8, 30, 0), EndDate = new DateTime(2026, 8, 20, 16, 0, 0), VenueId = 2, ImageUrl = "https://images.unsplash.com/photo-1552664730-d307ca884978?w=800" }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { BookingId = 1, BookingReference = "BK-2026-0001", EventId = 1, VenueId = 1, BookingDate = new DateTime(2026, 5, 1, 10, 0, 0), CustomerName = "Sarah Johnson", CustomerEmail = "sarah.johnson@example.com" }
            );
        }
    }
}
