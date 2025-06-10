using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Infrastructure.Data;
using ConcertTicketSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConcertTicketSystem.Tests.Infrastructure.Repositories
{
    public class TicketTypeRepositoryTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTicketTypesOrderedByPrice()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketTypes = new List<TicketType>
            {
                new TicketType { Id = Guid.NewGuid(), EventId = eventId, Price = 200 },
                new TicketType { Id = Guid.NewGuid(), EventId = eventId, Price = 100 }
            };
            context.TicketTypes.AddRange(ticketTypes);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);

            var result = (await repo.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.True(result[0].Price < result[1].Price);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTicketTypeWithEvent()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketType = new TicketType { Id = Guid.NewGuid(), EventId = eventId, Price = 100 };
            var eventEntity = new Event { Id = eventId, Name = "Concert" };
            context.Events.Add(eventEntity);
            context.TicketTypes.Add(ticketType);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);

            var result = await repo.GetByIdAsync(ticketType.Id);

            Assert.NotNull(result);
            Assert.Equal(ticketType.Id, result.Id);
            Assert.NotNull(result.Event);
            Assert.Equal("Concert", result.Event.Name);
        }

        [Fact]
        public async Task GetByEventIdAsync_ReturnsTicketTypesForEventOrderedByPrice()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var eventId = Guid.NewGuid();
            var ticketTypes = new List<TicketType>
            {
                new TicketType { Id = Guid.NewGuid(), EventId = eventId, Price = 300 },
                new TicketType { Id = Guid.NewGuid(), EventId = eventId, Price = 100 }
            };
            context.TicketTypes.AddRange(ticketTypes);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);

            var result = (await repo.GetByEventIdAsync(eventId)).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, tt => Assert.Equal(eventId, tt.EventId));
            Assert.True(result[0].Price < result[1].Price);
        }

        [Fact]
        public async Task CreateAsync_AddsTicketType()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new TicketTypeRepository(context);
            var ticketType = new TicketType { Id = Guid.NewGuid(), Price = 150 };

            await repo.CreateAsync(ticketType);
            context.SaveChanges();

            Assert.Single(context.TicketTypes);
            Assert.Equal(150, context.TicketTypes.First().Price);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTicketType()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticketType = new TicketType { Id = Guid.NewGuid(), Price = 100 };
            context.TicketTypes.Add(ticketType);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);
            ticketType.Price = 200;

            await repo.UpdateAsync(ticketType);
            context.SaveChanges();

            Assert.Equal(200, context.TicketTypes.First().Price);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTicketType()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticketType = new TicketType { Id = Guid.NewGuid(), Price = 100 };
            context.TicketTypes.Add(ticketType);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);

            await repo.DeleteAsync(ticketType.Id);
            context.SaveChanges();

            Assert.Empty(context.TicketTypes);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueIfExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var ticketType = new TicketType { Id = Guid.NewGuid() };
            context.TicketTypes.Add(ticketType);
            context.SaveChanges();

            var repo = new TicketTypeRepository(context);

            var exists = await repo.ExistsAsync(ticketType.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalseIfNotExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = GetDbContext(dbName);
            var repo = new TicketTypeRepository(context);

            var exists = await repo.ExistsAsync(Guid.NewGuid());

            Assert.False(exists);
        }
    }
}