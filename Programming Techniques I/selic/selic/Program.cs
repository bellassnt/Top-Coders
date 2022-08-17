using Microsoft.Extensions.DependencyInjection;

namespace selic
{
    public class Program
    {
        public static void Main()
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<ISelic, Selic>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var selic = serviceProvider.GetService<ISelic>();

            var data = selic!.Load();

            #region Lowest and highest Brazil's interest rate between 2012 and 2022
            var lowestSelic = selic!.GetMin(data);
            var highestSelic = selic!.GetMax(data); 
            Console.WriteLine($"Lowest and highest Brazil's interest rate between 2012 and 2022: {lowestSelic} and {highestSelic}, respectively.\n");
            #endregion

            #region Mode(s) of Brazil's interest rate between 2012 and 2022
            var selicModes = selic.GetModes(data);

            Console.WriteLine("Mode(s) of Brazil's interest rate between 2012 and 2022: ");

            foreach (var mode in selicModes)
                Console.WriteLine(mode);
            #endregion

            #region Brazil's interest rate average between 2012 and 2022
            var averageSelic = selic!.GetAverage(data);

            Console.WriteLine($"\nBrazil's interest rate average between 2012 and 2022: {averageSelic}.");
            #endregion

            #region Months in which Brazil's interest rate has varied between 2012 and 2022
            Console.WriteLine("\nMonths in which Brazil's interest rate has varied between 2012 and 2022: ");

            var monthsInWhichSelicHasVaried = data.OrderBy(x => x.Date)
                .Where((item, index) => index == 0 || item.SelicRate != data.ElementAt(index - 1).SelicRate)
                .Select(x => x.Date!.ToString("MM-yyyy"));

            foreach (var month in monthsInWhichSelicHasVaried)
                Console.WriteLine(month);
            #endregion

            #region Brazil's interest rate average per quarter since 2016
            var quarters = data!
                 .OrderBy(x => x.Date)
                 .Where(x => x.Date.Year >= 2016)
                 .GroupBy(x => $"{Math.Ceiling(x.Date.Month / 3m)}T{x.Date.Year}")
                 .Select(x => new
                 {
                     Quarter = x.Key,
                     Average = Math.Round(x.Average(x => x.SelicRate), 2)
                 })
                 .ToList();

            Console.WriteLine("\nBrazil's interest rate average per quarter since 2016: ");
            foreach (var quarter in quarters)
                Console.WriteLine($"{quarter.Quarter}: {quarter.Average}");
            #endregion

            #region Lowest and highest Brazil's interest rate for each of Brazil's president between 2012 to 2022
            var dilmasGovernmentStart = new DateTime(2012, 1, 1);
            var dilmasGovernmentEnd = new DateTime(2016, 8, 31);
            var dilmasLowestSelic = data.Where(x => x.Date >= dilmasGovernmentStart && x.Date <= dilmasGovernmentEnd)
                .Min(x => x.SelicRate);
            var dilmasHighestSelic = data.Where(x => x.Date >= dilmasGovernmentStart && x.Date <= dilmasGovernmentEnd)
                .Max(x => x.SelicRate);

            Console.WriteLine("\nDilma's government: ");
            Console.WriteLine($"Lowest Selic rate: {dilmasLowestSelic}.");
            Console.WriteLine($"Highest Selic rate: {dilmasHighestSelic}.");

            var temersGovernmentStart = dilmasGovernmentEnd;
            var temersGovernmentEnd = new DateTime(2019, 1, 1);
            var temersLowestSelic = data.Where(x => x.Date >= temersGovernmentStart && x.Date <= temersGovernmentEnd)
                .Min(x => x.SelicRate);
            var temersHighestSelic = data.Where(x => x.Date >= temersGovernmentStart && x.Date <= temersGovernmentEnd)
                .Max(x => x.SelicRate);

            Console.WriteLine("\nTemer's government: ");
            Console.WriteLine($"Lowest Selic rate: {temersLowestSelic}.");
            Console.WriteLine($"Highest Selic rate: {temersHighestSelic}.");

            var bolsonarosGovernmentStart = temersGovernmentEnd;
            var bolsonarosGovernmentEnd = data.Last().Date;
            var bolsonarosLowestSelic = data.Where(x => x.Date >= bolsonarosGovernmentStart && x.Date <= bolsonarosGovernmentEnd)
                .Min(x => x.SelicRate);
            var bolsonarosHighestSelic = data.Where(x => x.Date >= bolsonarosGovernmentStart && x.Date <= bolsonarosGovernmentEnd)
                .Max(x => x.SelicRate);

            Console.WriteLine("\nBolsonaro's government: ");
            Console.WriteLine($"Lowest Selic rate: {bolsonarosLowestSelic}.");
            Console.WriteLine($"Highest Selic rate: {bolsonarosHighestSelic}.");
            #endregion

            #region Brazil's interest rate average growth rate per month since March 2021
            var filteredData = data.Where(x => x.Date > new DateTime(2021, 3, 1))
                .Select(x => x.SelicRate)
                .Distinct();

            var averageGrowthRate = filteredData.Zip(filteredData.Skip(1), (a, b) => b - a).Average();

            Console.WriteLine($"\nBrazil's interest rate average growth rate per month since March 2021: {Math.Round(averageGrowthRate, 2)} per month.");
            #endregion
        }
    }
}
