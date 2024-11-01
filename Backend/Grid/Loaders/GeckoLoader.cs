using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grid.Loaders;

public class GeckoLoader : IBitcoinPriceLoader
{
    private readonly HttpClient _httpClient;

    public GeckoLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<decimal> LoadEurPriceAsync()
    {

        //TODO: await _httpClient.GetAsync: Měl bych se podívat do Headeru a zjistit za jak dlouho se mám znovu dotázat
        var response = await _httpClient.GetStringAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=eur");

        var bitcoinGecko = JsonSerializer.Deserialize<BitcoinGecko>(response);
        if (bitcoinGecko?.Bitcoin?.Eur == null)
            throw new InvalidOperationException("Invalid response: Bitcoin price not found.");

        return bitcoinGecko.Bitcoin.Eur;

    }

    private record BitcoinGecko
    {
        [JsonPropertyName("bitcoin")]
        public required BitconPrice Bitcoin { get; set; }
    }

    private record BitconPrice
    {
        [JsonPropertyName("eur")]
        public decimal Eur { get; set; }
    }
}


