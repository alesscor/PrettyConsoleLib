using System;
using Xunit;
using TechTalk.SpecFlow;

[Binding]
public class InfinitySteps
{
    private double _result;
    private string _output;

    [Given(@"the result is ""(.*)""")]
    public void GivenTheResultIs(string token)
    {
        // Accept either the glyph or a unicode escape sequence used as a token
        _result = token switch
        {
            "∞" => double.PositiveInfinity,
            "\\u221E" => double.PositiveInfinity,
            "-∞" => double.NegativeInfinity,
            _ when double.TryParse(token, out var v) => v,
            _ => throw new ArgumentException($"Unsupported token: {token}", nameof(token))
        };
    }

    [When(@"I format the result")]
    public void WhenIFormatTheResult()
    {
        // Use a central helper that returns the glyph for infinity
        _output = InfinityHandling.FormatDoubleForDisplay(_result);
    }

    [Then(@"the output should equal ""(.*)""")]
    public void ThenTheOutputShouldEqual(string expected)
    {
        Assert.Equal(expected, _output);
    }
}