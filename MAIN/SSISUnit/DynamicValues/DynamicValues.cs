using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SsisUnit.DynamicValues
{
    public class DynamicValues : Dictionary<string, DynamicValue>
    {
        internal DynamicValues(SsisTestSuite testSuite)
        {
            TestSuite = testSuite;
        }

        private SsisTestSuite TestSuite { get; set; }

        public void Add(DynamicValue dynamicValue)
        {
            this.Add(dynamicValue.AppliesTo, dynamicValue);
        }

        public void Apply()
        {
            foreach (var dynamicValue in Values)
            {
                ApplyExpression(dynamicValue);
            }
        }

        private void ApplyExpression(DynamicValue dynamicValue)
        {
            var targetObject = FindObject(TestSuite, dynamicValue.AppliesTo);
            targetObject.Item2.SetValue(targetObject.Item1, EvaluateExpression(targetObject.Item2.PropertyType, dynamicValue.Value), null);
        }

        private object EvaluateExpression(Type targetType, string value)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            // Process Value
            foreach (var parameter in TestSuite.Parameters)
            {
                value = value.Replace("%" + parameter.Key + "%", parameter.Value);
            }

            value = Environment.ExpandEnvironmentVariables(value);

            // Convert Value
            object endValue;
            if (TryParseObject(targetType, value, out endValue))
            {
                return endValue;
            }

            throw new InvalidOperationException("The dynamic value expression did not evaluate to a type that can be set on the specified property.");            
        }

        private static bool TryParseObject(Type targetType, string value, out object endValue)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    endValue = Enum.Parse(targetType, value, true);
                    return true;
                }

                endValue = Convert.ChangeType(value, targetType);
                return true;
            }
            catch (InvalidCastException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentException)
            {
            }

            endValue = null;
            return false;
        }

        private ObjectInfo FindObject(SsisTestSuite testSuite, string appliesTo)
        {
            var queuedObjects = new Queue<string>(appliesTo.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            object currentObject = testSuite;

            // TestSuite/PackageList/Package[TestPkg]/@packagePath
            while (queuedObjects.Count > 1)
            {
                var currentType = currentObject.GetType();
                var testValue = queuedObjects.Dequeue();
                if (testValue.Equals("TestSuite", StringComparison.OrdinalIgnoreCase) && currentType.Name.Equals("SsisTestSuite", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Move to the next object in the queue
                }

                string propertyName;
                string indexer;

                bool propertyUsesIndexer = SplitNode(testValue, out propertyName, out indexer);

                // TODO: Consider using a stack, and adding index processing to be handled like properties

                currentObject = currentObject.GetPropertyValue(propertyName);

                if (propertyUsesIndexer)
                {
                    currentObject = currentObject.GetCollectionItem(indexer);
                }
            }

            // Final Item
            // TODO: Handle indexer as final property?
            var finalProperty = queuedObjects.Dequeue();
            return new ObjectInfo(currentObject, currentObject.GetProperty(finalProperty));
        }

        private bool SplitNode(string testValue, out string property, out string indexer)
        {
            var parseProperty = new Regex(@"(\w+)\[(\w+)\]", RegexOptions.Compiled);
            indexer = null;

            var match = parseProperty.Match(testValue);
            if (match.Success && match.Groups.Count > 2)
            {
                property = match.Groups[1].Value;
                indexer = match.Groups[2].Value;
                return true;
            }

            property = testValue;
            return false;
        }

        private class ObjectInfo
        {
            public object Item1 { get; private set; }
            public PropertyInfo Item2 { get; private set; }

            public ObjectInfo(object item1, PropertyInfo item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }
    }

}