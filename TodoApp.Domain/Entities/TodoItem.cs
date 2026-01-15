namespace TodoApp.Domain.Entities
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = "medium"; // low, medium, high
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        // Need this for EF Core
        public TodoItem() { }

        public TodoItem(string title, string? description = null, DateTime? dueDate = null, string priority = "medium")
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");
            
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
        }

        // Update the todo with new values
        public void Update(string title, string? description, DateTime? dueDate, string priority, bool isCompleted)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");
            
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            IsCompleted = isCompleted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkPending()
        {
            IsCompleted = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
