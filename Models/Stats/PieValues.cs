namespace judo_backend.Models.Stats
{
    public class PieValues
    {
        public IList<string> Labels { get; set; }
        public IList<int> Values { get; set; }

        public PieValues()
        {
            Labels = new List<string>();
            Values = new List<int>();
        }
    }
}
