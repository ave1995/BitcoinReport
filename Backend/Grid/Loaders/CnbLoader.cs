
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grid.Loaders;

public class CnbLoader : ICurrencyLoader
{
    private readonly HttpClient _httpClient;

    public CnbLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<decimal> LoadEurToCzkCurrencyAsync()
    {
        var response = await _httpClient.GetStringAsync("https://api.cnb.cz/cnbapi/exrates/daily");

        var ratesResponse = JsonSerializer.Deserialize<RatesCNB>(response);

        var eurRate = (ratesResponse?.Rates.FirstOrDefault(r => r.CurrencyCode == "EUR")?.Rate)
                            ?? throw new InvalidDataException("I couldn't get EUR rate from cnapi!");
        return eurRate;
    }

    private record RatesCNB
    {
        [JsonPropertyName("rates")]
        public required RateItem[] Rates { get; set; }
    }

    private record RateItem
    {
        [JsonPropertyName("currencyCode")]
        public required string CurrencyCode { get; set; }
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }
}