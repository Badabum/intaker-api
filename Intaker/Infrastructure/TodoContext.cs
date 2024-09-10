using Microsoft.EntityFrameworkCore;

namespace Intaker.Infrastructure;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options): base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Todo>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Todo>()
            .Property(t => t.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Todo>()
            .Property(t => t.Name)
            .HasMaxLength(1000);
        modelBuilder
            .Entity<Todo>()
            .Property(t => t.Description)
            .HasMaxLength(4000);
        modelBuilder.Entity<Todo>()
            .HasIndex(t => t.AssignedTo);
    }
}