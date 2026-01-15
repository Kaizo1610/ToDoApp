using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoRepository _repo;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoRepository repo, ILogger<TodosController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>
        /// Get all todos from the database
        /// </summary>
        [HttpGet]
        [Produces(typeof(IEnumerable<TodoResponseDto>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var todos = await _repo.GetAllAsync();
                var dtos = todos.Select(MapToDto).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oops, couldn't fetch todos");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving todos" });
            }
        }

        /// <summary>
        /// Get a specific todo by its ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(TodoResponseDto))]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ID format" });

            try
            {
                var todo = await _repo.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(new { message = $"Todo with ID {id} not found" });

                return Ok(MapToDto(todo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting todo {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving todo" });
            }
        }

        /// <summary>
        /// Create a brand new todo
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces(typeof(TodoResponseDto))]
        public async Task<IActionResult> Create([FromBody] CreateTodoDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Title))
                    return BadRequest(new { message = "Title is required" });

                var todo = new TodoItem(
                    createDto.Title,
                    createDto.Description,
                    createDto.DueDate,
                    createDto.Priority ?? "medium"
                )
                {
                    IsCompleted = createDto.IsCompleted
                };

                await _repo.AddAsync(todo);
                var dto = MapToDto(todo);

                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Something's wrong with the input");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't create the todo");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error creating todo" });
            }
        }

        /// <summary>
        /// Update an existing todo - change anything you want
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoDto updateDto)
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ID format" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var todo = await _repo.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(new { message = $"Todo with ID {id} not found" });

                // Update all the fields
                todo.Update(
                    updateDto.Title,
                    updateDto.Description,
                    updateDto.DueDate,
                    updateDto.Priority ?? "medium",
                    updateDto.IsCompleted
                );

                await _repo.UpdateAsync(todo);
                return Ok(MapToDto(todo));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data in the update");
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Couldn't find todo {id}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Oops, couldn't update todo {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error updating todo" });
            }
        }

        /// <summary>
        /// Delete a todo - goodbye task!
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ID format" });

            try
            {
                var todo = await _repo.GetByIdAsync(id);
                if (todo == null)
                    return NotFound(new { message = $"Todo with ID {id} not found" });

                // Delete it
                await _repo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Couldn't delete todo {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error deleting todo" });
            }
        }

        /// <summary>
        /// Convert database todo to API response
        /// </summary>
        private TodoResponseDto MapToDto(TodoItem todo)
        {
            return new TodoResponseDto
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
        }
    }
}
