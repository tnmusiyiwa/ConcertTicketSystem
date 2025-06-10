using System;
using System.Threading;
using System.Threading.Tasks;
using ConcertTicketSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConcertTicketSystem.Tests.Application.Services
{
    public class ReservationExpiryServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsCleanupExpiredReservationsAsync()
        {
            // Arrange
            var ticketServiceMock = new Mock<ITicketService>();
            ticketServiceMock.Setup(t => t.CleanupExpiredReservationsAsync()).Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            var scopeMock = new Mock<IServiceScope>();
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactoryMock.Object);

            scopeMock.Setup(x => x.ServiceProvider)
                .Returns(new ServiceCollection()
                    .AddSingleton(ticketServiceMock.Object)
                    .BuildServiceProvider());

            scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);

            var loggerMock = new Mock<ILogger<ReservationExpiryService>>();

            var service = new ReservationExpiryService(serviceProviderMock.Object, loggerMock.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2)); // Stop after a short delay

            // Act
            await service.StartAsync(cts.Token);

            // Assert
            ticketServiceMock.Verify(t => t.CleanupExpiredReservationsAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_LogsError_WhenExceptionThrown()
        {
            // Arrange
            var ticketServiceMock = new Mock<ITicketService>();
            ticketServiceMock.Setup(t => t.CleanupExpiredReservationsAsync()).ThrowsAsync(new Exception("Cleanup failed"));

            var serviceProviderMock = new Mock<IServiceProvider>();
            var scopeMock = new Mock<IServiceScope>();
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactoryMock.Object);

            scopeMock.Setup(x => x.ServiceProvider)
                .Returns(new ServiceCollection()
                    .AddSingleton(ticketServiceMock.Object)
                    .BuildServiceProvider());

            scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);

            var loggerMock = new Mock<ILogger<ReservationExpiryService>>();

            var service = new ReservationExpiryService(serviceProviderMock.Object, loggerMock.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            // Act
            await service.StartAsync(cts.Token);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred during reservation cleanup")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
}