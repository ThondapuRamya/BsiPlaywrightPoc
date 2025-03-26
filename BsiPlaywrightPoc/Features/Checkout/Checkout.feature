Feature: Checkout

Knowledge application has a checkout funtionality which integrates with Shopify and sap.
This helps BSI customers to purchase a standard and for all the necessary information to travel respectively to the right systems.

@Retry(2) @CreateRandomUserAndLogin @WebScenario
Scenario Outline: Purchasing a Standard
	Given I have '<purchaseQuantity>' quantity '<sapId>', '<purchaseType>', standard, in my basket
	And I complete the payment form using '<payBy>' from '<country>'
	When I click pay now
	Then the standard purchase should be successful
	And purchase details should appear in the dwh database

Examples:
	| country        | purchaseType | sapId              | purchaseQuantity | payBy       |
	| United Kingdom | digital copy | 000000000030103100 | 1                | credit card |
	| Australia      | hard copy    | 000000000030103100 | 2                | credit card |
	| Australia      | digital copy | 000000000030103100 | 2                | credit card |
	| Canada         | digital copy | 000000000030103100 | 1                | credit card |
	| New Zealand    | digital copy | 000000000030103100 | 1                | credit card |
	| Zimbabwe       | digital copy | 000000000030103100 | 1                | credit card |
	| United Kingdom | digital copy | 000000000030103100 | 1                | invoice     |
	| Ireland        | digital copy | 000000000030103100 | 1                | credit card |