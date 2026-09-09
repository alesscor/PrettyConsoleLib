using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NCalc;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;
using PrettyConsoleLib;
using PrettyConsoleLib.Specs.Acceptance.Helpers;

namespace PrettyConsoleLib.Specs.Acceptance.Steps
{
    [Binding]
    public class PrettyConsoleAppSteps(ScenarioContext scenarioContext) {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private StringWriter? _capturedOutput;
        private TextWriter? _originalOut;
        private TextReader? _originalIn;


        [Given(@"a PrettyConsoleApp configured with options:")]
        public void GivenAPrettyConsoleAppConfiguredWithOptions(Table table)
        {
            // Keep originals so hooks can restore them
            _originalOut = Console.Out;
            _originalIn = Console.In;

            var options = new List<(char key, string description, Action action)>();

            // Build actions that operate on Console.In/Out (so Console.SetIn/SetOut will control them)
            foreach (var row in table.Rows)
            {
                char key = row["key"][0];
                string description = row["description"];

                if (key == 'u')
                {
                    options.Add((key, description, () =>
                    {
                        Console.Write("Enter a numeric expression for calculating: ");
                        string? userInput = Console.ReadLine();
                        if (userInput == null)
                        {
                            Console.WriteLine("Nothing to do!");
                            return;
                        }

                        try
                        {
                            var expr = new Expression(userInput);
                            object? result = expr.Evaluate();
                            Console.WriteLine($"The result is: {result}");
                        }
                        catch (Exception)
                        {
                            // Let PrettyConsoleApp observe the exception so it can log it.
                            Console.WriteLine("An error occurred while evaluating the expression");
                            throw;
                        }
                    }));
                }
                else if (key == 'm')
                {
                    options.Add((key, description, () => Console.WriteLine("The message is: Hellow there")));
                }
                else
                {
                    options.Add((key, description, () => Console.WriteLine("Unknown option")));
                }
            }

            // Test logger to capture error entries
            var testLogger = new TestLogger();
            _scenarioContext["TestLogger"] = testLogger;

            // Store the menu options and logger; construct the PrettyConsoleApp later after input/output redirection
            _scenarioContext["MenuOptions"] = options.ToArray();
            _scenarioContext["PrettyConsoleApp"] = null!;
        }

        [When(@"the user enters:")]
        public void WhenTheUserEnters(string multiLineInput)
        {
            // Prepare input and output; keep a reference to captured output for assertions
            _capturedOutput = new StringWriter();
            var input = new StringReader(multiLineInput.TrimEnd() + Environment.NewLine);

            // Redirect Console globally so the actions that call Console.ReadLine/WriteLine use these
            Console.SetOut(_capturedOutput);
            Console.SetIn(input);

            // Provide a safe Clear delegate that won't blow up in test runners without a console
            Action safeClear = () =>
            {
                try
                {
                    Console.Clear();
                }
                catch (IOException) { }
                catch (PlatformNotSupportedException) { }
            };

            // Construct the app now that Console.In/Out are redirected so the app captures the test streams
            var options = ( (char option, string description, Action action)[] )_scenarioContext["MenuOptions"]!;
            var testLogger = (TestLogger)_scenarioContext["TestLogger"]!;
            PrettyConsoleApp app = new ((Console.Out, Console.In, safeClear), "PrettyConsoleApp (test)", options, testLogger);
            _scenarioContext["PrettyConsoleApp"] = app;

            // Execute the app (this is synchronous in your Program.cs example)
            app.ExecuteConsoleApp();

            // Save captured output for assertions
            _scenarioContext["CapturedOutput"] = _capturedOutput.ToString();
        }

        [Then(@"the output contains ""(.*)""")]
        public void ThenTheOutputContains(string expected)
        {
            var output = (string)_scenarioContext["CapturedOutput"]!;

            // Normalizes: remove most punctuation, collapse whitespace, and compare case-insensitively.
            static string Normalize(string s) =>
                Regex.Replace(s ?? string.Empty, @"[\p{P}\p{S}]+", " ") // remove punctuation/symbols
                     .Replace("\r\n", "\n")
                     .Replace('\t', ' ')
                     .Trim()
                     .ToLowerInvariant()
                     .Replace("\n", " "); // collapse newlines to spaces for simpler Contains checks

            var normOutput = Normalize(output);
            var normExpected = Normalize(expected);

            Assert.Contains(normExpected, normOutput);
        }

        [Then(@"the test logger recorded an Error entry")]
        public void ThenTheTestLoggerRecordedAnErrorEntry()
        {
            var logger = (TestLogger)_scenarioContext["TestLogger"]!;
            Assert.True(logger.ContainsLevel(LogLevel.Error));
        }

        // optional: step to assert an error message was logged
        // Optional step to assert a specific logged message
        [Then(@"the test logger recorded an Error entry containing ""(.*)""")]
        public void ThenTheTestLoggerRecordedAnErrorEntryContaining(string expectedMessage)
        {
            var logger = (TestLogger)_scenarioContext["TestLogger"]!;
            Assert.True(logger.ContainsLevel(LogLevel.Error));
            Assert.True(logger.ContainsErrorMessage(expectedMessage), $"Expected logger to contain error message '{expectedMessage}'");
        }
    }
}