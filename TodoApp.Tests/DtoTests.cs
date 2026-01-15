using Xunit;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Tests
{
    // Test DTO validation and mapping
    public class DtoTests
    {
        [Fact]
        public void CreateTodoDto_WithValidData_IsCreatedSuccessfully()
        {
            // Arrange & Act
            var dto = new CreateTodoDto
            {
                Title = "New Task",
                Description = "Task Description",
                Priority = "high",
                DueDate = DateTime.Now.AddDays(7),
                IsCompleted = false
            };

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("New Task", dto.Title);
            Assert.Equal("Task Description", dto.Description);
            Assert.Equal("high", dto.Priority);
        }

        [Fact]
        public void UpdateTodoDto_WithValidData_IsCreatedSuccessfully()
        {
            // Arrange & Act
            var dto = new UpdateTodoDto
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Priority = "medium",
                DueDate = DateTime.Now.AddDays(3),
                IsCompleted = true
            };

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("Updated Task", dto.Title);
            Assert.True(dto.IsCompleted);
        }

        [Fact]
        public void TodoResponseDto_MapsFromEntity()
        {
            // Arrange
            var todo = new TodoItem("Test Task", "Description", DateTime.Now.AddDays(5), "high")
            {
                IsCompleted = true
            };

            // Act
            var dto = new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Priority = todo.Priority,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                DueDate = todo.DueDate
            };

            // Assert
            Assert.Equal(todo.Id, dto.Id);
            Assert.Equal(todo.Title, dto.Title);
            Assert.Equal(todo.Description, dto.Description);
            Assert.Equal(todo.Priority, dto.Priority);
            Assert.Equal(todo.IsCompleted, dto.IsCompleted);
        }

        [Fact]
        public void CreateTodoDto_AllowsNullDescription()
        {
            // Arrange & Act
            var dto = new CreateTodoDto
            {
                Title = "Task Without Description",
                Description = null,
                Priority = "low"
            };

            // Assert
            Assert.NotNull(dto);
            Assert.Null(dto.Description);
        }

        [Fact]
        public void CreateTodoDto_AllowsNullDueDate()
        {
            // Arrange & Act
            var dto = new CreateTodoDto
            {
                Title = "Task Without DueDate",
                Priority = "medium",
                DueDate = null
            };

            // Assert
            Assert.NotNull(dto);
            Assert.Null(dto.DueDate);
        }
    }
}
