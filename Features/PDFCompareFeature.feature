Feature: PDF Comparison

Scenario Outline:Compare UAT and Prod PDFs
	Given I have the UAT PDF file "<UATFile>"
	And I have the Prod PDF file "<ProdFile>"
	When I compare the PDF files
	Then the differences should be displayed

Examples:
	| UATFile                                      | ProdFile                                      |
	| C:\\Users\\iqbal\\OneDrive\\Desktop\\uat.pdf | C:\\Users\\iqbal\\OneDrive\\Desktop\\prod.pdf |
	| C:\\Users\\iqbal\\OneDrive\\Desktop\\uat.pdf | C:\\Users\\iqbal\\OneDrive\\Desktop\\prod.pdf |
	| C:\\Users\\iqbal\\OneDrive\\Desktop\\uat.pdf | C:\\Users\\iqbal\\OneDrive\\Desktop\\prod.pdf |
	| C:\\Users\\iqbal\\OneDrive\\Desktop\\uat.pdf | C:\\Users\\iqbal\\OneDrive\\Desktop\\prod.pdf |
	| C:\\Users\\iqbal\\OneDrive\\Desktop\\uat.pdf | C:\\Users\\iqbal\\OneDrive\\Desktop\\prod.pdf |
