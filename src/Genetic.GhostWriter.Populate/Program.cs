using System.Text;
using System.Text.Json;
using Genetic.GhostWriter.Populate;

const string BASE_URL_API = "http://127.0.0.1:5000";

var texto = await File.ReadAllTextAsync("piadas.txt");
var linhas = texto.Split("\n");

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(BASE_URL_API);

var piadas = linhas.Select(p => new Frase(p));

foreach (var piada in piadas)
{
    Console.WriteLine($"Inserindo piada: {piada.Texto}.");
    var json = JsonSerializer.Serialize(piada);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    await httpClient.PostAsync("api/frases", content);
}