using ContagemMcpServer.Tools;
using ContagemMcpServer.Tracing;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

using var connectionRedis = ConnectionMultiplexer.Connect(
    builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddSingleton(connectionRedis);

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
        serviceVersion: OpenTelemetryExtensions.ServiceVersion);
builder.Services.AddOpenTelemetry()
    .WithTracing((traceBuilder) =>
    {
        traceBuilder
            .AddSource(OpenTelemetryExtensions.ServiceName)
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation(connectionRedis)
            .AddOtlpExporter(cfg =>
            {
                cfg.Endpoint = new Uri(builder.Configuration["OtlpExporter:Endpoint"]!);
            })
            .AddConsoleExporter();
    });

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<ContadorTool>();

var app = builder.Build();

app.MapMcp();

app.Run();