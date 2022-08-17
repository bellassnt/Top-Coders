using System.Text.Json;
using System.Text.Json.Serialization;

namespace selic
{
    public class Selic : ISelic
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("selic")]
        public double SelicRate { get; set; }

        public List<Selic> Load()
        {
            using var reader = new StreamReader("./selic.json");
            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<List<Selic>>(json)!;
        }

        public double GetMin(List<Selic> data)
        {
            return data.Min(x => x.SelicRate);
        }

        public double GetMax(List<Selic> data)
        {
            return data.Max(x => x.SelicRate);
        }

        public IEnumerable<double> GetModes(List<Selic> data)
        {
            var groups =  data.GroupBy(x => x.SelicRate)
                              .OrderByDescending(x => x.Count())
                              .Select(x => new 
                              { 
                                  Value = x.Key, Count = x.Count() 
                              })
                              .ToList();

            var maxCount = groups.Max(x => x.Count);

            return groups.Where(x => x.Count == maxCount)
                         .Select(x => x.Value);
        }

        public double GetAverage(List<Selic> data)
        {
            return Math.Round(data.Average(x => x.SelicRate), 2);
        }
    }
}
