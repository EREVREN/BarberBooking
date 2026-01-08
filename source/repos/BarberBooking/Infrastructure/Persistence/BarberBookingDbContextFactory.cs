
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BarberBooking.Infrastructure.Persistence;

public class BarberBookingDbContextFactory
    : IDesignTimeDbContextFactory<BarberBookingDbContext>
{
    public BarberBookingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BarberBookingDbContext>();
        optionsBuilder.UseMySql(
            configuration.GetConnectionString("Default"),
            ServerVersion.AutoDetect(configuration.GetConnectionString("Default")));

        return new BarberBookingDbContext(optionsBuilder.Options);
    }
}