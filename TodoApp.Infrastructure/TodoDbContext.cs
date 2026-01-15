using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure
{
    public class TodoDbContext : DbContext
    {
        public DbSet<TodoItem> Todos { get; set; } = null!;

        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasDefaultValue("medium")
                    .HasMaxLength(10);

                entity.Property(e => e.IsCompleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("datetime('now')");

                entity.Property(e => e.UpdatedAt);

                entity.Property(e => e.DueDate);

                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.Priority);
            });
        }
    }
}
