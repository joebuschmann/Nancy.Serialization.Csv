using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Nancy.ModelBinding;
using Nancy.Serialization.Csv.ModelBinding;

namespace Nancy.Serialization.Csv.Tests.ModelBinding
{
    [TestFixture]
    public class CsvDeserializerFixture
    {
        [TestCase("application/csv", true)]
        [TestCase("text/csv", true)]
        [TestCase("application/vnd+csv", true)]
        [TestCase("application/json", false)]
        [TestCase("text/json", false)]
        [TestCase("application/vnd+json", false)]
        public void CanDeserialize(string contentType, bool expectedResult)
        {
            var deserializer = new CsvDeserializer();
            bool canDeserialize = deserializer.CanDeserialize(contentType, new BindingContext());

            Assert.That(canDeserialize, Is.EqualTo(expectedResult));
        }

        [Test]
        public void BindList()
        {
//            BindingContext bindingContext = new BindingContextForTests(typeof (List<Person>), typeof (Person));
//            
//            using (Stream stream = EmbeddedResource.Read("TestDataMultiLine.csv"))
//            {
//                var deserializer = new CsvDeserializer();
//                List<Person> instance = (List<Person>)deserializer.Deserialize("application/csv", stream, bindingContext);
//
//                Assert.That(instance, Is.Not.Null);
//                Assert.That(instance.Count, Is.EqualTo(4));
//
//
//            }
        }

        [Test]
        public void BindArray()
        {
            
        }

        [Test]
        public void BindInstance()
        {
            
        }

        [Test]
        public void BindDynamicDictionary()
        {
            
        }

        [Test]
        public void BindTypes()
        {
            
        }
    }
}
