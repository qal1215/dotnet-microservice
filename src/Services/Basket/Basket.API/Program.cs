using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Application Services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("BasketDb")!);

    // Có thể sử dụng 2 cách để config Id cho schema,
    // cách dưới hoặc tag [Identity] trên property
    //opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!;
    options.InstanceName = "Basket:";
});

//gRPC Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcConfigs:DiscountUrl"]!);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

// Cross-cutting concerns
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("BasketDb")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Configure the HTTP pipeline
app.MapCarter();
app.UseExceptionHandler(opts => { });

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();