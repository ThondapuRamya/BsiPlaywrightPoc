Feature: Vat

A short summary of the feature

@Retry(2) @WebScenario
Scenario Outline: Vat during checkout
	Given I am on checkout page as an '<persona>' user with vat '<vatDefined>' on my account profile
	When I '<atCheckout>' the vat number at checkout
	Then the VAT should be applied correctly as expected based on vat was '<vatDefined>' for '<persona>'
	And the purchase should be '<purchase>'

Examples:
	| persona | vatDefined  | atCheckout        | purchase   |
	| Irish   | defined     | make no change to | allowed    |
	| English | defined     | make no change to | allowed    |
	| Italian | defined     | make no change to | allowed    |
	| German  | defined     | make no change to | allowed    |
	| Irish   | not defined | make no change to | allowed    |
	| English | not defined | make no change to | allowed    |
	| Italian | not defined | make no change to | disallowed |
	| German  | not defined | make no change to | allowed    |
	| Italian | not defined | defined           | allowed    |
