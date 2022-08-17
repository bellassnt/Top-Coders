using System.Text.Json.Serialization;
using System.Text.Json;
using System.Data;

namespace selic
{
    public class Program
    {
        public static void Main()
        {
            using var reader = new StreamReader("./selic.json");
            var json = reader.ReadToEnd();
            var data = JsonSerializer.Deserialize<List<Selic>>(json)!;

            // Menor e maior valores da taxa Selic
            var minSelic = data.Min(x => x.SelicRate);
            var maxSelic = data.Max(x => x.SelicRate);

            Console.WriteLine($"O menor e o maior valor da taxa Selic é {minSelic} e {maxSelic}, respectivamente.\n");

            // Moda(s) da taxa Selic
            var groups = data.GroupBy(x => x.SelicRate)
                             .OrderByDescending(x => x.Count())
                             .Select(g => new 
                             { 
                                 Value = g.Key, Count = g.Count() 
                             })
                             .ToList();

            var maxCount = groups.Max(g => g.Count);

            var modes = groups.Where(g => g.Count == maxCount)
                              .Select(g => g.Value);

            Console.WriteLine("O(s) valor(es) mais comum(ns) da taxa Selic é(são): ");
            foreach (var mode in modes)
                Console.WriteLine(mode);

            // Média da taxa Selic
            var averageSelic = Math.Round(data.Average(x => x.SelicRate), 2);
            Console.WriteLine($"\nA média da taxa Selic é {averageSelic}.");

            //
            var changingMonths = data.OrderBy(x => x.Date)
                                     .Where((item, index) => index == 0 || item.SelicRate != data.ElementAt(index - 1).SelicRate)
                                     .Select(x => x.Date.ToString("MM-yyyy"))
                                     .Distinct()
                                     .ToList();

            foreach(var changingMonth in changingMonths)
                Console.WriteLine(changingMonth);

            //
            var quarters = data!
                 .OrderBy(x => x.Date)
                 .Where(x => x.Date.Year >= 2016)
                 .GroupBy(x => Math.Ceiling(x.Date.Month / 3m) + "/" + x.Date.Year)
                 .Select(x => new
                 {
                     Quarter = x.Key,
                     Average = x.Average(x => x.SelicRate)
                 })
                 .OrderBy(x => DateTime.Parse(x.Quarter))
                 .ToList();

            //foreach (var quarter in quarters)
            //{
            //    var averageSelicQuarter = Math.Round(quarter.Average(x => x.SelicRate), 2);
            //    Console.WriteLine($"{quarter.Key + 1}º quadrante: {averageSelicQuarter}.\n");
            //}

            Console.ReadKey();


        }
    }

    public class Selic
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("selic")]
        public double SelicRate { get; set; }
    }
}
