namespace selic
{
    public interface ISelic
    {
        List<Selic> Load();

        double GetMin(List<Selic> data);

        double GetMax(List<Selic> data);

        IEnumerable<double> GetModes(List<Selic> data);

        double GetAverage(List<Selic> data);
    }
}