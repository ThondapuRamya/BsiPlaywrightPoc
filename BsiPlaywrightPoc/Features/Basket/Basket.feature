@Retry(2) @WebScenario 
Feature: Basket

A short summary of the feature

@CreateRandomUserAndLogin
Scenario: Basket Merged when signed in and out
	Given I have a standard in my basket while signed in
	And Given I have added another standard to the basket while signed out
	When I sign back in
	Then Both standards should be visible in my basket

Scenario: Empty friendly Basket message for non-login users 
	Given I navigate to the basket page
	Then There are no items in your basket should be visible

@purchasedStandard
Scenario: Previously purchased standard 
	Given I search and open a previously purchased standard
	Then Add to basket button should be hidden
	And This Standard is already in your collection as a friendly message should also be visible 
