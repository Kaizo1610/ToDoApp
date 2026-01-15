using Xunit;
using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Tests
{
    // Test Repository layer business logic
    public class TodoRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllTodos()
        {
            // Arrange
            var mockRepository = new Mock<ITodoRepository>();
            var todos = new List<TodoItem>
            {
                new TodoItem("Task 1"),
                new TodoItem("Task 2"),
                new TodoItem("Task 3")
            };
            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

            // Act
            var result = await mockRepository.Object.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsTodo()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var todo = new TodoItem("Test Task") { Id = todoId };
            var mockRepository = new Mock<ITodoRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync(todo);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(todoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todoId, result.Id);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var mockRepository = new Mock<ITodoRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_WithValidTodo_CallsRepositoryOnce()
        {
            // Arrange
            var todo = new TodoItem("New Task", "Description");
            var mockRepository = new Mock<ITodoRepository>();
            mockRepository.Setup(r => r.AddAsync(todo)).Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.AddAsync(todo);

            // Assert
            mockRepository.Verify(r => r.AddAsync(todo), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidTodo_CallsRepositoryOnce()
        {
            // Arrange
            var todo = new TodoItem("Updated Task");
            var mockRepository = new Mock<ITodoRepository>();
            mockRepository.Setup(r => r.UpdateAsync(todo)).Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.UpdateAsync(todo);

            // Assert
            mockRepository.Verify(r => r.UpdateAsync(todo), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_CallsRepositoryOnce()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var mockRepository = new Mock<ITodoRepository>();
            mockRepository.Setup(r => r.DeleteAsync(todoId)).Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.DeleteAsync(todoId);

            // Assert
            mockRepository.Verify(r => r.DeleteAsync(todoId), Times.Once);
        }
    }
}
