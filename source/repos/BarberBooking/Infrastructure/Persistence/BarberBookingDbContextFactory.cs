  
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
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder =
            new DbContextOptionsBuilder<BarberBookingDbContext>();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=barber_booking;User=root;Password=Ee114677&.;";

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new BarberBookingDbContext(optionsBuilder.Options);
    }
}