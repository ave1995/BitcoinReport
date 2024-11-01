namespace Grid.Loaders;

public interface IBitcoinPriceLoader
{
    Task<decimal> LoadEurPriceAsync();
}