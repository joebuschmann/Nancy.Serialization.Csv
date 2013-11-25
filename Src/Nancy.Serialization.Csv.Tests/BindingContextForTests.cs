using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nancy.ModelBinding;
using Nancy.ModelBinding.DefaultConverters;

namespace Nancy.Serialization.Csv.Tests
{
    internal class BindingContextForTests : BindingContext
    {
        public BindingContextForTests(Type destinationType, Type genericType)
        {
            DestinationType = destinationType;
            GenericType = genericType;
            Configuration = new BindingConfig();

            ValidModelProperties = GetProperties(destinationType, genericType);
            TypeConverters = GetTypeConverters();
        }

        private IEnumerable<PropertyInfo> GetProperties(Type modelType, Type genericType)
        {
            Type targetType = genericType ?? modelType;

            return
                targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          .Where(p => p.CanWrite && !p.GetIndexParameters().Any());
        }

        private IEnumerable<ITypeConverter> GetTypeConverters()
        {
            return new ITypeConverter[] {new CollectionConverter(), new FallbackConverter()};
        }
    }
}
