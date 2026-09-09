# PrettyConsoleLib

Welcome to the `PrettyConsoleLib` project! This library is designed to enhance console output formatting in .NET applications, providing a more visually appealing and user-friendly experience.

## Overview

`PrettyConsoleLib` offers a set of utilities for formatting console output, including support for special characters, colors, and structured layouts. This library aims to simplify the process of creating visually appealing console applications.

## PrettyConsoleLib.Specs.Acceptance

Acceptance tests for the `PrettyConsoleLib` project. This project contains SpecFlow feature files and xUnit step definitions that verify end-to-end behavior and console output formatting, including handling of special characters like the infinity glyph (`∞`).

## Purpose
- Provide executable acceptance tests that exercise public behavior and console output formatting.
- Validate encoding/rendering of special characters across environments.

## Requirements
- .NET 8 SDK
- SpecFlow and test runner packages (see Dependencies)

## Run tests
From the repository root:

````````
dotnet test ./PrettyConsoleLib.Specs.Acceptance
````````

If running locally, ensure your console output encoding is set for UTF-8 when inspecting glyphs in output:

````````
Console.OutputEncoding = System.Text.Encoding.UTF8;
````````

## Feature files and encoding
- Prefer saving `.feature` files as **UTF-8 without BOM** to avoid an invisible U+FEFF BOM interfering with Gherkin matching.
- If a BOM is present, step definitions can defensively trim it with `token = token?.TrimStart('\uFEFF');`.

Quick conversion (PowerShell):

````````
Get-Content -Raw .\Features\YourFile.feature | Set-Content -Encoding utf8 .\Features\YourFile.feature
````````

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

## CI considerations
- Ensure CI agents use the .NET 8 SDK and check out files as UTF-8.
- Configure console encoding in test setup when asserting on glyphs.

## Contributing
Please follow repository `CONTRIBUTING.md` for coding standards, tests and PR workflow.

## License
This project is licensed under the terms contained in the `LICENSE` file in this repository. See `LICENSE` for details.