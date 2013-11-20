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

            if (!(destinationType.IsArray || destinationType.IsList()) || !context.DestinationType.HasDefaultConstructor())
                return null;

            var items = new List<object>();
            string[] lines = Encoding.UTF8.GetString(ReadAllBytes(bodyStream)).Split('\n');

            if (lines.Length > 1)
            {
                string[] fieldNames = lines[0].Split(',');

                foreach (var line in lines.Skip(1))
                {
                    string[] fieldValues = line.Split(',');
                    object instance = CreateInstance(fieldNames, fieldValues, context.GenericType, context);
                    items.Add(instance);
                }
            }

            var list = CreateList(destinationType, items);

            if (list != null)
                context.Configuration.BodyOnly = true;

            return list;
        }

        private object CreateList(Type destinationType, List<object> items)
        {
            if (destinationType.IsList() || destinationType.IsArray)
            {
                IList list = (IList)Activator.CreateInstance(destinationType);

                foreach (var item in items)
                    list.Add(item);

                return list;
            }

            return null;
        }

        private object CreateInstance(string[] fieldNames, string[] fieldValues, Type genericType, BindingContext context)
        {
            object instance = Activator.CreateInstance(genericType);

            for (int i = 0; i < fieldNames.Length && i < fieldValues.Length; i++)
            {
                string propertyName = fieldNames[i].Replace(" ", "");
                string propertyValue = fieldValues[i].Trim();

                PropertyInfo property = context.ValidModelProperties.FirstOrDefault(p => p.Name == propertyName);

                if (property != null)
                {
                    BindProperty(property, propertyValue, context, instance);
                }
            }

            return instance;
        }

        private static void BindProperty(PropertyInfo property, string stringValue, BindingContext context, object instance)
        {
            var destinationType = property.PropertyType;

            if (destinationType == typeof (string))
                property.SetValue(instance, stringValue);

            var typeConverter =
                context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(destinationType, context));

            if (typeConverter != null)
            {
                try
                {
                    
                    property.SetValue(instance, typeConverter.Convert(stringValue, destinationType, context));
                }
                catch (Exception e)
                {
                    throw new PropertyBindingException(property.Name, stringValue, e);
                }
            }
        }

        public byte[] ReadAllBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public static class TypeExtensions
    {
        public static bool IsList(this Type source)
        {
            var listType = typeof(List<>);

            return source.IsGenericType && source.GetGenericTypeDefinition() == listType;
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
