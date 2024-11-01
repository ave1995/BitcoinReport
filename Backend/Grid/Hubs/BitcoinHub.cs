using Grid.Services;
using Microsoft.AspNetCore.SignalR;

namespace Grid.Hubs;

public class BitcoinHub : Hub
{
    private readonly IBitcoinService _bitcoinService;
    private readonly IConfiguration _configuration;

    private static int _connectedClients = 0;
    private Timer? _timer;
    private readonly object _lock = new object();
    private static bool _isBroadcasting;

    public BitcoinHub(IBitcoinService bitcoinService, IConfiguration configuration)
    {
        _bitcoinService = bitcoinService;
        _configuration = configuration;
    }
    
    public override async Task OnConnectedAsync()
    {
        _connectedClients++;
        await base.OnConnectedAsync();

        if (_connectedClients == 1)
        {
            StartBroadcastBitcoinPrice();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClients--;
        await base.OnDisconnectedAsync(exception);

        if (_connectedClients == 0)
        {
            StopBroadcastBitcoinPrice();
        }
    }

    private void StartBroadcastBitcoinPrice()
    {
        lock (_lock)
        {
            if (_isBroadcasting)
                return;

            _timer = new Timer(async _ =>
                await _bitcoinService.BroadcastBitcoinResponseAsync(), null,
                TimeSpan.Zero, TimeSpan.FromSeconds(_configuration.GetValue<int>("BroadcastInterval")));
            _isBroadcasting = true;
        }
    }

    private void StopBroadcastBitcoinPrice()
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
            _isBroadcasting = false;
        }
    }
}
