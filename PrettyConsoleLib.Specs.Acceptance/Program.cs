using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCalc;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using PrettyConsoleLib;

/**
 * 
 * PM> dotnet list .\PrettyConsoleLib.Specs.Acceptance\PrettyConsoleLib.Specs.Acceptance.csproj package
Project 'PrettyConsoleLib.Specs.Acceptance' has the following package references
   [net8.0]: 
   Top-level Package                        Requested   Resolved
   > coverlet.collector                     6.0.0       6.0.0   
   > Microsoft.NET.Test.Sdk                 17.8.0      17.8.0  
   > SpecFlow                               3.9.74      3.9.74  
   > SpecFlow.Tools.MsBuild.Generation      3.9.74      3.9.74  
   > SpecFlow.xUnit                         3.9.74      3.9.74  
   > xunit                                  2.5.3       2.5.3   
   > xunit.runner.visualstudio              2.5.3       2.5.3   

 * 
 * The installs
 * ```bash
 * dotnet add package Microsoft.Extensions.Configuration
 * dotnet add package Microsoft.Extensions.Configuration.Json
 * dotnet add package NCalcAsync
 * dotnet add package Serilog
 * dotnet add package Serilog.Extensions.Logging
 * dotnet add package Serilog.Sinks.File
 * ```
 * 
PM> dotnet list .\AppConfigurationAndLogging.csproj package
Project 'AppConfigurationAndLogging' has the following package references
   [net8.0]: 
   Top-level Package                              Requested   Resolved
   > Microsoft.Extensions.Configuration           10.0.11     10.0.11 
   > Microsoft.Extensions.Configuration.Json      10.0.11     10.0.11 
   > NCalcAsync                                   7.1.0       7.1.0   
   > Serilog                                      4.4.0       4.4.0   
   > Serilog.Extensions.Logging                   10.0.0      10.0.0  
   > Serilog.Sinks.File                           7.0.0       7.0.0   
PM> 
 */
// Notes
// =====
// Let's use Gherkin and Reqnroll
//
// A) The configuration
// ====================
//
// 1. Build the configuration pipeline
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Ensures it finds the file in the output directory
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
// 2. Read a nested key using the ':' separator
string appTitle = config["PrettyConsoleApp:Title"] ??= "A Console App";
string appLogFile = config["Logs:Name"] ??= "app-.log";
string appLogDir = config["Logs:Directory"] ??= "logs";

//
// B) The log
// ==========
//
Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
string logPath = Path.Combine(AppContext.BaseDirectory, appLogDir, appLogFile);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();
Console.WriteLine("Log files will be written to: " + Path.GetDirectoryName(logPath));
// Create an ILogger factory that uses Serilog
using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger, dispose: false));
Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("PrettyConsoleApp");


//
// C) Feeding the console app
// ==========================
//
PrettyConsoleApp consoleApp = new (
    (Console.Out, Console.In, Console.Clear),
    appTitle,
    [('u',"Processing your lovely input",TheCall),
    ('m',"Read a special message",TheMessage)],
    logger
 );

static void TheCall() {
    // Accept user input
    Console.Write("Enter a numeric expression for calculating: ");
    string? userInput = Console.ReadLine();

    // Use the data processing service to perform operations and display the result
    if (userInput == null) {
        Console.WriteLine("Nothing to do!");
        return;
    }
    Expression expr = new (userInput);
    object? result = expr.Evaluate();
    Console.WriteLine($"The result is: {result}");
}
static void TheMessage() {
    Console.WriteLine("The message is: Hellow there");
}
consoleApp.ExecuteConsoleApp();
Log.CloseAndFlush();
