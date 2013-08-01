using System;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

namespace SsisUnit
{
    // TODO: This might be better as a special class of assert
    public class ComponentSourceCommand : CommandBase
    {
        private const string PropOperation = "component";
        private const string PropPath = "input";
        private const string PropValue = "inputValues";

        public ComponentSourceCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, object parent)
            : base(testSuite, parent)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, object parent, string commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, object parent, XmlNode commandXml)
            : base(testSuite, parent, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, string operation, string propertyPath, object value)
            : this(testSuite)
        {
            Properties[PropOperation].Value = operation;
            Properties[PropPath].Value = propertyPath;
            Properties[PropValue].Value = value == null ? string.Empty : value.ToString();
        }

        public ComponentSourceCommand(SsisTestSuite testSuite, object parent, string operation, string propertyPath, object value)
            : this(testSuite, parent)
        {
            Properties[PropOperation].Value = operation;
            Properties[PropPath].Value = propertyPath;
            Properties[PropValue].Value = value == null ? string.Empty : value.ToString();
        }

        /// <summary>
        /// Initialize Properties to default values
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="propertyPath"></param>
        /// <param name="value"></param>
        private void InitializeProperties(string operation, string propertyPath, object value)
        {
            if (!Properties.ContainsKey(PropOperation))
                Properties.Add(PropOperation, new CommandProperty(PropOperation, operation));

            if (!Properties.ContainsKey(PropPath))
                Properties.Add(PropPath, new CommandProperty(PropPath, propertyPath));

            if (!Properties.ContainsKey(PropValue))
                Properties.Add(PropValue, new CommandProperty(PropValue, value.ToString()));
        }


        //        public override object Execute()
        //        {
        //            string sourcePath = Properties[PROP_SOURCE_PATH].Value;
        //            string targetPath = Properties[PROP_TARGET_PATH].Value;
        //            string operation = Properties[PROP_OPERATION].Value;

        //            object returnValue = null;

        //            if (operation == "Copy" || operation == "Move")
        //            {
        //                if (targetPath == string.Empty)
        //                {
        //                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The targetPath must be specified for the {0} operation.", operation));
        //                }
        //            }

        //            try
        //            {
        //                if (operation == "Copy")
        //                {
        //                    File.Copy(sourcePath, targetPath, true);
        //                    returnValue = 0;
        //                }
        //                else if (operation == "Exists")
        //                {
        //                    returnValue = File.Exists(sourcePath);
        //                }
        //                else if (operation == "Move")
        //                {
        //                    File.Move(sourcePath, targetPath);
        //                    returnValue = 0;
        //                }
        //                else if (operation == "Delete")
        //                {
        //                    File.Delete(sourcePath);
        //                    returnValue = 0;
        //                }
        //                else if (operation == "LineCount")
        //                {
        //                    return File.ReadAllLines(sourcePath).Length;
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                returnValue = -1;
        //            }

        //            return returnValue;
        //        }

        //        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package)
        //        {
        //            return Execute();
        //        }

        //        public override object Execute(Microsoft.SqlServer.Dts.Runtime.Package package, Microsoft.SqlServer.Dts.Runtime.DtsContainer container)
        //        {
        //            return Execute();
        //        }

        public override object Execute(Package package, DtsContainer container)
        {
            // Evaluate the property
            return LocatePropertyValue(package, PropertyPath, Operation, Value);
        }

        public override object Execute(XmlNode command, Package package, DtsContainer container)
        {
            throw new NotImplementedException();
        }

        private PropertyOperation GetPropertyOperationFromString(string operation)
        {
            if (operation == "Get")
                return PropertyOperation.Get;
            
            if (operation == "Set")
                return PropertyOperation.Set;

            throw new ArgumentException("The operation provided was not valid.");
        }

        [Description("Defines whether to get or set the property.")]
        public PropertyOperation Operation
        {
            get { return GetPropertyOperationFromString(Properties[PropOperation].Value); }
            set { Properties[PropOperation].Value = value.ToString(); }
        }

        [Description("Sets the path of the property to get or set.")]
        public string PropertyPath
        {
            get { return Properties[PropPath].Value; }
            set { Properties[PropPath].Value = value; }
        }

        [Description("Sets the value to apply to the property.")]
        public string Value
        {
            get { return Properties[PropValue].Value; }
            set { Properties[PropValue].Value = value; }
        }

        public enum PropertyOperation
        {
            Get = 0,
            Set = 1
        }

        private object LocatePropertyValue(DtsObject dtsObject, string propertyPath, PropertyOperation operation, object value)
        {
            propertyPath = propertyPath.Replace("\\", ".");
            object returnValue = null;
            string firstPart = propertyPath;
            string restOfString = string.Empty;

            if (propertyPath.Contains("."))
            {
                // Can have periods in object names (like connection manager names)
                // Need to verify that period is not between an index marker
                int delimiterIndex = propertyPath.IndexOf(".");
                // while (delimiterIndex > propertyPath.IndexOf("[") &&
                //     delimiterIndex < propertyPath.IndexOf("]"))
                // {
                //     delimiterIndex = propertyPath.IndexOf(".", delimiterIndex + 1 );
                // }
                if (delimiterIndex > propertyPath.IndexOf("[") &&
                    delimiterIndex < propertyPath.IndexOf("]"))
                {
                    delimiterIndex = propertyPath.IndexOf(".", propertyPath.IndexOf("]"));
                }

                if (delimiterIndex > -1)
                {
                    firstPart = propertyPath.Substring(0, delimiterIndex);
                    restOfString = propertyPath.Substring(delimiterIndex + 1, (propertyPath.Length - (delimiterIndex + 1)));
                    if (firstPart.Length == 0)
                    {
                        return LocatePropertyValue(dtsObject, restOfString, operation, value);
                    }
                }
            }


            if (firstPart.ToUpper().StartsWith("PACKAGE"))
            {
                if (!(dtsObject is Package))
                {
                    throw new ArgumentException("The initial object must be of type Package.", "dtsObject");
                }
                return LocatePropertyValue(dtsObject, restOfString, operation, value);
            }

            // \Package.Variables[User::TestVar].Properties[Value]
            if (firstPart.ToUpper().StartsWith("VARIABLES"))
            {
                if (!(dtsObject is DtsContainer))
                {
                    throw new ArgumentException("Object must be of type DtsContainer to reference variables.", "dtsObject");
                }
                Variables vars = null;
                string varName = GetSubStringBetween(firstPart, "[", "]");

                DtsContainer cont = (DtsContainer)dtsObject;
                cont.VariableDispenser.LockOneForRead(varName, ref vars);
                returnValue = LocatePropertyValue(vars[varName], restOfString, operation, value);
                vars.Unlock();
                return returnValue;
            }

            // \Package.Properties[CreationDate]
            if (firstPart.ToUpper().StartsWith("PROPERTIES"))
            {
                if (!(dtsObject is IDTSPropertiesProvider))
                {
                    throw new ArgumentException("Object must be of type IDTSPropertiesProvider to reference properties.", "dtsObject");
                }
                
                IDTSPropertiesProvider propProv = (IDTSPropertiesProvider)dtsObject;
                string propIndex = GetSubStringBetween(firstPart, "[", "]");

                DtsProperty prop = propProv.Properties[propIndex];
                
                if (operation == PropertyOperation.Set)
                {
                    var variable = dtsObject as Variable;

                    if (variable != null)
                        prop.SetValue(variable, Convert.ChangeType(value, variable.DataType));
                    else
                        prop.SetValue(dtsObject, Convert.ChangeType(value, propProv.Properties[propIndex].Type));
                }

                return prop.GetValue(dtsObject);
            }

            // \Package.Connections[localhost.AdventureWorksDW2008].Properties[Description]
            if (firstPart.ToUpper().StartsWith("CONNECTIONS"))
            {
                if (!(dtsObject is Package))
                {
                    throw new ArgumentException("Object must be of type Package to reference Connections.", "dtsObject");
                }
                string connIndex = GetSubStringBetween(firstPart, "[", "]");
                Package pkg = (Package)dtsObject;
                return LocatePropertyValue(pkg.Connections[connIndex], restOfString, operation, value);
            }

            // \Package.EventHandlers[OnError].Properties[Description]
            if (firstPart.ToUpper().StartsWith("EVENTHANDLERS"))
            {
                if (!(dtsObject is EventsProvider))
                {
                    throw new ArgumentException("Object must be of type EventsProvider to reference events.", "dtsObject");
                }
                EventsProvider eventProvider = (EventsProvider)dtsObject;
                string eventIndex = GetSubStringBetween(firstPart, "[", "]");
                return LocatePropertyValue(eventProvider.EventHandlers[eventIndex], restOfString, operation, value);
            }

            // First Part of string is not one of the hard-coded values - it's either a task or container
            if (!(dtsObject is IDTSSequence))
            {
                throw new ArgumentException("Object must be of type IDTSSequence to reference other tasks or containers.", "dtsObject");
            }

            IDTSSequence seq = (IDTSSequence)dtsObject;
            if (seq.Executables.Contains(firstPart))
            {
                return LocatePropertyValue(seq.Executables[firstPart], restOfString, operation, value);
            }

            // \Package\Sequence Container\Script Task.Properties[Description]
            // \Package\Sequence Container.Properties[Description]
            // \Package\Execute SQL Task.Properties[Description]

            // \Package.EventHandlers[OnError].Variables[System::Cancel].Properties[Value]
            // \Package.EventHandlers[OnError]\Script Task.Properties[Description]
            if (restOfString.Length > 0)
            {
                returnValue = LocatePropertyValue(dtsObject, restOfString, operation, value);
            }

            return returnValue;
        }

        private static string GetSubStringBetween(string stringToParse, string startString, string endString)
        {
            string subString;
            int startPosition = stringToParse.IndexOf(startString) + 1;
            int endPosition = stringToParse.IndexOf(endString);
            subString = stringToParse.Substring(startPosition, endPosition - startPosition);
            return subString;
        }
    }

}
