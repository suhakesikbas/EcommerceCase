using EcommerceApp.Shared.Infrastructure.EntityFramework;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StockService.Api.Infrastructure.Messaging.Consumers;
using StockService.Api.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddScoped(typeof(ICommandRepository<,,>), typeof(CommandRepository<,,>));
builder.Services.AddScoped(typeof(IQueryRepository<,,>), typeof(QueryRepository<,,>));
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

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

        cfg.ReceiveEndpoint("stock-service-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

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