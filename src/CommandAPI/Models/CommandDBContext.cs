using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Models
{
    public class CommandDBContext : DbContext
    {
        public CommandDBContext(DbContextOptions<CommandDBContext> options) : base(options)
        {
            
        }

        public DbSet<Command> CommandItems { get; set; }

    }
}