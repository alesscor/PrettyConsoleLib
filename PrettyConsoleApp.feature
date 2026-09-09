Feature: PrettyConsoleApp
  In order to evaluate expressions and show messages
  As a consumer of PrettyConsoleLib
  I want a minimal menu, expression evaluation, and error logging

  Background:
    Given a PrettyConsoleApp configured with options:
      | key | description                |
      | u   | Evaluate math expression   |
      | m   | Display a lovely message   |

  Scenario: Evaluate a valid expression
    When the user enters:
      """
      u
      2+2
      exit
      """
    Then the output contains "The result is: 4"

  Scenario: Evaluate an invalid expression logs an error
    When the user enters:
      """
      u
      not-an-expression
      exit
      """
    Then the output contains "An error occurred while evaluating the expression"
    And the test logger recorded an Error entry