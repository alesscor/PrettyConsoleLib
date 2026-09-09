# Contributing to PrettyConsoleLib.Specs.Acceptance

Thank you for contributing! This document describes the development practices, coding conventions and PR workflow used by the `PrettyConsoleLib.Specs.Acceptance` project and guidance for contributing to the `PrettyConsoleLib` library.

## Table of Contents
- Coding standards
- Formatting and editor config
- Tests and feature files
- Generated code
- Contributing to the library
- Pull requests and branches
- Commits and changelogs
- CI and compatibility

## Coding standards
- Follow the repository `.editorconfig`. It is the authoritative source for indentation, naming and formatting rules.
- Target frameworks and language versions: .NET 8 (net8.0). Keep APIs and tests compatible with this target.
- Use idiomatic C# and prefer clarity over terseness. Keep methods small and single-responsibility.
- Name async methods with the `Async` suffix.
- Prefer explicit unit-test naming (e.g., `MethodUnderTest_State_Expected`) and use Given/When/Then for acceptance step names.

## Formatting and editor config
- Ensure your editor respects the repository `.editorconfig` settings. Do not override formatting in PRs.
- Use UTF‑8 without BOM for source files and `.feature` files. This avoids invisible U+FEFF characters that can break Gherkin matching.
- Recommended `.gitattributes` entries (already present in the repo):

```text
*.feature text eol=lf
*.md text
*.cs text
```

## Tests and feature files
- Unit tests use xUnit; acceptance tests use SpecFlow with `SpecFlow.xUnit`.
- Run tests locally with:

```bash
dotnet test ./PrettyConsoleLib.Specs.Acceptance
```

- When asserting on console output or glyphs, ensure test setup sets console encoding to UTF-8, e.g.:

```csharp
// Call from test setup / fixture initialization
Console.OutputEncoding = System.Text.Encoding.UTF8;
```

- Prefer representing special numeric values (e.g., Infinity) as numeric types in logic and render to glyphs ("\u221E") only when formatting for display.
- If your editor writes a BOM into `.feature` files, step definitions may defensively trim BOM tokens using `token = token?.TrimStart('\uFEFF');`.

## Generated code
- Generated files (for example SpecFlow generated `*.feature.cs`) are normally created in `obj/` and should not be committed. Keep `obj/` and `bin/` ignored in `.gitignore`.
- If the build generates code next to sources and it is committed by policy, document the generation step and do not hand-edit generated files. Prefer regenerating via the build tooling.

## Contributing to the library (PrettyConsoleLib)
We welcome contributions that improve the library's correctness, formatting, and usability. There are two supported workflows depending on contributor access and desired history:

1. Preferred: Contribute directly to the library's upstream repository
- Fork or branch the upstream `PrettyConsoleLib` repository and open a pull request against the upstream project.
- Include unit tests for library changes and ensure they run with .NET 8.
- Describe breaking changes clearly in the PR and include migration guidance.

2. Contributing via this repository (vendor-copy under `libs/PrettyConsoleLib/`)
- Use this workflow only if you need to develop the library together with acceptance tests in this repository.
- Place the library source under `libs/PrettyConsoleLib/` and add a `ProjectReference` from `PrettyConsoleLib.Specs.Acceptance` to the library project:

```xml
<ItemGroup>
  <ProjectReference Include="libs/PrettyConsoleLib/src/PrettyConsoleLib/PrettyConsoleLib.csproj" />
</ItemGroup>
```

- Ensure the library project targets a TFM compatible with `net8.0`. If not, update or multi-target as appropriate.
- Include or update unit tests in the library; run both library and acceptance tests:

```bash
dotnet test ./libs/PrettyConsoleLib
dotnet test ./PrettyConsoleLib.Specs.Acceptance
```

- When changes are ready, prefer a PR that documents that the vendor copy is modified and include steps to sync upstream (if applicable).

Notes when editing vendor code
- Do not remove `.git` from the copied library before making changes if you want to preserve history—unless you intentionally vendor a snapshot.
- Keep vendor updates focused and document the upstream commit or release used as the snapshot.

## Pull requests and branches
- Branch naming: `feature/<short-description>`, `fix/<short-description>`, or `chore/<short-description>`.
- Open PRs against `main`. Provide a clear description, motivation, and related issue (if any).
- Include tests that exercise your change. Changes that affect behavior must include or update tests.
- Do not merge until CI passes and at least one approval is received (project maintainers may require more approvals).
- Rebase or squash as requested by maintainers to keep history readable.

## Commits and changelog
- Follow Conventional Commits style for commit messages (e.g., `feat:`, `fix:`, `chore:`).
- Keep commits focused and small. Prefer many small commits over a large unrelated change.
- For library changes that affect public API, add an entry to the `CHANGELOG.md` or include a clear note in the PR description.

## CI and compatibility
- CI targets .NET 8. Ensure local development uses the .NET 8 SDK when running/tests.
- If adding new external dependencies, prefer patch/minor versions and document the reason in the PR.
- If you vendor the library under `libs/`, ensure CI builds and tests both projects. Add CI steps to restore and build the vendor folder if necessary.

## How to contribute
1. Fork the repository (or work on a branch if you have push access).
2. Create a feature branch: `git checkout -b feature/describe-change`.
3. Run tests and linters, update/add tests.
4. Commit changes and push the branch.
5. Open a Pull Request with a clear description and link to related issues.

Thank you for helping improve `PrettyConsoleLib` and its acceptance tests!