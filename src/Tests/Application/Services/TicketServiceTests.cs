using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;
using ConcertTicketSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConcertTicketSystem.Tests.Application.Services
{
    public class TicketServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TicketService>> _loggerMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TicketService>>();
            _ticketService = new TicketService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllTicketsAsync_ReturnsMappedTickets()
        {
            // Arrange
            var tickets = new List<Ticket> { new Ticket { Id = Guid.NewGuid() } };
            var ticketDtos = new List<TicketDto> { new TicketDto { Id = tickets[0].Id } };

            _unitOfWorkMock.Setup(u => u.Tickets.GetAllAsync()).ReturnsAsync(tickets);
            _mapperMock.Setup(m => m.Map<IEnumerable<TicketDto>>(tickets)).Returns(ticketDtos);

            // Act
            var result = await _ticketService.GetAllTicketsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(tickets[0].Id, result.First().Id);
        }

        [Fact]
        public async Task GetTicketByIdAsync_TicketExists_ReturnsMappedTicket()
        {
            // Arrange
            var ticket = new Ticket { Id = Guid.NewGuid() };
            var ticketDto = new TicketDto { Id = ticket.Id };

            _unitOfWorkMock.Setup(u => u.Tickets.GetByIdAsync(ticket.Id)).ReturnsAsync(ticket);
            _mapperMock.Setup(m => m.Map<TicketDto>(ticket)).Returns(ticketDto);

            // Act
            var result = await _ticketService.GetTicketByIdAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
        }

        [Fact]
        public async Task GetTicketByIdAsync_TicketDoesNotExist_ReturnsNull()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Tickets.GetByIdAsync(ticketId)).ReturnsAsync((Ticket)null);

            // Act
            var result = await _ticketService.GetTicketByIdAsync(ticketId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReserveTicketAsync_SuccessfulReservation_ReturnsMappedTicket()
        {
            // Arrange
            var ticketTypeId = Guid.NewGuid();
            var reserveDto = new ReserveTicketDto
            {
                TicketTypeId = ticketTypeId,
                CustomerEmail = "test@example.com",
                CustomerName = "Test User"
            };
            var ticketType = new TicketType
            {
                Id = ticketTypeId,
                EventId = Guid.NewGuid(),
                AvailableQuantity = 10,
                Price = 100
            };
            var createdTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketTypeId = ticketTypeId,
                CustomerEmail = reserveDto.CustomerEmail
            };
            var ticketDto = new TicketDto { Id = createdTicket.Id };

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(ticketTypeId)).ReturnsAsync(ticketType);
            _unitOfWorkMock.Setup(u => u.Tickets.CreateAsync(It.IsAny<Ticket>())).ReturnsAsync(createdTicket);
            _unitOfWorkMock.Setup(u => u.TicketTypes.UpdateAsync(ticketType)).Returns(Task.FromResult(ticketType));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(1));
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TicketDto>(createdTicket)).Returns(ticketDto);

            // Act
            var result = await _ticketService.ReserveTicketAsync(reserveDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdTicket.Id, result.Id);
            _unitOfWorkMock.Verify(u => u.Tickets.CreateAsync(It.IsAny<Ticket>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.TicketTypes.UpdateAsync(ticketType), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ReserveTicketAsync_TicketTypeNotFound_ThrowsArgumentException()
        {
            // Arrange
            var reserveDto = new ReserveTicketDto
            {
                TicketTypeId = Guid.NewGuid(),
                CustomerEmail = "test@example.com",
                CustomerName = "Test User"
            };
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.TicketTypes.GetByIdAsync(reserveDto.TicketTypeId)).ReturnsAsync((TicketType)null);
            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _ticketService.ReserveTicketAsync(reserveDto));
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }
    }
}