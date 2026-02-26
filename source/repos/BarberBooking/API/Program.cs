using BarberBooking.API.Middleware;
using BarberBooking.Application.Barbers.Queries;
using BarberBooking.Application.Common.Messaging;
using BarberBooking.Infrastructure;
using BarberBooking.Infrastructure.Messaging;
using BarberBooking.Infrastructure.Repositories;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetBarbersQuery).Assembly));


builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();

app.Run();