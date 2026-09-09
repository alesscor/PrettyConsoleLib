csharp tests/Steps/PrettyConsoleAppSteps.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;
using Microsoft.Extensions.Logging;
using AppConfigurationAndLogging.PrettyConsoleAppUtils; // adjust namespace to your library

namespace PrettyConsoleLib.Specs.Acceptance.Steps
{
    [Binding]
    public sealed class PrettyConsoleAppSteps
    {
        private StringWriter _writer = null!;
        private StringReader _reader = null!;
        private TestLogger _testLogger = null!;
        private Task? _appTask;
        private PrettyConsoleApp? _app;

        [Given("a PrettyConsoleApp configured with options:")]
        public void GivenAPrettyConsoleAppConfiguredWithOptions(Table table)
        {
            _writer = new StringWriter();
            // reader will be set later by input step
            _testLogger = new TestLogger();

            var evalAction = ExpressionActions.CreateEvaluateExpressionAction(_writer, null!, _testLogger); // placeholder; we'll recreate with reader later
            var lovelyAction = new Action(() => _writer.WriteLine("Have a lovely day!"));

            // We'll create the app instance later when real reader is available.
            // Keep the menu description stable for assertions.
        }

        [When("the user enters:")]
        public void WhenTheUserEntersMultiline(string multiline)
        {
            // Normalize lines and feed to reader; ensure final exit exists
            var input = multiline.Trim('\r','\n');
            _reader = new StringReader(input + Environment.NewLine);

            // Recreate actions that depend on the reader
            var evalAction = ExpressionActions.CreateEvaluateExpressionAction(_writer, _reader, _testLogger);
            var lovelyAction = new Action(() => _writer.WriteLine("Have a lovely day!"));

            var menu = new (char, string, Action)[]
            {
                ('u', "Evaluate math expression", evalAction),
                ('m', "Display a lovely message", lovelyAction)
            };

            // Create and run the app on a background task so it consumes reader until exit
            _app = new PrettyConsoleApp(( _writer, _reader, (Action)(() => {})), "Spec App", menu, _testLogger);
            _appTask = Task.Run(() => _app.ExecuteConsoleApp());
            // Wait briefly for app to process inputs; better: wait for Task completion if input ends with exit
            _appTask.Wait(TimeSpan.FromSeconds(2));
        }

        [Then("the output contains \"(.*)\"")]
        public void ThenTheOutputContains(string expected)
        {
            var output = _writer.ToString();
            Assert.Contains(expected, output);
        }

        [Then("the test logger recorded an Error entry")]
        public void ThenTheTestLoggerRecordedAnErrorEntry()
        {
            Assert.True(_testLogger.Entries.Any(e => e.LogLevel == LogLevel.Error), "Expected at least one Error log entry.");
        }
    }
}