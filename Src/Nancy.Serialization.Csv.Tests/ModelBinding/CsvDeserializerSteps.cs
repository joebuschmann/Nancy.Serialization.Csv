using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Nancy.ModelBinding;
using Nancy.Serialization.Csv.ModelBinding;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Nancy.Serialization.Csv.Tests.ModelBinding
{
    [Binding]
    public class CsvDeserializerSteps
    {
        private Stream _stream;
        private IEnumerable<Person> _people;
        private IEnumerable<NullableTypeTester> _nullableTypeTesters;
        private IEnumerable<DynamicDictionary> _dynamicDictionaries; 
        private string _contentType;

        [Given(@"the CSV data")]
        public void GivenTheCSVData(Table table)
        {
            _stream = table.ConvertToCsv();
        }

        [When(@"it is deserialized into a list")]
        public void WhenItIsDeserializedIntoAListObject()
        {
            var bindingContext = new BindingContextForTests(typeof(List<Person>), typeof(Person));
            var deserializer = new CsvDeserializer();
            var people = (List<Person>)deserializer.Deserialize("application/csv", _stream, bindingContext);
            _people = people;

            _stream.Dispose();
        }

        [When(@"it is deserialized into an array")]
        public void WhenItIsDeserializedIntoAnArray()
        {
            var bindingContext = new BindingContextForTests(typeof(Person[]), typeof(Person));
            var deserializer = new CsvDeserializer();
            var people = (Person[])deserializer.Deserialize("application/csv", _stream, bindingContext);
            _people = people;

            _stream.Dispose();
        }

        [When(@"it is deserialized into an instance of Person")]
        public void WhenItIsDeserializedIntoAnInstanceOfPerson()
        {
            var bindingContext = new BindingContextForTests(typeof(Person), null);
            var deserializer = new CsvDeserializer();
            var people = (Person)deserializer.Deserialize("application/csv", _stream, bindingContext);
            _people = new List<Person>() {people};

            _stream.Dispose();
        }

        [When(@"it is deserialized into a dynamic dictionary list")]
        public void WhenItIsDeserializedIntoADynamicDictionaryList()
        {
            var bindingContext = new BindingContextForTests(typeof(List<DynamicDictionary>), typeof(DynamicDictionary));
            var deserializer = new CsvDeserializer();
            var instance = (List<DynamicDictionary>)deserializer.Deserialize("application/csv", _stream, bindingContext);
            _dynamicDictionaries = instance;

            _stream.Dispose();
        }

        [When(@"it is deserialized into an instance of NullableTypeTester")]
        public void WhenItIsDeserializedIntoAnInstanceOfNullableTypeTester()
        {
            var bindingContext = new BindingContextForTests(typeof(List<NullableTypeTester>), typeof(NullableTypeTester));
            var deserializer = new CsvDeserializer();
            var instance = (List<NullableTypeTester>)deserializer.Deserialize("application/csv", _stream, bindingContext);
            _nullableTypeTesters = instance;

            _stream.Dispose();
        }

        [Then(@"the following items should be in the list")]
        [Then(@"the following instance properties should be set")]
        public void ThenTheFollowingItemsShouldBeInTheList(Table table)
        {
            if (_people != null)
                table.CompareToSet(_people);
            else if (_nullableTypeTesters != null)
                table.CompareToSet(_nullableTypeTesters);
            else if (_dynamicDictionaries != null)
                table.CompareToDynamicSet(_dynamicDictionaries);
            else
                Assert.Fail("No items were found for comparison.");
        }

        [Given(@"the content (.*)")]
        public void GivenTheContentApplicationCsv(string contentType)
        {
            _contentType = contentType;
        }

        [Then(@"the CSV deserializer should be able to deserialize it")]
        public void ThenTheCSVDeserializerShouldBeAbleToDeserializeIt()
        {
            var deserializer = new CsvDeserializer();
            bool canDeserialize = deserializer.CanDeserialize(_contentType, new BindingContext());

            Assert.That(canDeserialize, Is.True);
        }

        [Then(@"the CSV deserializer should NOT be able to deserialize it")]
        public void ThenTheCSVDeserializerShouldNOTBeAbleToDeserializeIt()
        {
            var deserializer = new CsvDeserializer();
            bool canDeserialize = deserializer.CanDeserialize(_contentType, new BindingContext());

            Assert.That(canDeserialize, Is.False);
        }
    }
}
