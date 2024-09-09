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
        // Demonstrating DTO approach
        // Data Transfer Object to restrict data that can be input and returned
        public string? Secret { get; set; }
    }
}
