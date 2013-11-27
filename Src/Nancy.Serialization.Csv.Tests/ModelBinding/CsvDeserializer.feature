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

Scenario: Validate binding a list
	Given the CSV data
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |
	When it is deserialized into a list
	Then the following items should be in the list
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |

Scenario: Validate binding to an array
	Given the CSV data
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |
	When it is deserialized into an array
	Then the following items should be in the list
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |

Scenario: Validate binding to an instance
	Given the CSV data
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
	When it is deserialized into an instance of Person
	Then the following instance properties should be set
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |

Scenario: Validate binding to a dynamic dictionary
	Given the CSV data
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |
	When it is deserialized into a dynamic dictionary list
	Then the following items should be in the list
		| First Name | Last Name | Gender |
		| Liz        | Lemon     | Female |
		| Jack       | Donaghy   | Male   |
		| Tracy      | Jordan    | Male   |
		| Jenna      | Maroney   | Female |

Scenario: Validate binding to null values
	Given the CSV data
		| String Value | Nullable Int Value | Nullable Char Value | Nullable Enum Value |
		|              |                    |                     |                     |
		| Jack Donaghy | 12                 | L                   | Male                |
	When it is deserialized into an instance of NullableTypeTester
	Then the following instance properties should be set
		| String Value | Nullable Int Value | Nullable Char Value | Nullable Enum Value |
		|              |                    |                     |                     |
		| Jack Donaghy | 12                 | L                   | Male                |
