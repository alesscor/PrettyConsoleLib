using System;
using System.Globalization;
using System.Text;
using System.Text.Json;

public static class InfinityHandling
{
    public static void ConfigureConsoleForUnicode()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    public static string FormatDoubleForDisplay(double value)
    {
        if (double.IsPositiveInfinity(value)) return "\u221E";   // "oo"
        if (double.IsNegativeInfinity(value)) return "-\u221E";  // "-oo"
        if (double.IsNaN(value)) return "NaN";
        return value.ToString(CultureInfo.InvariantCulture);
    }

    // Example: produce a JSON-safe representation (replace infinities with the symbol)
    public static string ToJsonSafe(object payload)
    {
        // Simple approach: convert numeric values to display strings before serializing.
        // For complex scenarios implement a JsonConverter<double>.
        var wrapper = new { Result = payload?.ToString() ?? string.Empty };
        return JsonSerializer.Serialize(wrapper);
    }

    // Example usage
    public static void Demo()
    {
        ConfigureConsoleForUnicode();

        double r = double.PositiveInfinity;
        Console.WriteLine($"Result: {FormatDoubleForDisplay(r)}"); // prints "oo"

        // Check numeric infinity in logic
        if (double.IsPositiveInfinity(r))
        {
            // handle special-case logic (fallback, error, or display)
        }
    }
}