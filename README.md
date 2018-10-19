# ssisUnit
ssisUnit is a unit testing framework for SQL Server Integration Services. It is loosely based on the xUnit family of unit testing
frameworks, but is tailored to better support the SSIS environment. One of the bigger differences is that you do not
have to write code to create the unit tests. Instead, we have taken a declarative approach to specifying the unit tests.
The test information is stored in an XML file. Since many SSIS developers do not have a background in developing desktop 
or web applications, we felt it was important to deliver something that did not require an understanding of .NET development to use.

ssisUnit supports task level testing in SSIS. Any control flow task can have a set of tests created around it. This level of
granularity in testing can make testing complex packages much easier.

The ssisUnit test framework has evolved into **LegiTest**. If you are looking for:
- coverage for databases (anything with an OLE DB or ODBC provider)
- SQL Server Analysis Services (tabular and multidimensional)
- SQL Server Reporting Services
- a more robust user interface and an enhanced user experience

then please take a look at [LegiTest](http://legitest.com). It supports testing SSIS packages, cubes, databases, and reports, 
and adds some great user experience features, along with offering other valuable tools for BI developers.

## More Info
- [Unit Test Structure](/docs/Unit%20Test%20Structure.md)
- [Getting Started](/docs/Getting%20Started.md)
- [Product Sample Package and Test](/docs/Product%20Sample%20Package%20and%20Test.md)
- [Using Expressions in Asserts](/docs/Using%20Expressions%20in%20Asserts.md)

Please report issues using the Issues tracker in GitHub.
