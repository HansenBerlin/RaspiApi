namespace RaspiApi;

public interface IRaspberryConnector
{
    int LedCount { get; }
    void SetLed(int led, string color);
    void SetAllOff();
    Dictionary<int, string> GetLedColors();
    void Render();
}