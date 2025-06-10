using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcertTicketSystem.Infrastructure.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if data already exists
                if (await _context.Events.AnyAsync())
                {
                    _logger.LogInformation("Database already contains data. Skipping seeding.");
                    return;
                }

                _logger.LogInformation("Starting database seeding...");

                // Create Events
                var events = CreateEvents();
                await _context.Events.AddRangeAsync(events);
                await _context.SaveChangesAsync();

                // Create Ticket Types
                var ticketTypes = CreateTicketTypes(events);
                await _context.TicketTypes.AddRangeAsync(ticketTypes);
                await _context.SaveChangesAsync();

                // Create Sample Tickets
                var tickets = CreateSampleTickets(events, ticketTypes);
                await _context.Tickets.AddRangeAsync(tickets);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Database seeding completed successfully!");
                _logger.LogInformation("Created {EventCount} events, {TicketTypeCount} ticket types, and {TicketCount} sample tickets",
                    events.Count, ticketTypes.Count, tickets.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }

        private List<Event> CreateEvents()
        {
            var now = DateTime.UtcNow;
            
            return new List<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Rock Legends Live",
                    Description = "An epic night of classic rock featuring legendary bands and their greatest hits. Experience the power of live rock music in an unforgettable concert.",
                    Venue = "Madison Square Garden, New York",
                    EventDate = now.AddDays(30),
                    TotalCapacity = 20000,
                    AvailableCapacity = 19850,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Jazz Under the Stars",
                    Description = "A smooth jazz evening under the open sky featuring world-renowned jazz musicians. Perfect for a romantic night out or jazz enthusiasts.",
                    Venue = "Central Park Bandshell, New York",
                    EventDate = now.AddDays(15),
                    TotalCapacity = 5000,
                    AvailableCapacity = 4750,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Electronic Dance Festival",
                    Description = "The biggest EDM festival of the year featuring top DJs from around the world. Dance the night away with incredible beats and amazing visuals.",
                    Venue = "Brooklyn Mirage, Brooklyn",
                    EventDate = now.AddDays(45),
                    TotalCapacity = 15000,
                    AvailableCapacity = 14200,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Classical Symphony Night",
                    Description = "An elegant evening of classical music performed by the renowned Metropolitan Symphony Orchestra. Experience the beauty of classical compositions.",
                    Venue = "Lincoln Center, New York",
                    EventDate = now.AddDays(60),
                    TotalCapacity = 2500,
                    AvailableCapacity = 2350,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Summer Music Festival 2023",
                    Description = "A past event that was completely sold out. Multi-day festival featuring various genres and artists.",
                    Venue = "Governors Island, New York",
                    EventDate = now.AddDays(-30),
                    TotalCapacity = 25000,
                    AvailableCapacity = 0,
                    IsActive = false,
                    CreatedAt = now.AddDays(-60),
                    UpdatedAt = now.AddDays(-30)
                }
            };
        }

        private List<TicketType> CreateTicketTypes(List<Event> events)
        {
            var ticketTypes = new List<TicketType>();
            var now = DateTime.UtcNow;

            // Rock Legends Live - Madison Square Garden
            var rockEvent = events.First(e => e.Name == "Rock Legends Live");
            ticketTypes.AddRange(new[]
            {
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = rockEvent.Id,
                    Name = "General Admission",
                    Description = "Standing room access to the main floor",
                    Price = 89.99m,
                    TotalQuantity = 8000,
                    AvailableQuantity = 7950,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = rockEvent.Id,
                    Name = "Reserved Seating",
                    Description = "Assigned seats in the lower bowl",
                    Price = 149.99m,
                    TotalQuantity = 10000,
                    AvailableQuantity = 9850,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = rockEvent.Id,
                    Name = "VIP Experience",
                    Description = "Premium seating, meet & greet, and exclusive merchandise",
                    Price = 299.99m,
                    TotalQuantity = 2000,
                    AvailableQuantity = 1950,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            });

            // Jazz Under the Stars - Central Park
            var jazzEvent = events.First(e => e.Name == "Jazz Under the Stars");
            ticketTypes.AddRange(new[]
            {
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = jazzEvent.Id,
                    Name = "Lawn Seating",
                    Description = "Bring your own blanket and enjoy from the lawn",
                    Price = 45.00m,
                    TotalQuantity = 3000,
                    AvailableQuantity = 2800,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = jazzEvent.Id,
                    Name = "Premium Seating",
                    Description = "Reserved chairs closer to the stage",
                    Price = 85.00m,
                    TotalQuantity = 1500,
                    AvailableQuantity = 1400,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = jazzEvent.Id,
                    Name = "VIP Table",
                    Description = "Reserved table for 4 with complimentary drinks",
                    Price = 125.00m,
                    TotalQuantity = 500,
                    AvailableQuantity = 450,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            });

            // Electronic Dance Festival - Brooklyn Mirage
            var edmEvent = events.First(e => e.Name == "Electronic Dance Festival");
            ticketTypes.AddRange(new[]
            {
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = edmEvent.Id,
                    Name = "General Admission",
                    Description = "Access to main dance floor and outdoor areas",
                    Price = 199.00m,
                    TotalQuantity = 10000,
                    AvailableQuantity = 9500,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = edmEvent.Id,
                    Name = "VIP Access",
                    Description = "Elevated viewing area with private bar",
                    Price = 349.00m,
                    TotalQuantity = 3000,
                    AvailableQuantity = 2800,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = edmEvent.Id,
                    Name = "Ultra VIP",
                    Description = "Private cabana, bottle service, and artist meet & greet",
                    Price = 449.00m,
                    TotalQuantity = 2000,
                    AvailableQuantity = 1900,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            });

            // Classical Symphony Night - Lincoln Center
            var classicalEvent = events.First(e => e.Name == "Classical Symphony Night");
            ticketTypes.AddRange(new[]
            {
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = classicalEvent.Id,
                    Name = "Balcony Seating",
                    Description = "Upper level seating with excellent acoustics",
                    Price = 65.00m,
                    TotalQuantity = 1000,
                    AvailableQuantity = 950,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = classicalEvent.Id,
                    Name = "Orchestra Seating",
                    Description = "Main floor seating close to the orchestra",
                    Price = 95.00m,
                    TotalQuantity = 1000,
                    AvailableQuantity = 950,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = classicalEvent.Id,
                    Name = "Premium Box",
                    Description = "Private box seating for up to 6 guests",
                    Price = 125.00m,
                    TotalQuantity = 500,
                    AvailableQuantity = 450,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }
            });

            // Past Event - Summer Music Festival (sold out)
            var pastEvent = events.First(e => e.Name == "Summer Music Festival 2023");
            ticketTypes.AddRange(new[]
            {
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = pastEvent.Id,
                    Name = "3-Day Pass",
                    Description = "Full festival access for all three days",
                    Price = 299.00m,
                    TotalQuantity = 20000,
                    AvailableQuantity = 0,
                    IsActive = false,
                    CreatedAt = now.AddDays(-60),
                    UpdatedAt = now.AddDays(-30)
                },
                new TicketType
                {
                    Id = Guid.NewGuid(),
                    EventId = pastEvent.Id,
                    Name = "VIP Weekend",
                    Description = "VIP access with camping and premium amenities",
                    Price = 599.00m,
                    TotalQuantity = 5000,
                    AvailableQuantity = 0,
                    IsActive = false,
                    CreatedAt = now.AddDays(-60),
                    UpdatedAt = now.AddDays(-30)
                }
            });

            return ticketTypes;
        }

        private List<Ticket> CreateSampleTickets(List<Event> events, List<TicketType> ticketTypes)
        {
            var tickets = new List<Ticket>();
            var now = DateTime.UtcNow;

            // Get some ticket types for creating sample tickets
            var rockGeneralAdmission = ticketTypes.First(tt => tt.Name == "General Admission" && 
                events.Any(e => e.Id == tt.EventId && e.Name == "Rock Legends Live"));
            var jazzPremium = ticketTypes.First(tt => tt.Name == "Premium Seating" && 
                events.Any(e => e.Id == tt.EventId && e.Name == "Jazz Under the Stars"));
            var edmVip = ticketTypes.First(tt => tt.Name == "VIP Access" && 
                events.Any(e => e.Id == tt.EventId && e.Name == "Electronic Dance Festival"));

            // Create sample purchased tickets
            tickets.AddRange(new[]
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventId = rockGeneralAdmission.EventId,
                    TicketTypeId = rockGeneralAdmission.Id,
                    CustomerEmail = "john.doe@example.com",
                    CustomerName = "John Doe",
                    Status = TicketStatus.Purchased,
                    Price = rockGeneralAdmission.Price,
                    ReservedAt = now.AddHours(-2),
                    PurchasedAt = now.AddHours(-1.5),
                    PaymentTransactionId = "txn_rock_001",
                    CreatedAt = now.AddHours(-2),
                    UpdatedAt = now.AddHours(-1.5)
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventId = jazzPremium.EventId,
                    TicketTypeId = jazzPremium.Id,
                    CustomerEmail = "jane.smith@example.com",
                    CustomerName = "Jane Smith",
                    Status = TicketStatus.Purchased,
                    Price = jazzPremium.Price,
                    ReservedAt = now.AddDays(-1),
                    PurchasedAt = now.AddDays(-1).AddMinutes(10),
                    PaymentTransactionId = "txn_jazz_002",
                    CreatedAt = now.AddDays(-1),
                    UpdatedAt = now.AddDays(-1).AddMinutes(10)
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventId = edmVip.EventId,
                    TicketTypeId = edmVip.Id,
                    CustomerEmail = "alice.johnson@example.com",
                    CustomerName = "Alice Johnson",
                    Status = TicketStatus.Purchased,
                    Price = edmVip.Price,
                    ReservedAt = now.AddHours(-3),
                    PurchasedAt = now.AddHours(-2.5),
                    PaymentTransactionId = "txn_edm_003",
                    CreatedAt = now.AddHours(-3),
                    UpdatedAt = now.AddHours(-2.5)
                }
            });

            // Create a sample reserved ticket that will expire soon (for testing background service)
            tickets.Add(new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = rockGeneralAdmission.EventId,
                TicketTypeId = rockGeneralAdmission.Id,
                CustomerEmail = "mike.johnson@example.com",
                CustomerName = "Mike Johnson",
                Status = TicketStatus.Reserved,
                Price = rockGeneralAdmission.Price,
                ReservedAt = now.AddMinutes(-5),
                ExpiresAt = now.AddMinutes(10), // Will expire in 10 minutes
                CreatedAt = now.AddMinutes(-5),
                UpdatedAt = now.AddMinutes(-5)
            });

            // Create a cancelled ticket example
            tickets.Add(new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = jazzPremium.EventId,
                TicketTypeId = jazzPremium.Id,
                CustomerEmail = "bob.wilson@example.com",
                CustomerName = "Bob Wilson",
                Status = TicketStatus.Cancelled,
                Price = jazzPremium.Price,
                ReservedAt = now.AddDays(-2),
                CancelledAt = now.AddDays(-2).AddHours(1),
                CreatedAt = now.AddDays(-2),
                UpdatedAt = now.AddDays(-2).AddHours(1)
            });

            return tickets;
        }
    }
}
