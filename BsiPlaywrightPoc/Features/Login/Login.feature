@Retry(2) @WebScenario
Feature: Login

Discription: verify login feature for the knowledge application works as expected.

Background: 
	Given I navigate to knowledge log in page

Scenario Outline: User can login
	Given I fillOut username '<userName>' and password '<password>' credentials
	When I click login
	Then the user login should be '<expectedResult>'

Examples:
	| userName                   | password         | expectedResult       |
	| sub.admin.user@example.org | I hate passwords | should be successful |
	| invalid@example.org        | I hate passwords | Invalid credentials  |
	
@SmokeTest
Scenario: User can log Out
	Given I am logged In
	When I click logout
	Then I should be logged out successfully