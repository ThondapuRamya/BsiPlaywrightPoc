Feature: CodiceFiscale

A short summary of the feature

@Retry(2) @WebScenario
Scenario Outline: Codice Fiscale for Italians during checkout
	Given I am on checkout page as an Italian user with codice fiscale '<codeFiscaleDefined>' on my account profile
	When I '<atCheckout>' the vat number at checkout
	Then the VAT should be applied correctly as expected based on vat was '<codeFiscaleDefined>' for '<persona>'
	And the purchase should be '<purchase>'

Examples:
	| persona | codeFiscaleDefined | atCheckout        | purchase   |
	| Italian | defined            | make no change to | allowed    |
	| Italian | not defined        | defined           | allowed    |
	| Italian | not defined        | make no change to | disallowed |