Feature: Ingestion

A short summary of the feature

@Retry(2) @WebScenario
Scenario: Digital Standards ingested
	Given I have ingested a digital copy
	And I am on the knowledge standard page
	When I search for the ingested standard
	Then it should be visible
