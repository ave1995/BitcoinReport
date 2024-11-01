using System.Text.Json;
using Grid.Hubs;
using Grid.Loaders;
using Microsoft.AspNetCore.SignalR;

namespace Grid.Services;

public class BitcoinService : IBitcoinService
{
    private readonly IBitcoinPriceLoader _bitcoinPriceLoader;
    private readonly ICurrencyLoader _currencyLoader;
    private readonly IHubContext<BitcoinHub> _bitcoinHub;

    public BitcoinService(IBitcoinPriceLoader bitcoinPriceLoader, ICurrencyLoader currencyLoader, IHubContext<BitcoinHub> bitcoinHub)
    {
        _bitcoinPriceLoader = bitcoinPriceLoader;
        _currencyLoader = currencyLoader;
        _bitcoinHub = bitcoinHub;
    }

    private async Task<decimal> LoadBitcoinPriceFromEurToCZK()
    {
        var bitcoinEur = await _bitcoinPriceLoader.LoadEurPriceAsync();
        var eurToCzk = await _currencyLoader.LoadEurToCzkCurrencyAsync();

        return bitcoinEur * eurToCzk;
    }

    private async Task<string?> GetBitcoinResponseFromEurToCZK()
    {
        var price = await LoadBitcoinPriceFromEurToCZK();

        var response = new BitcoinResponse(price, TimeOnly.FromDateTime(DateTime.Now).ToString("HH:mm:ss"));
        var bitcoinResponse = JsonSerializer.Serialize(response);
        return bitcoinResponse;
    }

    //Tohle se mi tady nelíbí
    public async Task BroadcastBitcoinResponseAsync()
    {
        var bitcoinResponse = await GetBitcoinResponseFromEurToCZK();
        await _bitcoinHub.Clients.All.SendAsync("ReceiveBitcoinPrice", bitcoinResponse);
    }
}
