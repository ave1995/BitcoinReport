namespace Grid.Loaders;

public interface ICurrencyLoader
{
    Task<decimal> LoadEurToCzkCurrencyAsync();
}