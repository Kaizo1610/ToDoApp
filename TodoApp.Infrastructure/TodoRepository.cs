using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.Infrastructure
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _context.Todos.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.Todos.FindAsync(id);
            if (item != null)
            {
                _context.Todos.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _context.Todos
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TodoItem?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty", nameof(id));

            return await _context.Todos.FindAsync(id);
        }

        public async Task UpdateAsync(TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var existingItem = await _context.Todos.FindAsync(item.Id);
            if (existingItem == null)
                throw new KeyNotFoundException($"Todo with ID {item.Id} not found");

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }
    }
}
