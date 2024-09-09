using Microsoft.EntityFrameworkCore;

namespace TodoApi
{
    /// <summary>
    /// Database Context
    /// </summary>
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<Todo> Todos => Set<Todo>();
    }
}
