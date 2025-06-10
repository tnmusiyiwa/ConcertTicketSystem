using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Interfaces;
using ConcertTicketSystem.Infrastructure.Data;
using ConcertTicketSystem.Infrastructure.Mapping;
using ConcertTicketSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConcertTicketSystem.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ITicketTypeService, TicketTypeService>();
            services.AddScoped<ITicketService, TicketService>();

            // Background Services
            services.AddHostedService<ReservationExpiryService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Data Seeder
            services.AddScoped<DataSeeder>();

            return services;
        }
    }
}
