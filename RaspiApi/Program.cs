using System.Drawing;
using rpi_ws281x;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

const int count = 145;
var settings = Settings.CreateDefaultSettings();
var controller = settings.AddController(count, Pin.Gpio18, StripType.WS2812_STRIP);
var rpi = new WS281x(settings);

app.MapPost("/set", (Dictionary<int, string> json) =>
{
    foreach (var kv in json)
    {
        var color = ColorTranslator.FromHtml(kv.Value);
        controller.SetLED(kv.Key, color);
    }
    rpi.Render();
});

app.MapGet("/active", () =>
{
    Dictionary<int, bool> activeLeds = new();
    var leds = controller.LEDs.ToList();
    for (int i = 0; i < leds.Count; i++)
    {
        bool isOn = ColorTranslator.ToHtml(leds[i].Color) != "#000000";
        activeLeds.Add(i, isOn);
    }

    return activeLeds;
});

app.MapGet("/colors", () =>
{
    Dictionary<int, string> activeLeds = new();
    var leds = controller.LEDs.ToList();
    for (int i = 0; i < leds.Count; i++)
    {
        var color = ColorTranslator.ToHtml(leds[i].Color);
        activeLeds.Add(i, color);
    }

    return activeLeds;
});

app.MapPost("/shuffle", async (Shuffle json) =>
{
    var color = ColorTranslator.FromHtml(json.Color);
    var range = Enumerable.Range(0, 145).ToList();
    Random random = new Random();
    range = range.OrderBy(x => random.Next()).ToList();

    foreach (var rn in range)
    {
        controller.SetLED(rn, color);
        rpi.Render();
        await Task.Delay(json.Delay);
    }
});

app.MapPost("/alloff", () =>
{
    var color = ColorTranslator.FromHtml("#" + 000000);
    for(int i = 0; i < count; i++)
    {
        controller.SetLED(i, color);
    }
    rpi.Render();
});

app.Run();

public record Shuffle(int Delay, string Color);


