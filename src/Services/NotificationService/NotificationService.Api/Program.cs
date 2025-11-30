using EcommerceApp.Shared.Infrastructure.EntityFramework;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Infrastructure.Messaging.Consumers;
using NotificationService.Api.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddScoped(typeof(ICommandRepository<,,>), typeof(CommandRepository<,,>));
builder.Services.AddScoped(typeof(IQueryRepository<,,>), typeof(QueryRepository<,,>));
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderStockReservedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", builder.Configuration["RabbitMQ:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.UseMessageRetry(r => r.Exponential(
           retryLimit: 5,
           minInterval: TimeSpan.FromSeconds(2),
           maxInterval: TimeSpan.FromMinutes(2),
           intervalDelta: TimeSpan.FromSeconds(2)));

        cfg.ReceiveEndpoint("notification-service-queue", e =>
        {
            e.ConfigureConsumer<OrderStockReservedConsumer>(context);
        });
    });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Health Checks
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();