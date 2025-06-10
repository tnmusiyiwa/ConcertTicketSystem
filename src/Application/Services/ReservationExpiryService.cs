using ConcertTicketSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConcertTicketSystem.Application.Services
{
    public class ReservationExpiryService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationExpiryService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public ReservationExpiryService(IServiceProvider serviceProvider, ILogger<ReservationExpiryService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation expiry service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                    
                    await ticketService.CleanupExpiredReservationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during reservation cleanup");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Reservation expiry service stopped");
        }
    }
}
