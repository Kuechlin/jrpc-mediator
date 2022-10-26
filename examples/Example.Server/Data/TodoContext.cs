using Example.Contract.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.Server.Data;

public class TodoContext : DbContext
{
    public DbSet<TodoModel> Todos { get; set; }

    public TodoContext(DbContextOptions options) : base(options) { }
}