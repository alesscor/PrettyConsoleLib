using System;
using System.IO;
using TechTalk.SpecFlow;

namespace PrettyConsoleLib.Specs.Acceptance.Hooks
{
    [Binding]
    public class ConsoleHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private TextWriter? _originalOut;
        private TextReader? _originalIn;

        public ConsoleHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Save originals in scenario context so multiple scenarios are safe
            _originalOut = Console.Out;
            _originalIn = Console.In;
            _scenarioContext["OriginalOut"] = _originalOut;
            _scenarioContext["OriginalIn"] = _originalIn;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TryGetValue("OriginalOut", out object? originalOutObj) && originalOutObj is TextWriter originalOut)
            {
                Console.SetOut(originalOut);
            }

            if (_scenarioContext.TryGetValue("OriginalIn", out object? originalInObj) && originalInObj is TextReader originalIn)
            {
                Console.SetIn(originalIn);
            }
        }
    }
}