Feature: Standards

A short summary of the feature

@Retry(2) @WebScenario
Scenario: Standards pagination
	Given I navigate to knowledge standard page
	Then 20 standards count should be displayed