using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;
using ConcertTicketSystem.Infrastructure.Data;
using ConcertTicketSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConcertTicketSystem.Tests.Infrastructure.Repositories
{
    public class TicketRepositoryTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTicketsOrderedByCreatedAt()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);

            // Add required related entities
            var eventId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();
            context.Events.Add(new Event { Id = eventId, Name = "Event" });
            context.TicketTypes.Add(new TicketType { Id = ticketTypeId, Name = "Type", EventId = eventId });

            var tickets = new List<Ticket>
    {
        new Ticket { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddMinutes(-1), EventId = eventId, TicketTypeId = ticketTypeId },
        new Ticket { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, EventId = eventId, TicketTypeId = ticketTypeId }
    };
            context.Tickets.AddRange(tickets);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var result = (await repo.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.True(result[0].CreatedAt > result[1].CreatedAt);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTicketWithIncludes()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                TicketTypeId = ticketTypeId,
                CreatedAt = DateTime.UtcNow
            };
            context.Events.Add(new Event { Id = eventId, Name = "Event" });
            context.TicketTypes.Add(new TicketType { Id = ticketTypeId, Name = "Type", EventId = eventId });
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var result = await repo.GetByIdAsync(ticket.Id);

            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
            Assert.NotNull(result.Event);
            Assert.NotNull(result.TicketType);
        }

        [Fact]
        public async Task GetByEventIdAsync_ReturnsTicketsForEvent()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();

            // Add the related Event and TicketType entities
            context.Events.Add(new Event { Id = eventId, Name = "Event" });
            context.TicketTypes.Add(new TicketType { Id = ticketTypeId, Name = "Type", EventId = eventId });

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                TicketTypeId = ticketTypeId,
                CreatedAt = DateTime.UtcNow
            };
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var result = (await repo.GetByEventIdAsync(eventId)).ToList();

            Assert.Single(result);
            Assert.Equal(eventId, result[0].EventId);
        }

        [Fact]
        public async Task GetByCustomerEmailAsync_ReturnsTicketsForCustomer()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var email = "test@example.com";
            var eventId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();

            // Add required related entities
            context.Events.Add(new Event { Id = eventId, Name = "Event" });
            context.TicketTypes.Add(new TicketType { Id = ticketTypeId, Name = "Type", EventId = eventId });

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                CustomerEmail = email,
                EventId = eventId,
                TicketTypeId = ticketTypeId,
                CreatedAt = DateTime.UtcNow
            };
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var result = (await repo.GetByCustomerEmailAsync(email)).ToList();

            Assert.Single(result);
            Assert.Equal(email, result[0].CustomerEmail);
        }

        [Fact]
        public async Task GetExpiredReservationsAsync_ReturnsExpiredReservedTickets()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var expired = new Ticket
            {
                Id = Guid.NewGuid(),
                Status = TicketStatus.Reserved,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
            };
            var notExpired = new Ticket
            {
                Id = Guid.NewGuid(),
                Status = TicketStatus.Reserved,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            context.Tickets.AddRange(expired, notExpired);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var result = (await repo.GetExpiredReservationsAsync()).ToList();

            Assert.Single(result);
            Assert.Equal(expired.Id, result[0].Id);
        }

        [Fact]
        public async Task CreateAsync_AddsTicket()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new TicketRepository(context);
            var ticket = new Ticket { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

            await repo.CreateAsync(ticket);
            context.SaveChanges();

            Assert.Single(context.Tickets);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTicket()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticket = new Ticket { Id = Guid.NewGuid(), CustomerName = "Old" };
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);
            ticket.CustomerName = "New";

            await repo.UpdateAsync(ticket);
            context.SaveChanges();

            Assert.Equal("New", context.Tickets.First().CustomerName);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTicket()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticket = new Ticket { Id = Guid.NewGuid() };
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            await repo.DeleteAsync(ticket.Id);
            context.SaveChanges();

            Assert.Empty(context.Tickets);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueIfExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticket = new Ticket { Id = Guid.NewGuid() };
            context.Tickets.Add(ticket);
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var exists = await repo.ExistsAsync(ticket.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalseIfNotExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new TicketRepository(context);

            var exists = await repo.ExistsAsync(Guid.NewGuid());

            Assert.False(exists);
        }

        [Fact]
        public async Task GetAvailableQuantityAsync_ReturnsAvailableQuantity()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticketTypeId = Guid.NewGuid();
            context.TicketTypes.Add(new TicketType { Id = ticketTypeId, AvailableQuantity = 7 });
            context.SaveChanges();

            var repo = new TicketRepository(context);

            var qty = await repo.GetAvailableQuantityAsync(ticketTypeId);

            Assert.Equal(7, qty);
        }

        [Fact]
        public async Task GetAvailableQuantityAsync_ReturnsZeroIfNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new TicketRepository(context);

            var qty = await repo.GetAvailableQuantityAsync(Guid.NewGuid());

            Assert.Equal(0, qty);
        }
    }
}