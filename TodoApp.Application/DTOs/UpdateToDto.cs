using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.DTOs
{
    public class UpdateTodoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^(low|medium|high)$", ErrorMessage = "Priority must be 'low', 'medium', or 'high'")]
        public string Priority { get; set; } = "medium";

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
