_Expressions in Asserts is a feature planned for the ssisUnit 1.1 release. It's implemented in the source, so you can play with it now by downloading and building the source._

# Assert Expressions
The expression feature allows you to embed simple C# expressions into the ExpectedResults of an Assert. This adds a lot of flexibility to the Asserts. Previously, they were limited to evaluating whether result returned by the command object in the Assert was equal to the ExpectedResult. While this worked in a number of scenarios, it wasn't very flexible for checking non-equality, or making the expected result the current date. Now, there is capability in the Assert to handle those scenarios.

The expression in ExpectedResult must evaluate to a boolean (True/False) value, and it has to be valid C# syntax (similar to the expression language in SSIS). You can reference the actual result in the expression using the _result_ object, which will contain the output from the command object contained in the Assert. Since C# is type safe, the expression need cast the result objects to the correct type for the comparison.

To use the expression in an Assert, you'd set the Expression property to true, and put the expression in the ExpectedResult property:

![Expressions In Asserts](/docs/Using%20Expressions%20in%20Asserts_SsisUnitExpressions.png)

Or if you prefer the raw XML:
```xml
<Assert name="ExpressionTest" expectedResult="(int)result==1" testBefore="false" expression="true">
  <SqlCommand connectionRef="AdventureWorks" returnsValue="true">
    SELECT 1FROM Production.Product
  </SqlCommand>
</Assert>
```

## Examples
### Check a result to see if it is less than 1
```csharp
(int)result<=1
```

### Compare a string
```csharp
result.ToString()=="test"
```

### Checking the result against the current date
```csharp
((DateTime)result).Date==DateTime.Now.Date
```
