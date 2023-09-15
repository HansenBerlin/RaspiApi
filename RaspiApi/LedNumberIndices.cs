namespace RaspiApi;

public class LedNumberIndices
{
    public List<int> DigitsAsc { get; } = new() { 90, 91, 92, 52, 53, 54, 15, 16, 17 };
    public List<int> DigitsDesc { get; } = new() { 83, 82, 81, 45, 44, 43 };
    public List<int> ZeroIndicesAsc { get; } = new() { 90, 91, 92, 52, 54, 15, 16, 17 };
    public List<int> OneIndicesAsc { get; } = new() { 92, 54, 17 };
    public List<int> TwoIndicesAsc { get; } = new() { 90, 91, 92, 52, 53, 54, 15, 16, 17 };
    public List<int> ThreeIndicesAsc { get; } = new() { 90, 91, 92, 54, 53, 52, 15, 16, 17 };
    public List<int> FourIndicesAsc { get; } = new() { 90, 52, 53, 54, 92, 17 };
    public List<int> FiveIndicesAsc { get; } = new() { 90, 91, 92, 54, 53, 52, 15, 16, 17 };
    public List<int> SixIndicesAsc { get; } = new() { 90, 91, 92, 54, 53, 52, 15, 16, 17 };
    public List<int> SevenIndicesAsc { get; } = new() { 90, 91, 92, 54, 17 };
    public List<int> EightIndicesAsc { get; } = new() { 90, 91, 92, 54, 53, 52, 15, 16, 17 };
    public List<int> NineIndicesAsc { get; } = new() { 90, 91, 92, 54, 53, 52, 15, 16, 17 };
    public List<int> ZeroIndicesDesc { get; } = new() { 83, 81, 45, 43 };
    public List<int> OneIndicesDesc { get; } = new() { 81, 43 };
    public List<int> TwoIndicesDesc { get; } = new() { 81, 45 };
    public List<int> ThreeIndicesDesc { get; } = new() { 81, 43 };
    public List<int> FourIndicesDesc { get; } = new() { 83, 81, 43 };
    public List<int> FiveIndicesDesc { get; } = new() { 83, 43 };
    public List<int> SixIndicesDesc { get; } = new() { 83, 45, 43 };
    public List<int> SevenIndicesDesc { get; } = new() { 81, 43 };
    public List<int> EightIndicesDesc { get; } = new() { 83, 81, 45, 43 };
    public List<int> NineIndicesDesc { get; } = new() { 83, 81, 43 };
    public List<int> SecondsModTenIndices { get; } = new() { 120, 121, 122, 123, 124, 125, 126, 127, 128, 129 };
    public List<int> SecondsModSixIndices { get; } = new() { 119, 115, 114, 87, 86, 49 };
}