
using System.Text.Json.Serialization;

//Mohl jsem vybrat lepší úmístění
namespace Grid.Services;

public record BitcoinResponse
{
    [JsonPropertyName("price")]
    public decimal Price { get; init; }
    [JsonPropertyName("time")]
    public string Time { get; init; }

    public BitcoinResponse(decimal price, string time)
    {
        Price = price;
        Time = time;
    }
}

public record BitcoinResponseGet : BitcoinResponse
{
    [JsonPropertyName("note")]
    public string Note { get; init; }

    [JsonPropertyName("guid")]
    public Guid Guid { get; init; }

    public BitcoinResponseGet(decimal price, string time, string note, Guid guid) : base(price, time)
    {
        Note = note;
        Guid = guid;
    }
}

public class BitcoinUpdateNoteRequest
{
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}


