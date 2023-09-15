namespace RaspiApi;

public class ClockBackgroundService : BackgroundService
{
    private readonly IRaspberryConnector _connector;
    private readonly LedNumberIndices _indices = new();
    
    public ClockBackgroundService(IRaspberryConnector connector)
    {
        _connector = connector;
    }
    
    public Task RunClock()
    {
        var now = DateTime.Now;
        var hour = now.Hour.ToString();
        var minute = now.Minute.ToString();
        var hourFirstDigit = hour.Length == 1 ? "0" : hour[0].ToString();
        var hourSecondDigit = hour.Length == 1 ? hour : hour[1].ToString();
        var minuteFirstDigit = minute.Length == 1 ? "0" : minute[0].ToString();
        var minuteSecondDigit = minute.Length == 1 ? minute : minute[1].ToString();
        var hourFirstIndices = GetIndices(hourFirstDigit);
        var hourSecondIndices = GetIndices(hourSecondDigit, 4);
        var minuteFirstIndices = GetIndices(minuteFirstDigit, 8);
        var minuteSecondIndices = GetIndices(minuteSecondDigit, 12);
        SetIndices(CombineIndices(), hourFirstIndices, "#FF0000");
        SetIndices(CombineIndices(4), hourSecondIndices, "#FF0000");
        SetIndices(CombineIndices(8), minuteFirstIndices, "#00FF00");
        SetIndices(CombineIndices(12), minuteSecondIndices, "#00FF00");
        SetSeconds();
        _connector.Render();
        return Task.CompletedTask;
    }

    private Task SetSeconds()
    {
        
        var now = DateTime.Now;
        var second = now.Second;
        if (second < 10)
        {
            for (int i = 0; i < _indices.SecondsModSixIndices.Count; i++)
            {
                _connector.SetLed(_indices.SecondsModTenIndices[i], "#000000");
            }
            
            for (int i = 0; i < _indices.SecondsModTenIndices.Count; i++)
            {
                bool isOn = i + 1 == second;
                var color = isOn ? "#FFFFFF" : "#000000";
                _connector.SetLed(_indices.SecondsModTenIndices[i], color);
            }
        }
        else
        {
            string secondString = second.ToString();
            for (int i = 0; i < _indices.SecondsModTenIndices.Count; i++)
            {
                bool isOn = i + 1 == int.Parse(secondString[1].ToString()) && second != 10;
                var color = isOn ? "#FFFFFF" : "#000000";
                _connector.SetLed(_indices.SecondsModTenIndices[i], color);
            }
            for (int i = 0; i < _indices.SecondsModSixIndices.Count; i++)
            {
                bool isOn = i + 1 == int.Parse(secondString[0].ToString());
                var color = isOn ? "#FFFFFF" : "#000000";
                _connector.SetLed(_indices.SecondsModSixIndices[i], color);
            }
        }
        return Task.CompletedTask;
    }

    private void SetIndices(List<int> baseDigits, List<int> indicesToSet, string htmlColor)
    {
        foreach (var index in baseDigits)
        {
            bool isOn = indicesToSet.Contains(index);
            var color = isOn ? htmlColor : "#000000";
            //Console.WriteLine($"Setting index {index} to {color}. IsOn: {isOn}");
            _connector.SetLed(index, color);
        }
    }

    private List<int> CombineIndices(int offset = 0)
    {
        var digits = _indices.DigitsAsc.Select(x => x + offset).ToList();
        digits.AddRange(_indices.DigitsDesc.Select(x => x - offset));
        return digits;
    }
    

    private List<int> GetIndices(string number, int offset = 0)
    {
        var indices = new List<int>();
        var baseDigitsAsc = new List<int>();
        var baseDigitsDsc = new List<int>();
        switch (number)
        {
            case "0":
                baseDigitsAsc = _indices.ZeroIndicesAsc;
                baseDigitsDsc = _indices.ZeroIndicesDesc;
                break;
            case "1":
                baseDigitsAsc = _indices.OneIndicesAsc;
                baseDigitsDsc = _indices.OneIndicesDesc;
                break;
            case "2":
                baseDigitsAsc = _indices.TwoIndicesAsc;
                baseDigitsDsc = _indices.TwoIndicesDesc;
                break;
            case "3":
                baseDigitsAsc = _indices.ThreeIndicesAsc;
                baseDigitsDsc = _indices.ThreeIndicesDesc;
                break;
            case "4":
                baseDigitsAsc = _indices.FourIndicesAsc;
                baseDigitsDsc = _indices.FourIndicesDesc;
                break;
            case "5":
                baseDigitsAsc = _indices.FiveIndicesAsc;
                baseDigitsDsc = _indices.FiveIndicesDesc;
                break;
            case "6":
                baseDigitsAsc = _indices.SixIndicesAsc;
                baseDigitsDsc = _indices.SixIndicesDesc;
                break;
            case "7":
                baseDigitsAsc = _indices.SevenIndicesAsc;
                baseDigitsDsc = _indices.SevenIndicesDesc;
                break;
            case "8":
                baseDigitsAsc = _indices.EightIndicesAsc;
                baseDigitsDsc = _indices.EightIndicesDesc;
                break;
            case "9":
                baseDigitsAsc = _indices.NineIndicesAsc;
                baseDigitsDsc = _indices.NineIndicesDesc;
                break;
        }

        indices = baseDigitsDsc.Select(x => x - offset).ToList();
        indices.AddRange(baseDigitsAsc.Select(x => x + offset));
        return indices;
    }

    private readonly CancellationTokenSource _stoppingCts = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunClock();
            await Task.Delay(200, stoppingToken);
        }
    }
    
    public void Stop()
    {
        _stoppingCts.Cancel();
    }
}