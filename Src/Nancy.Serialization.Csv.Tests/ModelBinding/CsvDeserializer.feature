Feature: CSV Deserializer
	In order to validate CSV files are deserialized correctly
	As a consumer of CSV files
	I want to ensure objects are constructed correctly.

Scenario Outline: Validate can deserialize
	Given the content <Content Type>
	Then the CSV deserializer should be able to deserialize it

Examples:
	| Content Type        |
	| application/csv     |
	| text/csv            |
	| application/vnd+csv |

Scenario Outline: Validate cannot deserialize
	Given the content <Content Type>
	Then the CSV deserializer should NOT be able to deserialize it

Examples:
	| Content Type         |
	| application/json     |
	| text/json            |
	| application/vnd+json |

Scenario: Validate deserialization into a list
	Given the CSV data
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |
	When it is deserialized into a list object
	Then the following items should be in the list
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |
