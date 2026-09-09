using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

/**
 * PM> dotnet list .\PrettyConsoleLib.csproj package
Project 'PrettyConsoleLib' has the following package references
   [net8.0]: 
   Top-level Package                   Requested   Resolved
   > Microsoft.Extensions.Logging      10.0.11     10.0.11 
 */
namespace PrettyConsoleLib; 
/// <summary>
/// Exception thrown for errors originating in PrettyConsoleApp execution.
/// </summary>
[Serializable]
public sealed class PrettyConsoleAppException : Exception {
    public PrettyConsoleAppException() : base("Exception in PrettyConsoleApp") { }
    public PrettyConsoleAppException(string message) : base(message) { }
    public PrettyConsoleAppException(string message, 
        Exception innerException) : base(message, innerException) { }
}
/// <summary>
/// `PrettyConsoleApp` is a sealed class designed to create a console application with a 
/// user-friendly menu interface, allowing users to select options and execute associated
/// actions while providing an option to exit the application.
/// </summary>
/// <param name="console">The console or output/input stream including their Clear method</param>
/// <param name="title">The tilte to the console app</param>
/// <param name="menu">The menu options, keys, descriptions and delegate action</param>
/// <param name="logger">The logger platform if any is required</param>
public sealed class PrettyConsoleApp(
    (TextWriter Writer, TextReader Reader, Action Clear) console, 
    string title, 
    (char, string, Action)[] menu,
    ILogger? logger = null
) {
    private volatile bool _displayEnded;
    private readonly string _title = title;
    private readonly (TextWriter Writer, TextReader Reader, Action Clear) _console = console;
    private readonly (char option, string description, Action action)[] _menu = menu;
    private readonly ILogger? _logger = logger;

    private void DisplayHeader() {
        _console.Clear();
        _console.Writer.WriteLine(_title);
        _console.Writer.WriteLine(new string('=', _title.Length));
    }
    private void DisplayFooter() {
        _console.Writer.WriteLine();
        _console.Writer.WriteLine(new string('-', 30));
        _console.Writer.Write("Enter Exit or Q to close the app, any other to return to the main menu: ¦");
    }
    private void DisplayMenu() {
        for (int i = 0; i < _menu.Length; i++) {
            _console.Writer.Write(_menu[i].option);
            _console.Writer.Write(" – ");
            _console.Writer.WriteLine(_menu[i].description);
        }
        _console.Writer.WriteLine();
        _console.Writer.WriteLine(new string('-',30));
        _console.Writer.Write("Enter Exit or Q to close the app: ¦");
    }

    private void ConcludeMenuOption() {
        string? input = _console.Reader.ReadLine();
        if(input == null) {
            FinishDisplay();
            return;
        }
        input = input.Trim();
        if (input.Equals("exit") || input.Equals("q")) {
            FinishDisplay();
            return;
        } else if (!string.IsNullOrEmpty(input)) {
            // nothing
            // it only returns to  main menu
        }
    }
    private void ReadAndExecuteMenuOption() {
        string? input = _console.Reader.ReadLine();
        if (input == null) {
            FinishDisplay();
            return;
        }
        input = input.Trim();
        if (input.Equals("exit") || input.Equals("q")) {
            FinishDisplay();
            return;
        } else if (!string.IsNullOrEmpty(input)) {
            char selectedOption = input[0];
            var (option, description, action) = _menu.FirstOrDefault(x => x.option == selectedOption);
            if (action != null) {
                DisplayHeader();
                _console.Writer.WriteLine("[" + option.ToString() + " – " + description + "]:");
                try {
                    action.Invoke();
                } catch (Exception ex) {
                    PrettyConsoleAppException consoleEx = new ($"Exception on PrettyConsoleApp invoking action for menu option '{option}'",ex);
                    // Log structured error via Microsoft.Extensions.Logging if provided
                    if (_logger != null) {
                        _logger.LogError(ex, "PrettyConsoleApp: failed executing action for option '{Option}'", option);
                    } else {
                        // Fallback: write to the configured writer so behavior is visible without a logger
                        // ... no code here to avoid duplicates and I think it's convenient the user knows what is happening
                    }
                    // Log to the configured writer and continue to main menu
                    _console.Writer.WriteLine();
                    _console.Writer.WriteLine("An error occurred while executing the selected action:");
                    _console.Writer.WriteLine(consoleEx.Message);
                    _console.Writer.WriteLine("Inner: " + ex.GetType().Name + ": " + ex.Message);
                    _console.Writer.WriteLine();
                }
                DisplayFooter();
                ConcludeMenuOption();
            } else {
                _console.Writer.WriteLine("There was no option handler!!");
                DisplayFooter();
                ConcludeMenuOption();
            }
        }
    }
    /// <summary>
    /// Allows to mark the end of the console application input's cycle.
    /// </summary>
    private void FinishDisplay() {
        _displayEnded = true;
    }
    /// <summary>
    /// Runs the console application, repeatedly displaying the title and menu until the display ends.
    /// </summary>
    public void ExecuteConsoleApp() {
        while (!_displayEnded) {
            DisplayHeader();
            DisplayMenu();
            ReadAndExecuteMenuOption();
            _console.Clear();
        }
    }
}
