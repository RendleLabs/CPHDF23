using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace UfoData;

internal static class Instrumentation
{
    public static readonly ActivitySource ActivitySource =
        new("UfoData", "1.0");
    
    public static readonly Meter Meter =
        new("UfoData", "1.0");
}