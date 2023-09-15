using System.Drawing;
using rpi_ws281x;

namespace RaspiApi;

public class RaspberryConnector : IRaspberryConnector
{
    public int LedCount { get; } = 145;
    private readonly Controller _controller;
    private readonly WS281x _rpi;
    
    public RaspberryConnector()
    {
        var settings = Settings.CreateDefaultSettings();
        _controller = settings.AddController(LedCount, Pin.Gpio18, StripType.WS2812_STRIP);
        _rpi = new WS281x(settings);
    }
    
    public void SetLed(int led, string color)
    {
        var colorObj = ColorTranslator.FromHtml(color);
        _controller.SetLED(led, colorObj);
    }

    public void SetAllOff()
    {
        var leds = _controller.LEDs.ToList();
        foreach (var led in leds)
        {
            led.Color = Color.Black;
        }
    }

    public Dictionary<int, string> GetLedColors()
    {
        Dictionary<int, string> activeLeds = new();
        var leds = _controller.LEDs.ToList();
        for (int i = 0; i < leds.Count; i++)
        {
            var color = ColorTranslator.ToHtml(leds[i].Color);
            activeLeds.Add(i, color);
        }

        return activeLeds;
    }
    
    public void Render()
    {
        _rpi.Render();
    }
}