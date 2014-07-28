using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SsisUnit
{
    internal static class ObjectHelper
    {
        public static PropertyInfo GetProperty(this object item, string propertyName)
        {
            var type = item.GetType();
            var match = type.GetProperties().FirstOrDefault(entity => entity.Name == propertyName);
            if (match != null)
            {
                return match;
            }

            throw new ArgumentException("The specified property name doesn't exist on this object.", "propertyName");
        }

        public static object GetPropertyValue(this object item, string propertyName)
        {
            var type = item.GetType();
            var match = type.GetProperties().FirstOrDefault(entity => entity.Name == propertyName);
            if (match != null)
            {
                return match.GetValue(item, null);
            }

            throw new ArgumentException("The specified property name doesn't exist on this object.", "propertyName");
        }

        public static object GetCollectionItem<T>(this object item, T indexValue)
        {
            var type = item.GetType();
            foreach (var propertyInfo in type.GetProperties())
            {
                var indexParameters = propertyInfo.GetIndexParameters();
                if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(T))
                {
                    return propertyInfo.GetValue(item, new object[] { indexValue }); 
                }
            }

            throw new ArgumentException("Object had no matching indexer for index type provided.", "item");
        }
    }
}
