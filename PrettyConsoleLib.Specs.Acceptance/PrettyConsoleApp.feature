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
    3+2


    exit
    """
    Then the output contains "The result is: 5"

  Scenario: Evaluate an invalid expression displays an error
    When the user enters:
    """
    u
    not-an-expression

    exit
    """
    Then the output contains "An error occurred while"

  Scenario: Evaluate an invalid expression displays and logs an error
    When the user enters:
    """
    u
    not-an-expression

    exit
    """
    Then the output contains "An error occurred while"
    And the test logger recorded an Error entry

  Scenario: Evaluate a valid expression after an error
    When the user enters:
    """
    u
    not-an-expression

    u
    30000+2


    exit
    """
    Then the test logger recorded an Error entry
    And the output contains "The result is: 30002"


  Scenario: Evaluate multiple valid expressions
    When the user enters:
    """
    u
    3+2

    u
    -300/0

    u
    3-2000

    u
    9**0.5

    exit
    """
    Then the output contains "The result is: 5"
    And the output contains "The result is: -∞"
    And the output contains "The result is: -1997"
    And the output contains "The result is: 3"


  Scenario: Evaluate a valid expression after multiple invalid expressions
    When the user enters:
    """
    u
    3+2+hhh

    u
    -300/0+hhh

    u
    3-2000+hhh

    u
    9**0.5

    exit
    """
    Then the test logger recorded an Error entry
    And the test logger recorded an Error entry
    And the test logger recorded an Error entry
    And the output contains "The result is: 3"
