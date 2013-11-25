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
        private List<Person> _people;
        private string _contentType;

        [Given(@"the CSV data")]
        public void GivenTheCSVData(Table table)
        {
            _stream = table.ConvertToCsv();
        }

        [When(@"it is deserialized into a list object")]
        public void WhenItIsDeserializedIntoAListObject()
        {
            var bindingContext = new BindingContextForTests(typeof(List<Person>), typeof(Person));
            var deserializer = new CsvDeserializer();
            _people = (List<Person>)deserializer.Deserialize("application/csv", _stream, bindingContext);
            _stream.Dispose();
        }

        [Then(@"the following items should be in the list")]
        public void ThenTheFollowingItemsShouldBeInTheList(Table table)
        {
            table.CompareToSet(_people);
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
