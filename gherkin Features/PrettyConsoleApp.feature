Feature: PrettyConsoleApp

  Scenario: Evaluate expression
    Given a PrettyConsoleApp configured with options:
      | key | description         |
      | u   | evaluate expression |
    When the user enters:
      """
      2+2
      q
      """
    Then the output contains "The result is: 4"