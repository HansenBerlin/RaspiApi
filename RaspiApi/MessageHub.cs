using Microsoft.AspNetCore.SignalR;
using PeriodicSystem.Core;

namespace RaspiApi;

public class MessageHub : Hub, IWebsocketEndpoints
{
    
    private bool _isBusy;
    private readonly EndpointIdentifier _endpointIdentifier;
    private readonly IRaspberryConnector _connector;
    
    public MessageHub(EndpointIdentifier endpointIdentifier, IRaspberryConnector connector)
    {
        _endpointIdentifier = endpointIdentifier;
        _connector = connector;
    }

    
    
    public async Task SendActiveLeds(Dictionary<int, string> activeLeds)
    {
        if (_isBusy) return;
        _isBusy = true;
        foreach (var kv in activeLeds)
        {
            _connector.SetLed(kv.Key, kv.Value);
        }
        _connector.Render();
        _isBusy = false;
        var leds = _connector.GetLedColors();
        var countOn = leds.Count(x => x.Value != "#000000");
        Console.WriteLine($"Active LEDs: {countOn}");
        await Clients.Others.SendAsync(_endpointIdentifier.SendLedsReceive, leds);
    }

    public async Task RunAnimation(LedAnimationRequest animationRequest)
    {
        if (_isBusy) return;
        _isBusy = true;
        _connector.SetAllOff();
        var range = Enumerable.Range(0, _connector.LedCount).ToList();
        if (animationRequest.Randomize)
        {
            Random random = new Random();
            range = range.OrderBy(x => random.Next()).ToList();
        }
        
        foreach (var rn in range)
        {
            _connector.SetLed(rn, animationRequest.LedColors[rn]);
            _connector.Render();
            await Task.Delay(animationRequest.Delay);
        }
        _isBusy = false;
        
        var leds = _connector.GetLedColors();
        var countOn = leds.Count(x => x.Value != "#000000");
        Console.WriteLine($"Active LEDs: {countOn}");
        await Clients.All.SendAsync("ActiveLedsReceived", leds);
    }
}
