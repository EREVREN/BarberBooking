using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BarberBooking.Infrastructure.Persistence;

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

        return services;
    }
}