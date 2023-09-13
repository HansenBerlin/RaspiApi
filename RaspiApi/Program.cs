using System.Drawing;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using RaspiApi;

//using rpi_ws281x;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
var connections = new List<WebSocket>();



app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var curName = context.Request.Query["name"];

        using var ws = await context.WebSockets.AcceptWebSocketAsync();

        connections.Add(ws);
        Console.WriteLine($"{curName} connction added");

        //await Broadcast($"BOT: {curName} joined the room. {connections.Count} users connected.");
        //await Broadcast(new ChatMessage
        //{
        //    Sender = "BOT",
        //    Content = $"{curName} joined the room. {connections.Count} users connected."
        //});
        try
        {
            await ReceiveMessage(ws,
                async (result, messageJson) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(messageJson);
                        await Broadcast(new ChatMessage
                        {
                            ActiveLeds = chatMessage.ActiveLeds,
                        });
                    }
                    else if (result.MessageType == WebSocketMessageType.Close 
                             || ws.State == WebSocketState.Aborted 
                             || ws.State == WebSocketState.CloseSent 
                             || ws.State == WebSocketState.CloseReceived)
                    {
                        //await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                        connections.Remove(ws);
                        Console.WriteLine($"{curName} connction removed");
                        //await Broadcast(new ChatMessage
                        //{
                        //    ActiveLeds = new Dictionary<int, bool>(),
                        //});
                    }
                });
        }
        catch (Exception e)
        {
            connections.Remove(ws);
            Console.WriteLine($"{curName} connction removed (err)");
            Console.WriteLine(e);
        }
        
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
{
    var buffer = new byte[1024 * 4];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text)
        {
            string messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            handleMessage(result, messageJson);
        }
        
        //var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //handleMessage(result, buffer);
    }
}

async Task Broadcast(ChatMessage  message)
{
    var messageJson = JsonSerializer.Serialize(message);
    var bytes = Encoding.UTF8.GetBytes(messageJson);

    foreach (var socket in connections)
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

app.MapGet("/stop", async () =>
{
    for (var i = 0; i < connections.Count; i++)
    {
        var ws = connections[i];
        if (ws.State == WebSocketState.Open)
        {
            Console.WriteLine("Closing connection");
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
            //connections.RemoveAt(i);
            //i--;
            //continue;
        }
        //var message = $"Hello from server. Client: {i}. Total: {webSockets.Count}";
        //byte[] buffer = Encoding.ASCII.GetBytes(message);
        //await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }
    return connections.Count;
});

Dictionary<int, bool> activeLeds = new();
app.MapGet("/setup", () =>
{
    var range = Enumerable.Range(0, 145).ToList();
    Random random = new Random();
    for (int i = 0; i < range.Count; i++)
    {
        activeLeds.Add(i, random.Next(0, 2) == 1);
    }
    return activeLeds;
});

app.MapPost("/set", (Dictionary<int, string> json) =>
{
    foreach (var kv in json)
    {
        bool isOn = kv.Value != "#000000";
        activeLeds[kv.Key] = isOn;
    }
});



const int count = 145;
//var settings = Settings.CreateDefaultSettings();
//var controller = settings.AddController(count, Pin.Gpio18, StripType.WS2812_STRIP);
//var rpi = new WS281x(settings);

//app.MapPost("/set", (Dictionary<int, string> json) =>
//{
//    foreach (var kv in json)
//    {
//        var color = ColorTranslator.FromHtml(kv.Value);
//        controller.SetLED(kv.Key, color);
//    }
//    rpi.Render();
//});
//
//app.MapGet("/active", () =>
//{
//    Dictionary<int, bool> activeLeds = new();
//    var leds = controller.LEDs.ToList();
//    for (int i = 0; i < leds.Count; i++)
//    {
//        bool isOn = ColorTranslator.ToHtml(leds[i].Color) != "#000000";
//        activeLeds.Add(i, isOn);
//    }
//
//    return activeLeds;
//});
//
//app.MapGet("/colors", () =>
//{
//    Dictionary<int, string> activeLeds = new();
//    var leds = controller.LEDs.ToList();
//    for (int i = 0; i < leds.Count; i++)
//    {
//        var color = ColorTranslator.ToHtml(leds[i].Color);
//        activeLeds.Add(i, color);
//    }
//
//    return activeLeds;
//});
//
//app.MapPost("/shuffle", async (Shuffle json) =>
//{
//    var color = ColorTranslator.FromHtml(json.Color);
//    var range = Enumerable.Range(0, 145).ToList();
//    Random random = new Random();
//    range = range.OrderBy(x => random.Next()).ToList();
//
//    foreach (var rn in range)
//    {
//        controller.SetLED(rn, color);
//        rpi.Render();
//        await Task.Delay(json.Delay);
//    }
//});
//
//app.MapPost("/alloff", () =>
//{
//    var color = ColorTranslator.FromHtml("#" + 000000);
//    for(int i = 0; i < count; i++)
//    {
//        controller.SetLED(i, color);
//    }
//    rpi.Render();
//});

await app.RunAsync();


public record Shuffle(int Delay, string Color);




