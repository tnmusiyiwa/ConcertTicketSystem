using ConcertTicketSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Event configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Venue).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.EventDate);
                entity.HasIndex(e => e.IsActive);
            });

            // TicketType configuration
            modelBuilder.Entity<TicketType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Description).HasMaxLength(500);
                entity.Property(t => t.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(t => t.EventId);
                entity.HasIndex(t => t.IsActive);

                entity.HasOne(t => t.Event)
                      .WithMany(e => e.TicketTypes)
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Ticket configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.CustomerEmail).IsRequired().HasMaxLength(200);
                entity.Property(t => t.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Price).HasColumnType("decimal(18,2)");
                entity.Property(t => t.PaymentTransactionId).HasMaxLength(200);
                entity.HasIndex(t => t.EventId);
                entity.HasIndex(t => t.TicketTypeId);
                entity.HasIndex(t => t.CustomerEmail);
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.ExpiresAt);

                entity.HasOne(t => t.Event)
                      .WithMany(e => e.Tickets)
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.TicketType)
                      .WithMany(tt => tt.Tickets)
                      .HasForeignKey(t => t.TicketTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
