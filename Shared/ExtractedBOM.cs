namespace Shared
{
    public class Column
    {
        public string Label { get; set; }
        public bool? Numeric { get; set; }
    }

    public class ExtractedBOM
    {
        public Column[] Columns { get; set; }
        public object[][] Data { get; set; }
    }
}
