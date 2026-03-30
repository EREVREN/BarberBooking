using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Interfaces.Repositories;
using BarberBooking.Infrastructure.Messaging;
using BarberBooking.Infrastructure.Persistence;
using BarberBooking.Infrastructure.Persistence.Repositories;
using BarberBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BarberBookingDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Default"),
                ServerVersion.AutoDetect(
                    configuration.GetConnectionString("Default")
                )));
        
        services.AddScoped<IBarberRepository, BarberRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IBarberWorkingDayRepository, BarberWorkingDayRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IBlockedSlotRepository, BlockedSlotRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        



        return services;
    }
}
