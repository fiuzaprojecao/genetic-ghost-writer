using Genetic.GhostWriter.Services;

var ghostWriterService = new GhostWriterService();
await ghostWriterService.CarregarAsync();

Console.SetError(TextWriter.Null);
GhostWriterService.PrintHeader();

try
{
    ghostWriterService.Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine();
    Console.WriteLine("Error: {0}", ex.Message);
    Console.ResetColor();
    Console.ReadKey();
    return;
}

Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine();
Console.WriteLine("Evoluiu.");
Console.ResetColor();
Console.ReadKey();
ghostWriterService.Run();