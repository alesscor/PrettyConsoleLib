Feature: PrettyConsoleApp main menu
  In order to operate the console utility
  As a user of the PrettyConsoleApp
  I want to be presented with a minimal menu, select actions, see results, and have errors logged

  Background:
    Given an `appsettings.json` with:
      | PrettyConsoleApp:Title | logs:Directory | logs:Name     |
      | "Calculator"           | "logs"         | "app-.log"    |
    And a PrettyConsoleApp configured with the following options:
      | key | description                |
      | u   | Evaluate math expression   |
      | m   | Display a lovely message   |
    And a test console attached to the app
    And a test logger attached to the app

  @display
  Scenario: Show minimal menu on start
    When the app starts
    Then the menu header is displayed
    And the option "u" with description "Evaluate math expression" is displayed
    And the option "m" with description "Display a lovely message" is displayed
    And the menu prompt is displayed

  @action-message
  Scenario: Selecting "m" shows a lovely message and returns to menu
    Given the app is running
    When the user enters "m"
    Then the lovely message is displayed
    And the menu prompt is displayed again

  @action-evaluate
  Scenario Outline: Selecting "u" asks for an expression and displays the evaluated result
    Given the app is running
    When the user enters "u"
    And the user enters the expression "<expression>"
    Then the output contains the result "<result>"
    And the menu prompt is displayed again

    Examples:
      | expression | result |
      | 2+2        | 4      |
      | 10-3       | 7      |
      | 3*5        | 15     |

  @action-error @logging
  Scenario: Selecting "u" with invalid expression logs an error and displays a friendly message
    Given the app is running
    When the user enters "u"
    And the user enters the invalid expression "not-an-expression"
    Then an error message is displayed to the user
    And a log entry exists containing "Error executing action" or "PrettyConsoleAppException"

  @exit
  Scenario: Entering "exit" terminates the application
    Given the app is running
    When the user enters "exit"
    Then the application stops