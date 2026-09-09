# PrettyConsoleLib

Welcome to the `PrettyConsoleLib` project! This library is designed to enhance console output formatting in .NET applications, providing a more visually appealing and user-friendly experience.

## Overview

`PrettyConsoleLib` offers a set of utilities for formatting console output, including support for special characters, colors, and structured layouts. This library aims to simplify the process of creating visually appealing console applications.

## Getting Started

- **Prerequisites**: .NET 8 SDK.
- **Quick run**: Execute the following command to run acceptance tests:
  
  `dotnet test ./PrettyConsoleLib.Specs.Acceptance`

- Ensure your console output encoding is set for UTF-8 when inspecting glyphs:

  `Console.OutputEncoding = System.Text.Encoding.UTF8;`

- For detailed instructions on contributing or running tests locally, please see `CONTRIBUTING.md`.

## PrettyConsoleLib.Specs.Acceptance

Acceptance tests for the `PrettyConsoleLib` project. This project contains SpecFlow feature files and xUnit step definitions that verify end-to-end behavior and console output formatting, including handling of special characters like the infinity glyph (`∞`);

## Purpose

- Provide executable acceptance tests that exercise public behavior and console output formatting.
- Validate encoding/rendering of special characters across environments.

## Examples

- **Formatting infinity for display in tests or samples**:
```  
  // Convert numeric infinity to glyph when formatting for display
  public static string FormatDoubleForDisplay(double value)  {
      if (double.IsPositiveInfinity(value)) return "\u221E"; // "∞"
      if (double.IsNegativeInfinity(value)) return "-\u221E"; // "-∞"
      if (double.IsNaN(value)) return "NaN";
      return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
  }
```

## Requirements

- .NET 8 SDK
- SpecFlow and test runner packages (see Dependencies)

## Run Tests

From the repository root:
  
`dotnet test ./PrettyConsoleLib.Specs.Acceptance`

If running locally, ensure your console output encoding is set for UTF-8 when inspecting glyphs:
  
`Console.OutputEncoding = System.Text.Encoding.UTF8;`

## Feature Files and Encoding

- Prefer saving `.feature` files as **UTF-8 without BOM** to avoid an invisible U+FEFF BOM interfering with Gherkin matching.
- If a BOM is present, step definitions can defensively trim it with `token = token?.TrimStart('\uFEFF');`.


- Quick conversion (PowerShell):
  
```
Get-Content -Raw .\Features\YourFile.feature | Set-Content -Encoding utf8 .\Features\YourFile.feature
```

## Dependencies

Project `PrettyConsoleLib.Specs.Acceptance` (targeting `net8.0`) references:

- coverlet.collector 6.0.0
- Microsoft.Extensions.Configuration 10.0.12
- Microsoft.Extensions.Configuration.Json 10.0.12
- Microsoft.NET.Test.Sdk 17.8.0
- NCalcAsync 7.1.0
- Serilog 4.4.0
- Serilog.Extensions.Logging 10.0.0
- Serilog.Sinks.File 7.0.0
- SpecFlow 3.9.74
- SpecFlow.Tools.MsBuild.Generation 3.9.74
- SpecFlow.xUnit 3.9.74
- xunit 2.5.3
- xunit.runner.visualstudio 2.5.3

## Continuous Integration Considerations

- Ensure CI agents use the .NET 8 SDK and check out files as UTF-8.
- Configure console encoding in test setup when asserting on glyphs.

## Reporting Issues / Need Help?

Please open an issue on GitHub and include:
1. Repro steps
2. .NET SDK version (`dotnet --version`)
3. OS and terminal/console encoding (if related to glyphs)
4. Relevant logs or failing test output

## Contact / Maintainers

- Maintainer: @alesscor
- Expected response time: 3–5 business days

## Troubleshooting

- **Invisible BOM breaks Gherkin matching**: Convert `.feature` files to UTF-8 without BOM, e.g. PowerShell:
  
  Get-Content -Raw .\Features\YourFile.feature | Set-Content -Encoding utf8 .\Features\YourFile.feature

- If tests that assert glyphs fail locally, ensure `Console.OutputEncoding` is set to `UTF8` in test setup.

## Code of Conduct

Please follow the project's `CODE_OF_CONDUCT.md`. If not present, be respectful and open issues or PRs with constructive feedback.

> Note: Save `README.md` as UTF-8 without BOM to ensure consistent rendering and avoid invisible characters in feature files and documentation.

## License

This project is licensed under the terms contained in the `LICENSE` file in this repository. See `LICENSE` for details.