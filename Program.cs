using Microsoft.EntityFrameworkCore;

namespace TodoApi
{
    /// <summary>
    /// Todo API example
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main program entry point
        /// </summary>
        /// <param name="args">Command line Args</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add database context to the dependency injection container and enable displaying database-related exceptions
            builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            var app = builder.Build();

            /* Individually set endpoints
             * 
            // Get Endpoints
            app.MapGet("/todoitems", async (TodoDb db) => 
                await db.Todos.ToListAsync());

            app.MapGet("/todoitems/complete", async (TodoDb db) =>
                await db.Todos.Where(t => t.IsComplete).ToListAsync());

            app.MapGet("todoitems/{id}", async (int id, TodoDb db) =>
                await db.Todos.FindAsync(id)
                    is Todo todo
                        ? Results.Ok(todo)
                        : Results.NotFound());

            // Post Endpoints

            // Data is added to the in-memory database
            app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/todoitems/{todo.Id}", todo);
            });

            // Put Endpoints
            app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return Results.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // Delete Endpoints
            app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
            {
                if (await db.Todos.FindAsync(id) is Todo todo)
                {
                    db.Todos.Remove(todo);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            });
            */

            // Sets up the group to use the URL prefix /todoitems
            RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

            /* Defining endpoints using Results
             * 
            // Get Endpoints
            todoItems.MapGet("/", async (TodoDb db) =>
                await db.Todos.ToListAsync());

            todoItems.MapGet("/complete", async (TodoDb db) =>
                await db.Todos.Where(task => task.IsComplete).ToListAsync());

            todoItems.MapGet("/{id}", async (int id, TodoDb db) =>
                await db.Todos.FindAsync(id)
                    is Todo todo
                        ? Results.Ok(todo)
                        : Results.NotFound());

            // Post Endpoints
            todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/todoitems/{todo.Id}", todo);
            });

            // Put Endpoints
            todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return Results.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // Delete Endpoints
            todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
            {
                if (await db.Todos.FindAsync(id) is Todo todo)
                {
                    db.Todos.Remove(todo);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            });
            */

            // Define endpoints using TypedResults
            // Improves testability and automatically returns the response type metadata for OpenAPI
            // to describe the endpoint
            // Calls methods instead of lambdas

            todoItems.MapGet("/", GetAllTodos);
            todoItems.MapGet("/complete", GetCompleteTodos);
            todoItems.MapGet("/{id}", GetTodo);
            todoItems.MapPost("/", CreateTodo);
            todoItems.MapPut("/{id}", UpdateTodo);
            todoItems.MapDelete("/{id}", DeleteTodo);

            app.Run();

            /* Method definitions without DTO
            // Endpoint methods
            static async Task<IResult> GetAllTodos(TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.ToArrayAsync());
            }

            static async Task<IResult> GetCompleteTodos(TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.Where(task => task.IsComplete).ToListAsync());
            }

            static async Task<IResult> GetTodo(int id, TodoDb db)
            {
                return await db.Todos.FindAsync(id)
                    is Todo todo
                        ? TypedResults.Ok(todo)
                        : TypedResults.NotFound();
            }

            static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return TypedResults.Created($"/todoitems/{todo.Id}", todo);
            }

            static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return TypedResults.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;

                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            static async Task<IResult> DeleteTodo(int id, TodoDb db)
            {
                if(await db.Todos.FindAsync(id) is Todo todo)
                {
                    db.Todos.Remove(todo);
                    await db.SaveChangesAsync();
                    return TypedResults.NoContent();
                }
                return TypedResults.NotFound();
            }
            */

            // Methods using DTO
            // Hides the secret field
            // Endpoint methods
            static async Task<IResult> GetAllTodos(TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
            }

            static async Task<IResult> GetCompleteTodos(TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.Where(task => task.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
            }

            static async Task<IResult> GetTodo(int id, TodoDb db)
            {
                return await db.Todos.FindAsync(id)
                    is Todo todo
                        ? TypedResults.Ok(new TodoItemDTO(todo))
                        : TypedResults.NotFound();
            }

            static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
            {
                var todoItem = new Todo
                {
                    IsComplete = todoItemDTO.IsComplete,
                    Name = todoItemDTO.Name
                };

                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();

                todoItemDTO = new TodoItemDTO(todoItem);

                return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
            }

            static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return TypedResults.NotFound();

                todo.Name = todoItemDTO.Name;
                todo.IsComplete = todoItemDTO.IsComplete;

                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            static async Task<IResult> DeleteTodo(int id, TodoDb db)
            {
                if (await db.Todos.FindAsync(id) is Todo todo)
                {
                    db.Todos.Remove(todo);
                    await db.SaveChangesAsync();
                    return TypedResults.NoContent();
                }
                return TypedResults.NotFound();
            }
        }
    }
}
