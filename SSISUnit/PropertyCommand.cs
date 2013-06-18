﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using System.ComponentModel;

namespace SsisUnit
{
    public class PropertyCommand : CommandBase
    {
        private const string PROP_OPERATION = "operation";
        private const string PROP_PATH = "propertyPath";
        private const string PROP_VALUE = "value";

        public PropertyCommand(SsisTestSuite testSuite)
            : base(testSuite)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
            //Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, PropertyOperation.Get.ToString()));
            //Properties.Add(PROP_PATH, new CommandProperty(PROP_PATH, string.Empty));
            //Properties.Add(PROP_VALUE, new CommandProperty(PROP_VALUE, string.Empty));
        }

        public PropertyCommand(SsisTestSuite testSuite, string commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public PropertyCommand(SsisTestSuite testSuite, XmlNode commandXml)
            : base(testSuite, commandXml)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);
        }

        public PropertyCommand(SsisTestSuite testSuite, string operation, string propertyPath, object value)
            : base(testSuite)
        {
            InitializeProperties(PropertyOperation.Get.ToString(), string.Empty, string.Empty);

            Properties[PROP_OPERATION].Value = operation;
            Properties[PROP_PATH].Value = propertyPath;
            Properties[PROP_VALUE].Value = value == null ? string.Empty : value.ToString();
        }

        /// <summary>
        /// Initialize Properties to default values
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="propertyPath"></param>
        /// <param name="value"></param>
        private void InitializeProperties(string operation, string propertyPath, object value)
        {
            if (!Properties.ContainsKey(PROP_OPERATION))
            {
                Properties.Add(PROP_OPERATION, new CommandProperty(PROP_OPERATION, operation));
            }
            if (!Properties.ContainsKey(PROP_PATH))
            {
                Properties.Add(PROP_PATH, new CommandProperty(PROP_PATH, propertyPath));
            }
            if (!Properties.ContainsKey(PROP_VALUE))
            {
                Properties.Add(PROP_VALUE, new CommandProperty(PROP_VALUE, value.ToString()));
            }
        }

        public override object Execute(Package package, DtsContainer container)
        {
            try
            {
                OnCommandStarted(new CommandStartedEventArgs(DateTime.Now, Name, null, null));

                object returnValue = LocatePropertyValue(package, this.PropertyPath, this.Operation, this.Value);

                OnCommandCompleted(new CommandCompletedEventArgs(DateTime.Now, Name, null, null, string.Format("The {0} command has completed.", Name)));

                return returnValue;
            }
            catch (Exception ex)
            {
                OnCommandFailed(new CommandFailedEventArgs(DateTime.Now, Name, null, null, ex.Message));

                throw;
            }
        }

        public override object Execute(XmlNode command, Package package, DtsContainer container)
        {
            throw new NotImplementedException();
        }

        private PropertyOperation GetPropertyOperationFromString(string operation)
        {
            if (operation == "Get") return PropertyOperation.Get;
            if (operation == "Set") return PropertyOperation.Set;

            throw new ArgumentException("The operation provided was not valid.");
        }

        [Description("Defines whether to get or set the property.")]
        public PropertyOperation Operation
        {
            get { return GetPropertyOperationFromString(Properties[PROP_OPERATION].Value); }
            set { Properties[PROP_OPERATION].Value = value.ToString(); }
        }

        [Description("Sets the path of the property to get or set.")]
        public string PropertyPath
        {
            get { return Properties[PROP_PATH].Value; }
            set { Properties[PROP_PATH].Value = value; }
        }

        [Description("Sets the value to apply to the property.")]
        public string Value
        {
            get { return Properties[PROP_VALUE].Value; }
            set { Properties[PROP_VALUE].Value = value; }
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
                int delimiterIndex = propertyPath.IndexOf(".", StringComparison.Ordinal);

                if (delimiterIndex > propertyPath.IndexOf("[", StringComparison.Ordinal) &&
                    delimiterIndex < propertyPath.IndexOf("]", StringComparison.Ordinal))
                {
                    delimiterIndex = propertyPath.IndexOf(".", propertyPath.IndexOf("]", StringComparison.Ordinal), StringComparison.Ordinal);
                }

                if (delimiterIndex > -1)
                {
                    firstPart = propertyPath.Substring(0, delimiterIndex);
                    restOfString = propertyPath.Substring(delimiterIndex + 1, propertyPath.Length - (delimiterIndex + 1));
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
                    if (dtsObject is Variable)
                    {
                        Variable var = (Variable)dtsObject;
                        prop.SetValue(dtsObject, Convert.ChangeType(value, var.DataType));
                    }
                    else
                    {
                        prop.SetValue(dtsObject, Convert.ChangeType(value, propProv.Properties[propIndex].Type));
                    }
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
            int startPosition = stringToParse.IndexOf(startString, StringComparison.Ordinal) + 1;
            int endPosition = stringToParse.IndexOf(endString, StringComparison.Ordinal);
            string subString = stringToParse.Substring(startPosition, endPosition - startPosition);
            return subString;
        }
    }

}