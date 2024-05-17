namespace judo_backend.Models.Stats
{
    public class BarValues
    {
        public IList<string> Labels { get; set; }
        public IList<BarValueDataset> Values { get; set; }

        public BarValues()
        {
            Labels = new List<string>();
            Values = new List<BarValueDataset>();
        }
    }

    public class BarValueDataset
    {
        public string Label { get; set; }

        public IList<int> Data { get; set; }

        public BarValueDataset()
        {
            Data = new List<int>();
        }
    }
}