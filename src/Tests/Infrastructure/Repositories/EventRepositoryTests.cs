using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Infrastructure.Data;
using ConcertTicketSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ConcertTicketSystem.Tests.Infrastructure.Repositories
{
    public class EventRepositoryTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEventsOrderedByDate()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "A", EventDate = DateTime.UtcNow.AddDays(2) },
                new Event { Id = Guid.NewGuid(), Name = "B", EventDate = DateTime.UtcNow.AddDays(1) }
            };
            context.Events.AddRange(events);
            context.SaveChanges();

            var repo = new EventRepository(context);

            // Act
            var result = (await repo.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result[0].EventDate < result[1].EventDate);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEventWithTicketTypes()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketTypes = new List<TicketType>
            {
                new TicketType { Id = Guid.NewGuid(), Name = "VIP", EventId = eventId }
            };
            var eventEntity = new Event
            {
                Id = eventId,
                Name = "Concert",
                EventDate = DateTime.UtcNow,
                TicketTypes = ticketTypes
            };
            context.Events.Add(eventEntity);
            context.TicketTypes.AddRange(ticketTypes);
            context.SaveChanges();

            var repo = new EventRepository(context);

            // Act
            var result = await repo.GetByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            Assert.NotNull(result.TicketTypes);
            Assert.Single(result.TicketTypes);
            Assert.Equal("VIP", result.TicketTypes.First().Name);
        }

        [Fact]
        public async Task CreateAsync_AddsEvent()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new EventRepository(context);
            var eventEntity = new Event { Id = Guid.NewGuid(), Name = "New Event", EventDate = DateTime.UtcNow };

            // Act
            await repo.CreateAsync(eventEntity);
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Events.Count());
            Assert.Equal("New Event", context.Events.First().Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEvent()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventEntity = new Event { Id = Guid.NewGuid(), Name = "Old Name", EventDate = DateTime.UtcNow };
            context.Events.Add(eventEntity);
            context.SaveChanges();

            var repo = new EventRepository(context);
            eventEntity.Name = "Updated Name";

            // Act
            await repo.UpdateAsync(eventEntity);
            context.SaveChanges();

            // Assert
            Assert.Equal("Updated Name", context.Events.First().Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEvent()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventEntity = new Event { Id = Guid.NewGuid(), Name = "To Delete", EventDate = DateTime.UtcNow };
            context.Events.Add(eventEntity);
            context.SaveChanges();

            var repo = new EventRepository(context);

            // Act
            await repo.DeleteAsync(eventEntity.Id);
            context.SaveChanges();

            // Assert
            Assert.Empty(context.Events);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueIfExists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            context.Events.Add(new Event { Id = eventId, Name = "Exists", EventDate = DateTime.UtcNow });
            context.SaveChanges();

            var repo = new EventRepository(context);

            // Act
            var exists = await repo.ExistsAsync(eventId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalseIfNotExists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new EventRepository(context);

            // Act
            var exists = await repo.ExistsAsync(Guid.NewGuid());

            // Assert
            Assert.False(exists);
        }
    }
}