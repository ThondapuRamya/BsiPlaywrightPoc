Feature: MarketingPreference

This feature helps the marketing team to customise each shop users marketing preferences

@Retry(2) @WebScenario
Scenario: New user set marketing preference
	Given I am on the create new account page
	And I fill out the new account form with valid credentials
	When I check marketing preference checkbox and click create new account
	Then I should be logged in successfully with all marketing preferences options selected for me

