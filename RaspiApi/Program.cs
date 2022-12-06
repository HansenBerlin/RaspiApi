using System.Collections;
using System.Device.Gpio;
using System.Drawing;
using rpi_ws281x;

int[] allowedPins = { 5, 6, 13, 18, 23, 24, 12, 33, 32 };
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

app.MapGet("/", () => "Minimal User Data API");


app.MapGet("/freq/{hex}/{led:int}", (string hex, int led) =>
{
    var settings = Settings.CreateDefaultSettings();
    var controller = settings.AddController(300, Pin.Gpio18, StripType.WS2812_STRIP);
    Color color = ColorTranslator.FromHtml("#" + hex);
    using (var rpi = new WS281x(settings))
    {
        controller.SetLED(led, color);
        rpi.Render();
    }
});


void Open(int pin)
{
    using var controller = new GpioController();
    controller.OpenPin(pin, PinMode.Output);
    controller.Write(pin, PinValue.High);
    
    
    //controller.ClosePin(pin);
}

void Close(int pin)
{
    using var controller = new GpioController();
    controller.OpenPin(pin, PinMode.Output);
    controller.Write(pin, PinValue.Low);
    //controller.ClosePin(pin);
}

bool IsAllowed(int pin)
{
    return allowedPins.Contains(pin);
}


app.Run();

