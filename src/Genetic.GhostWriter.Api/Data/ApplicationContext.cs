using Genetic.GhostWriter.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Genetic.GhostWriter.Api.Data;

public class ApplicationContext : DbContext
{
    public DbSet<Frase> Frases { get; set; }
    
    public ApplicationContext(DbContextOptions options) : base(options)
    {
        
    }
}