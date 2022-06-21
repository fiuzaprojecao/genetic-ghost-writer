using System.Text.Json;
using Genetic.GhostWriter.Chromosomes;
using Genetic.GhostWriter.Fitness;
using Genetic.GhostWriter.Models;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace Genetic.GhostWriter.Services;

public class GhostWriterService
{
    private const string BASE_URL_API = "http://127.0.0.1:5000";
    
    private const int MIN_POPULATION_SIZE = 100;
    private const int MAX_POPULATION_SIZE = 200;

    private const int FITNESS_STAGNATION = 100;

    private const int QTY_PALAVRAS_TEXTO = 5;
    
    private readonly HttpClient _httpClient;
        
    private List<string> _frases;
    private List<string> _palavras;

    public GhostWriterService()
    {
        _frases = new List<string>();
        _palavras = new List<string>();

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BASE_URL_API);
    }

    public async Task CarregarAsync()
    {
        var frases = await ObterFrasesAsync();
            
        foreach (var frase in frases)
        {
            _frases.Add(frase.Texto);
            _palavras.AddRange(frase.Texto.Split(' '));
        }

        _palavras = _palavras.Select(w => w.RemovePunctuations()).Distinct().OrderBy(w => w).ToList();
    }

    private async Task<IEnumerable<Frase>> ObterFrasesAsync()
    {
        var response = await _httpClient.GetAsync("api/frases");

        if (!response.IsSuccessStatusCode) 
            throw new ApplicationException("Nao foi possivel obter as frases da api.");
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<Frase>>(json)!;
    }

    public void Run()
    {
        var geneticAlgorithm = CreateGeneticAlgorithm();
        var terminationName = geneticAlgorithm.Termination.GetType().Name;

        geneticAlgorithm.GenerationRan += delegate
        {
            var bestChromosome = geneticAlgorithm.Population.BestChromosome;
            
            PrintHeader();
            Console.WriteLine("Termination: {0}", terminationName);
            Console.WriteLine("Generations: {0}", geneticAlgorithm.Population.GenerationsNumber);
            Console.WriteLine("Fitness: {0,10}", bestChromosome.Fitness);
            Console.WriteLine("Tempo: {0}", geneticAlgorithm.TimeEvolving);
            Console.WriteLine("Velocidade (gen/sec): {0:0.0000}", geneticAlgorithm.Population.GenerationsNumber / geneticAlgorithm.TimeEvolving.TotalSeconds);
            PrintResultado(bestChromosome);
        };
        
        ConfigGeneticAlgorithm(geneticAlgorithm);
        geneticAlgorithm.Start();
    }

    public static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Genetic - Ghost Writer - Projeção");
        Console.WriteLine("GABRIEL ALEXANDRE MENEZES FIUZA - 201820821");
        Console.WriteLine("THIAGO ESCOVEDO DA COSTA - 201820098");
        Console.ResetColor();
    }

    private static void ConfigGeneticAlgorithm(GeneticAlgorithm geneticAlgorithm)
    {
        geneticAlgorithm.TaskExecutor = new ParallelTaskExecutor
        {
            MinThreads = 25,
            MaxThreads = 50
        };
    }

    private IFitness CreateFitness()
    {
        return new GhostWriterFitness(text =>
        {
            var minDistance = _frases.Min(q => LevenshteinDistance(q, text));

            return 1 - minDistance / 100f;
        });
    }

    private Population CreatePopulation()
    {
        return new Population(MIN_POPULATION_SIZE, MAX_POPULATION_SIZE, CreateChromosome())
        {
            GenerationStrategy = new PerformanceGenerationStrategy()
        };
    }

    private GeneticAlgorithm CreateGeneticAlgorithm()
    {
        var selection = new EliteSelection();;
        var crossover = new UniformCrossover();
        var mutation = new UniformMutation(true);
        var fitness = CreateFitness();
        var population = CreatePopulation();
        
        return new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
        {
            Termination = new FitnessStagnationTermination(FITNESS_STAGNATION)
        };
    }

    private IChromosome CreateChromosome()
    {
        return new GhostWriterChromosome(_palavras, QTY_PALAVRAS_TEXTO);
    }

    private static void PrintResultado(IChromosome bestChromosome)
    {
        var c = bestChromosome as GhostWriterChromosome;
        Console.WriteLine("Texto: {0}", c.BuildText());
    }

    private int LevenshteinDistance(string s, string t)
    {
        // degenerate cases
        if (s == t) return 0;

        if (s.Length == 0) return t.Length;

        if (t.Length == 0) return s.Length;

        // create two work vectors of integer distances
        var v0 = new int[t.Length + 1];
        var v1 = new int[t.Length + 1];

        // initialize v0 (the previous row of distances)
        // this row is A[0][i]: edit distance for an empty s
        // the distance is just the number of characters to delete from t
        for (var i = 0; i < v0.Length; i++) v0[i] = i;

        for (var i = 0; i < s.Length; i++)
        {
            // calculate v1 (current row distances) from the previous row v0

            // first element of v1 is A[i+1][0]
            //   edit distance is delete (i+1) chars from s to match empty t
            v1[0] = i + 1;

            // use formula to fill in the rest of the row
            for (var j = 0; j < t.Length; j++)
            {
                var cost = s[i] == t[j] ? 0 : 1;
                v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
            }

            // copy v1 (current row) to v0 (previous row) for next iteration
            for (var j = 0; j < v0.Length; j++) v0[j] = v1[j];
        }

        return v1[t.Length];
    }
}