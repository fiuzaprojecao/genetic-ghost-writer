namespace Genetic.GhostWriter.Api.Models;

public class Frase
{
    public Guid Id { get; set; }

    public string Texto { get; set; }

    public Frase()
    {
        Id = Guid.NewGuid();
    }
}