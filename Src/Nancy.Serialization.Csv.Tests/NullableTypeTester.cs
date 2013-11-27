namespace Nancy.Serialization.Csv.Tests
{
    public class NullableTypeTester
    {
        public string StringValue { get; set; }
        public int? NullableIntValue { get; set; }
        public char? NullableCharValue { get; set; }
        public Gender? NullableEnumValue { get; set; }
    }
}
