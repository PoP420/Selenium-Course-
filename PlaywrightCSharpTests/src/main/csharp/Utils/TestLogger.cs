using System;
using System.Diagnostics;

namespace PlaywrightCSharpTests.Utils;

public static class TestLogger
{
    public static void Log(LogLevel level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[{timestamp}] [{level}] {message}");
        Debug.WriteLine($"[{timestamp}] [{level}] {message}");
    }

    public static void LogInformation(string message) => Log(LogLevel.Information, message);
    public static void LogWarning(string message) => Log(LogLevel.Warning, message);
    public static void LogError(string message) => Log(LogLevel.Error, message);
}

public enum LogLevel
{
    Information,
    Warning,
    Error
}
