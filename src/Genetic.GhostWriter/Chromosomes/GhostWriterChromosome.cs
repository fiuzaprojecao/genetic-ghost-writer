using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace Genetic.GhostWriter.Chromosomes;

public sealed class GhostWriterChromosome : ChromosomeBase
{
    private readonly IList<string> _palavras;

    public GhostWriterChromosome(IList<string> palavras,
                                 int quantidadePalavasTexto): base(quantidadePalavasTexto)
    {
        _palavras = palavras;

        for (var i = 0; i < quantidadePalavasTexto; i++)
            ReplaceGene(i, GenerateGene(i));
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(_palavras[RandomizationProvider.Current.GetInt(0, _palavras.Count)]);
    }

    public override IChromosome CreateNew()
    {
        return new GhostWriterChromosome(_palavras, Length);
    }

    public string BuildText()
    {
        return string.Join(" ", GetGenes().Select(g => g.Value.ToString()).ToArray());
    }
}