using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy.ModelBinding;

namespace Nancy.Serialization.Csv.ModelBinding
{
    public class CsvDeserializer : IBodyDeserializer
    {
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/csv", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/csv", StringComparison.InvariantCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+csv", StringComparison.InvariantCultureIgnoreCase));
        }

        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            Type destinationType = context.DestinationType;

            if (!IsValidDestinationType(destinationType))
                return null;

            object model = null;
            string[] lines = bodyStream.AsString().FixLineEndings().Split('\n');

            if (destinationType.IsList() || destinationType.IsArray)
                model = BuildCollection(context, lines);
            else
                model = BuildInstance(context, lines);

            if (model != null)
                context.Configuration.BodyOnly = true;

            return model;
        }

        private object BuildInstance(BindingContext context, string[] lines)
        {
            if (lines.Length > 1)
            {
                string[] fieldNames = lines[0].Split(',');
                string[] fieldValues = lines[1].Split(',');

                return CreateInstance(fieldNames, fieldValues, context.DestinationType, context);
            }

            return null;
        }

        private object BuildCollection(BindingContext context, string[] lines)
        {
            var items = new List<object>();

            if (lines.Length > 1)
            {
                string[] fieldNames = lines[0].Split(',');

                foreach (var line in lines.Skip(1).Where(l => l.Length > 0))
                {
                    string[] fieldValues = line.Split(',');
                    object instance = CreateInstance(fieldNames, fieldValues, context.GenericType, context);
                    items.Add(instance);
                }
            }

            var list = CreateCollectionInstance(context, items);

            return list;
        }

        private object CreateCollectionInstance(BindingContext context, List<object> items)
        {
            Type destinationType = context.DestinationType;

            if (destinationType.IsList())
            {
                IList list = (IList)Activator.CreateInstance(destinationType);

                foreach (var item in items)
                    list.Add(item);

                return list;
            }

            if (destinationType.IsArray)
            {
                var array = Array.CreateInstance(destinationType.GetElementType(), items.Count);

                for (int i = 0; i < items.Count; i++)
                    array.SetValue(items[i], i);

                return array;
            }

            return null;
        }

        private object CreateInstance(string[] fieldNames, string[] fieldValues, Type type, BindingContext context)
        {
            object instance = Activator.CreateInstance(type);

            for (int i = 0; i < fieldNames.Length && i < fieldValues.Length; i++)
            {
                string propertyName = fieldNames[i].Replace(" ", "");
                string propertyValue = fieldValues[i].Trim();

                if (type.IsDynamicDictionary())
                {
                    BindDynamicProperty(propertyName, propertyValue, instance);
                }
                else
                {
                    BindProperty(propertyName, propertyValue, context, instance);
                }
            }

            return instance;
        }

        private void BindDynamicProperty(string propertyName, string propertyValue, object instance)
        {
            var dynamicDictionary = instance as DynamicDictionary;

            if (dynamicDictionary == null)
                return;

            dynamicDictionary[propertyName] = propertyValue;
        }

        private static void BindProperty(string propertyName, string propertyValue, BindingContext context, object instance)
        {
            PropertyInfo property = context.ValidModelProperties.FirstOrDefault(p => p.Name == propertyName);

            if (property == null)
                return;

            var destinationType = property.PropertyType;

            if (destinationType == typeof (string))
                property.SetValue(instance, propertyValue);

            var typeConverter =
                context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(destinationType, context));

            if (typeConverter != null)
            {
                try
                {
                    property.SetValue(instance, typeConverter.Convert(propertyValue, destinationType, context));
                }
                catch (Exception e)
                {
                    throw new PropertyBindingException(property.Name, propertyValue, e);
                }
            }
        }

        private bool IsValidDestinationType(Type destinationType)
        {
            return (destinationType.IsArray || destinationType.HasDefaultConstructor());
        }
    }

    public static class TypeExtensions
    {
        public static bool IsList(this Type type)
        {
            var listType = typeof(List<>);

            return type.IsGenericType && type.GetGenericTypeDefinition() == listType;
        }

        public static bool IsDynamicDictionary(this Type type)
        {
            return type == typeof (DynamicDictionary);
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }

    public static class StringExtensions
    {
        public static string FixLineEndings(this string value)
        {
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }

    public static class StreamExtensions
    {
        public static string AsString(this Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
