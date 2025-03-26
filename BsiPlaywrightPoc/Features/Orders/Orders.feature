@WebScenario @purchasedStandard
Feature: Orders

A short summary of the feature

Scenario: Download purchased standard from order history page 
	Given I am on order history page
	When I download a previously purchased standard in my order history
	Then It should link to the correct product Id

Scenario: Download purchased standard from product page 
	Given I am on product page
	When I download a previously purchased standard product page
	Then It should link to the correct product Id
