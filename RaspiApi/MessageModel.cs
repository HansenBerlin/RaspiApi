namespace RaspiApi;

public class ChatMessage
{
    public Dictionary<int, bool> ActiveLeds { get; set; } = new();
}