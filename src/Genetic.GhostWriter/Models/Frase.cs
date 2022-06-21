using System.Text.Json.Serialization;

namespace Genetic.GhostWriter.Models;

public class Frase
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("texto")]
    public string Texto { get; set; }
}