The unit test structure follows the traditional xUnit structure - setup, test, and teardown. The unit tests are defined in XML files. Setup and Teardown can contain multiple commands, which can perform different types of operations. In the current release, the supported command types are SQLCommand, ProcessCommand, and VariableCommand. In Setup, these commands are used to establish the test data needed for the test to execute. In Teardown, the commands can be used to clean up the data. 

Test nodes contain a reference to a task, a task within the package (or the package itself), an expected result, and a command. The command is used to verify that the task has run successfully. For example, a Test node could reference an Execute SQL Task that sets a variable within the package. The Test node would contain a VariableCommand that retrieves the value of the variable and compares it to the expected result to determine if the Execute SQL Task performed as expected. The reference to the task can be the task name or the GUID of the task. If the task reference is the GUID of the package itself, the test is effectively for the entire package.

Setup and Teardown runs prior to and after each Test. If the unit test file contains 5 tests, Setup and Teardown will be run 5 times.

The unit test file can also include a TestRef node, which references another unit test file. This allows unit tests to be nested. The Setup and Teardown from a parent unit test will be added to the child's list of Setup and Teardown nodes, so this allows for defining a common set of Setup and Teardown commands to be used across multiple unit tests.

The ConnectionList node contains a list of Connection nodes. Connection nodes provide the connection strings to use for any SQL statements that need to be executed. In the future, the Connection node will support referencing a connection manager from the package, but this functionality is not yet implemented.


When the unit test is run, this is the sequence of execution for each test:
1. All commands in the setup node are executed.
1. The task referenced by the test is executed. If the task reference is the package guid, the entire package is executed.
1. The command contained in the test node is executed.
1. The value returned from the command is compared to the expected value for the test.
1. All commands in the teardown node are executed.

This sequence repeats for each subsequent test.

This is a sample of a unit test file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<TestSuite xmlns="http://tempuri.org/ssisUnit.xsd">
  <Setup>
    <SQLCommand connectionRef="AdventureWorks" returnsValue="true">
      SELECT COUNT(ProductID) FROM Production.Product
    </SQLCommand>
    <SQLCommand connectionRef="AdventureWorks" returnsValue="false">
      SELECT ProductID FROM Production.Product
    </SQLCommand>
    <ProcessCommand process="CMD.EXE" arguments="/c COPY c:\temp\temp.txt c:\temp\temp2.txt"/>
    <VariableCommand name="ProductRowCount" value="20"/>
  </Setup>
  <Tests>
    <Test name="PassedTestSQL" package="C:\Projects\SSISUnit\SSIS2005\SSIS2005\UT Basic Scenario.dtsx" task="SELECT COUNT" expectedResult="504">
      <VariableCommand name="ProductRowCount"/>
    </Test>
    <Test name="FailedTestSQL" package="C:\Projects\SSISUnit\SSIS2005\SSIS2005\UT Basic Scenario.dtsx" task="SELECT COUNT" expectedResult="1">
      <SQLCommand connectionRef="AdventureWorks" returnsValue="true">
        SELECT COUNT(*) FROM Production.Product
      </SQLCommand>
    </Test>
    <TestRef path="C:\Projects\SSISUnit\UTssisUnit\UTssisUnit_Package.xml" package="C:\Projects\SSISUnit\SSIS2005\SSIS2005\UT_Simple.dtsx" task="{03894FFA-8636-4E81-B0CD-3F78E2CFBBEF}"/>
  </Tests>
  <Teardown>
    <SQLCommand connectionRef="AdventureWorks" returnsValue="true">
      SELECT COUNT(ProductID) FROM Production.Product
    </SQLCommand>
    <VariableCommand name="ProductRowCount" value="10"/>
  </Teardown>
</TestSuite>
```
