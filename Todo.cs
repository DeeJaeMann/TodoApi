namespace TodoApi
{
    /// <summary>
    /// Model for app
    /// </summary>
    public class Todo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
