using Xunit;
using TodoApp.Domain.Entities;

namespace TodoApp.Tests
{
    // Test TodoItem domain entity validation
    public class TodoItemTests
    {
        [Fact]
        public void Constructor_WithValidTitle_CreatesSuccessfully()
        {
            // Arrange & Act
            var todo = new TodoItem("Test Task", "Test Description", null, "high");

            // Assert
            Assert.NotNull(todo);
            Assert.Equal("Test Task", todo.Title);
            Assert.Equal("Test Description", todo.Description);
            Assert.Equal("high", todo.Priority);
            Assert.False(todo.IsCompleted);
        }

        [Fact]
        public void Constructor_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new TodoItem("", "Description"));
        }

        [Fact]
        public void Constructor_WithNullTitle_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new TodoItem(null!, "Description"));
        }

        [Fact]
        public void Constructor_WithWhitespaceTitle_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new TodoItem("   ", "Description"));
        }

        [Fact]
        public void MarkCompleted_SetsIsCompletedTrue()
        {
            // Arrange
            var todo = new TodoItem("Test Task");

            // Act
            todo.MarkCompleted();

            // Assert
            Assert.True(todo.IsCompleted);
            Assert.NotNull(todo.UpdatedAt);
        }

        [Fact]
        public void MarkPending_SetsIsCompletedFalse()
        {
            // Arrange
            var todo = new TodoItem("Test Task");
            todo.IsCompleted = true;

            // Act
            todo.MarkPending();

            // Assert
            Assert.False(todo.IsCompleted);
            Assert.NotNull(todo.UpdatedAt);
        }

        [Fact]
        public void Update_WithValidData_UpdatesAllFields()
        {
            // Arrange
            var todo = new TodoItem("Original", "Original Desc");
            var newDueDate = DateTime.Now.AddDays(7);

            // Act
            todo.Update("Updated", "Updated Desc", newDueDate, "high", true);

            // Assert
            Assert.Equal("Updated", todo.Title);
            Assert.Equal("Updated Desc", todo.Description);
            Assert.Equal("high", todo.Priority);
            Assert.True(todo.IsCompleted);
            Assert.Equal(newDueDate, todo.DueDate);
            Assert.NotNull(todo.UpdatedAt);
        }

        [Fact]
        public void Update_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var todo = new TodoItem("Test Task");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                todo.Update("", "Description", null, "medium", false));
        }

        [Fact]
        public void Id_GeneratesUniqueGuid()
        {
            // Arrange & Act
            var todo1 = new TodoItem("Task 1");
            var todo2 = new TodoItem("Task 2");

            // Assert
            Assert.NotEqual(Guid.Empty, todo1.Id);
            Assert.NotEqual(Guid.Empty, todo2.Id);
            Assert.NotEqual(todo1.Id, todo2.Id);
        }

        [Fact]
        public void DefaultPriority_IsMedium()
        {
            // Arrange & Act
            var todo = new TodoItem("Test Task");

            // Assert
            Assert.Equal("medium", todo.Priority);
        }

        [Fact]
        public void CreatedAt_SetToUtcNow()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var todo = new TodoItem("Test Task");

            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.True(todo.CreatedAt >= beforeCreation && todo.CreatedAt <= afterCreation);
        }
    }
}
