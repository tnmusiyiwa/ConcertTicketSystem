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
    public class EventServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EventService>> _loggerMock;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EventService>>();
            _eventService = new EventService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateEventAsync_Success_ReturnsEventDto()
        {
            // Arrange
            var createDto = new CreateEventDto
            {
                Name = "Concert",
                EventDate = DateTime.Now,
                TotalCapacity = 100
            };
            var eventEntity = new Event
            {
                Name = createDto.Name,
                EventDate = createDto.EventDate,
                TotalCapacity = createDto.TotalCapacity,
                AvailableCapacity = createDto.TotalCapacity
            };
            Event? capturedEntity = null;
            var eventDto = new EventDto();

            _mapperMock.Setup(m => m.Map<Event>(createDto)).Returns(eventEntity);
            _unitOfWorkMock.Setup(u => u.Events.CreateAsync(It.IsAny<Event>()))
                .Callback<Event>(e =>
                {
                    capturedEntity = e;
                    eventDto.Id = e.Id; // Set the DTO's Id to match the generated one
                })
                .ReturnsAsync(() => capturedEntity!);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(1));
            _mapperMock.Setup(m => m.Map<EventDto>(It.IsAny<Event>())).Returns(() => eventDto);

            // Act
            var result = await _eventService.CreateEventAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(capturedEntity.Id, result.Id);
            _unitOfWorkMock.Verify(u => u.Events.CreateAsync(It.IsAny<Event>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateEventAsync_Exception_ThrowsAndLogs()
        {
            // Arrange
            var createDto = new CreateEventDto
            {
                Name = "Concert",
                EventDate = DateTime.Now,
                TotalCapacity = 100
            };
            _mapperMock.Setup(m => m.Map<Event>(createDto)).Throws(new Exception("Mapping failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _eventService.CreateEventAsync(createDto));
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while creating event")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsMappedEvents()
        {
            // Arrange
            var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Test" } };
            var eventDtos = new List<EventDto> { new EventDto { Id = events[0].Id, Name = "Test" } };

            _unitOfWorkMock.Setup(u => u.Events.GetAllAsync()).ReturnsAsync(events);
            _mapperMock.Setup(m => m.Map<IEnumerable<EventDto>>(events)).Returns(eventDtos);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(events[0].Id, ((List<EventDto>)result)[0].Id);
        }

        [Fact]
        public async Task GetEventByIdAsync_EventExists_ReturnsMappedEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventEntity = new Event { Id = eventId, Name = "Test" };
            var eventDto = new EventDto { Id = eventId, Name = "Test" };

            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);
            _mapperMock.Setup(m => m.Map<EventDto>(eventEntity)).Returns(eventDto);

            // Act
            var result = await _eventService.GetEventByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }

        [Fact]
        public async Task GetEventByIdAsync_EventDoesNotExist_ReturnsNull()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Events.GetByIdAsync(eventId)).ReturnsAsync((Event)null);

            // Act
            var result = await _eventService.GetEventByIdAsync(eventId);

            // Assert
            Assert.Null(result);
        }
    }
}