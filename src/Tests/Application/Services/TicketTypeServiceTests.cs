using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConcertTicketSystem.Tests.Application.Services
{
    public class TicketTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TicketTypeService>> _loggerMock;
        private readonly TicketTypeService _service;

        public TicketTypeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TicketTypeService>>();
            _service = new TicketTypeService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllTicketTypesAsync_ReturnsMappedTicketTypes()
        {
            var ticketTypes = new List<TicketType> { new TicketType { Id = Guid.NewGuid() } };
            var ticketTypeDtos = new List<TicketTypeDto> { new TicketTypeDto { Id = ticketTypes[0].Id } };

            _unitOfWorkMock.Setup(u => u.TicketTypes.GetAllAsync()).ReturnsAsync(ticketTypes);
            _mapperMock.Setup(m => m.Map<IEnumerable<TicketTypeDto>>(ticketTypes)).Returns(ticketTypeDtos);

            var result = await _service.GetAllTicketTypesAsync();

            Assert.Single(result);
            Assert.Equal(ticketTypes[0].Id, ((List<TicketTypeDto>)result)[0].Id);
        }

        [Fact]
        public async Task GetTicketTypeByIdAsync_Exists_ReturnsMappedTicketType()
        {
            var id = Guid.NewGuid();
            var ticketType = new TicketType { Id = id };
            var ticketTypeDto = new TicketTypeDto { Id = id };

            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(id)).ReturnsAsync(ticketType);
            _mapperMock.Setup(m => m.Map<TicketTypeDto>(ticketType)).Returns(ticketTypeDto);

            var result = await _service.GetTicketTypeByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetTicketTypeByIdAsync_NotExists_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(id)).ReturnsAsync((TicketType)null);

            var result = await _service.GetTicketTypeByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTicketTypesByEventIdAsync_ReturnsMappedTicketTypes()
        {
            var eventId = Guid.NewGuid();
            var ticketTypes = new List<TicketType> { new TicketType { Id = Guid.NewGuid(), EventId = eventId } };
            var ticketTypeDtos = new List<TicketTypeDto> { new TicketTypeDto { Id = ticketTypes[0].Id } };

            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByEventIdAsync(eventId)).ReturnsAsync(ticketTypes);
            _mapperMock.Setup(m => m.Map<IEnumerable<TicketTypeDto>>(ticketTypes)).Returns(ticketTypeDtos);

            var result = await _service.GetTicketTypesByEventIdAsync(eventId);

            Assert.Single(result);
            Assert.Equal(ticketTypes[0].Id, ((List<TicketTypeDto>)result)[0].Id);
        }

        [Fact]
        public async Task CreateTicketTypeAsync_EventExists_ReturnsMappedTicketType()
        {
            var createDto = new CreateTicketTypeDto
            {
                EventId = Guid.NewGuid(),
                Name = "VIP",
                TotalQuantity = 50,
                Price = 100
            };
            var ticketType = new TicketType
            {
                Id = Guid.NewGuid(),
                EventId = createDto.EventId,
                Name = createDto.Name,
                TotalQuantity = createDto.TotalQuantity,
                Price = createDto.Price,
                AvailableQuantity = createDto.TotalQuantity
            };
            var createdTicketType = ticketType;
            var ticketTypeDto = new TicketTypeDto { Id = ticketType.Id };

            _unitOfWorkMock.Setup(u => u.Events.ExistsAsync(createDto.EventId)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<TicketType>(createDto)).Returns(ticketType);
            _unitOfWorkMock.Setup(u => u.TicketTypes.CreateAsync(It.IsAny<TicketType>())).ReturnsAsync(createdTicketType);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(0));
            _mapperMock.Setup(m => m.Map<TicketTypeDto>(createdTicketType)).Returns(ticketTypeDto);

            var result = await _service.CreateTicketTypeAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(ticketType.Id, result.Id);
        }

        [Fact]
        public async Task CreateTicketTypeAsync_EventDoesNotExist_ThrowsArgumentException()
        {
            var createDto = new CreateTicketTypeDto { EventId = Guid.NewGuid() };
            _unitOfWorkMock.Setup(u => u.Events.ExistsAsync(createDto.EventId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTicketTypeAsync(createDto));
        }

        [Fact]
        public async Task UpdateTicketTypeAsync_Exists_ReturnsMappedTicketType()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateTicketTypeDto { Name = "Updated" };
            var existing = new TicketType { Id = id };
            var updated = new TicketType { Id = id, Name = "Updated" };
            var ticketTypeDto = new TicketTypeDto { Id = id, Name = "Updated" };

            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(id)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing)).Callback(() => existing.Name = updateDto.Name);
            _unitOfWorkMock.Setup(u => u.TicketTypes.UpdateAsync(existing)).ReturnsAsync(updated);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(0));
            _mapperMock.Setup(m => m.Map<TicketTypeDto>(updated)).Returns(ticketTypeDto);

            var result = await _service.UpdateTicketTypeAsync(id, updateDto);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Updated", result.Name);
        }

        [Fact]
        public async Task UpdateTicketTypeAsync_NotExists_ThrowsArgumentException()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateTicketTypeDto { Name = "Updated" };
            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(id)).ReturnsAsync((TicketType)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateTicketTypeAsync(id, updateDto));
        }

        [Fact]
        public async Task DeleteTicketTypeAsync_Exists_DeletesAndSaves()
        {
            var id = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.TicketTypes.ExistsAsync(id)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.TicketTypes.DeleteAsync(id)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(0));

            await _service.DeleteTicketTypeAsync(id);

            _unitOfWorkMock.Verify(u => u.TicketTypes.DeleteAsync(id), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTicketTypeAsync_NotExists_ThrowsArgumentException()
        {
            var id = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.TicketTypes.ExistsAsync(id)).ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteTicketTypeAsync(id));
        }
    }
}