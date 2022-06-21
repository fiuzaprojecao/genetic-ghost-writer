using Genetic.GhostWriter.Chromosomes;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace Genetic.GhostWriter.Fitness;

public class GhostWriterFitness : IFitness
{
    private readonly Func<string, double> _funcEvoluir;

    public GhostWriterFitness(Func<string, double> funcEvoluir)
    {
        ExceptionHelper.ThrowIfNull("funcEvoluir", funcEvoluir);
        _funcEvoluir = funcEvoluir;
    }

    public double Evaluate(IChromosome chromosome)
    {
        var ghostWriterChromosome = chromosome as GhostWriterChromosome;
        var text = ghostWriterChromosome.BuildText();

        return _funcEvoluir(text);
    }
}