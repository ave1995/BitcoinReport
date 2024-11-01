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
        Interlocked.Increment(ref _connectedClients);
        lock (_lock)
        {
            if (_connectedClients == 1)
            {
                StartBroadcastBitcoinPrice();
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Interlocked.Decrement(ref _connectedClients);
        lock (_lock)
        {
            if (_connectedClients == 0)
            {
                StopBroadcastBitcoinPrice();
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    private void StartBroadcastBitcoinPrice()
    {
        if (_isBroadcasting)
            return;

        _timer = new Timer(async _ =>
            await _bitcoinService.BroadcastBitcoinResponseAsync(), null,
            TimeSpan.Zero, TimeSpan.FromSeconds(_configuration.GetValue<int>("BroadcastInterval")));
        _isBroadcasting = true;
    }

    private void StopBroadcastBitcoinPrice()
    {
        _timer?.Dispose();
        _timer = null;
        _isBroadcasting = false;
    }
}
