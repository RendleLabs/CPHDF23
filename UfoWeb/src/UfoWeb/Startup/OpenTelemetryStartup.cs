using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace UfoWeb.Startup;

public static class OpenTelemetryStartup
{
    public static void Add(WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService("UfoWeb", "TinFoilHat", "1.0");

        var tracesConfiguration = builder.Configuration.GetSection("OpenTelemetry:Traces");
        var metricsConfiguration = builder.Configuration.GetSection("OpenTelemetry:Metrics");
        
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracer =>
            {
                tracer.SetResourceBuilder(resourceBuilder);
                tracer.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddSource("UfoData");

                tracer.SetSampler(
                    new ParentBasedSampler(
                        new TraceIdRatioBasedSampler(0.01d)
                        )
                    );
                
                tracer.AddOtlpExporter(otlp =>
                {
                    tracesConfiguration.Bind(otlp);
                });
            })
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder);
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("UfoData");

                metrics.AddOtlpExporter(otlp =>
                {
                    metricsConfiguration.Bind(otlp);
                });
            });
    }
}