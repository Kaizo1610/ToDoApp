import React, { useEffect, useState, useCallback } from "react";
import { getTodos, createTodo, updateTodo, deleteTodo } from "./api/todoApi";
import "./App.css";

function App() {
  const [todos, setTodos] = useState([]);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [priority, setPriority] = useState("medium");
  const [editingId, setEditingId] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [viewingTodo, setViewingTodo] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [filterStatus, setFilterStatus] = useState("all");
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState("dateAdded");

  useEffect(() => {
    loadTodos();
  }, []);

  const loadTodos = async () => {
    setLoading(true);
    try {
      const res = await getTodos();
      setTodos(res.data);
      setError("");
    } catch (err) {
      console.error(err);
      setError("Failed to load tasks. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const showSuccess = (message) => {
    setSuccessMessage(message);
    setTimeout(() => setSuccessMessage(""), 3000);
  };

  const resetForm = () => {
    setTitle("");
    setDescription("");
    setDueDate("");
    setPriority("medium");
    setEditingId(null);
    setShowModal(false);
    setError("");
  };

  const validateForm = () => {
    if (!title.trim()) {
      setError("Task title is required");
      return false;
    }
    if (title.trim().length < 3) {
      setError("Task title must be at least 3 characters");
      return false;
    }
    return true;
  };

  const submitTodo = async () => {
    if (!validateForm()) return;

    setLoading(true);
    try {
      const payload = {
        title: title.trim(),
        description: description.trim(),
        dueDate: dueDate || null,
        priority,
        isCompleted: editingId ? todos.find(t => t.id === editingId)?.isCompleted : false,
      };

      if (editingId) {
        await updateTodo(editingId, payload);
        showSuccess("Task updated successfully!");
      } else {
        await createTodo(payload);
        showSuccess("Task created successfully!");
      }

      resetForm();
      await loadTodos();
    } catch (err) {
      console.error(err);
      setError("Failed to save task. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const editTodo = (todo) => {
    setEditingId(todo.id);
    setTitle(todo.title);
    setDescription(todo.description || "");
    setDueDate(todo.dueDate ? todo.dueDate.split("T")[0] : "");
    setPriority(todo.priority || "medium");
    setShowModal(true);
    setError("");
  };

  const viewTodo = (todo) => {
    setViewingTodo(todo);
    setShowViewModal(true);
  };

  const toggleStatus = async (todo) => {
    setLoading(true);
    try {
      const updatedTodo = {
        title: todo.title,
        description: todo.description,
        dueDate: todo.dueDate,
        priority: todo.priority,
        isCompleted: !todo.isCompleted,
      };
      await updateTodo(todo.id, updatedTodo);
      showSuccess(todo.isCompleted ? "Task marked as incomplete" : "Task marked as complete");
      await loadTodos();
    } catch (err) {
      console.error(err);
      setError("Failed to update task status");
    } finally {
      setLoading(false);
    }
  };

  const removeTodo = async (id) => {
    if (!window.confirm("Are you sure you want to delete this task? This action cannot be undone.")) {
      return;
    }
    setLoading(true);
    try {
      await deleteTodo(id);
      showSuccess("Task deleted successfully!");
      await loadTodos();
    } catch (err) {
      console.error(err);
      setError("Failed to delete task");
    } finally {
      setLoading(false);
    }
  };

  const isOverdue = (todo) => {
    return !todo.isCompleted && todo.dueDate && new Date(todo.dueDate) < new Date();
  };

  const getTaskStats = () => {
    const total = todos.length;
    const completed = todos.filter(t => t.isCompleted).length;
    const overdue = todos.filter(isOverdue).length;
    return { total, completed, overdue };
  };

  const filteredAndSortedTodos = useCallback(() => {
    let filtered = todos;

    // Filter by status
    if (filterStatus === "completed") {
      filtered = filtered.filter(t => t.isCompleted);
    } else if (filterStatus === "pending") {
      filtered = filtered.filter(t => !t.isCompleted);
    } else if (filterStatus === "overdue") {
      filtered = filtered.filter(isOverdue);
    }

    // Filter by search term
    if (searchTerm.trim()) {
      const lowerSearch = searchTerm.toLowerCase();
      filtered = filtered.filter(t =>
        t.title.toLowerCase().includes(lowerSearch) ||
        (t.description && t.description.toLowerCase().includes(lowerSearch))
      );
    }

    // Sort
    if (sortBy === "dueDate") {
      filtered.sort((a, b) => {
        if (!a.dueDate) return 1;
        if (!b.dueDate) return -1;
        return new Date(a.dueDate) - new Date(b.dueDate);
      });
    } else if (sortBy === "priority") {
      const priorityOrder = { high: 0, medium: 1, low: 2 };
      filtered.sort((a, b) => 
        (priorityOrder[a.priority] || 1) - (priorityOrder[b.priority] || 1)
      );
    } else if (sortBy === "title") {
      filtered.sort((a, b) => a.title.localeCompare(b.title));
    }

    return filtered;
  }, [todos, filterStatus, searchTerm, sortBy]);

  const displayedTodos = filteredAndSortedTodos();
  const stats = getTaskStats();

  const getPriorityColor = (priority) => {
    switch (priority) {
      case "high":
        return "#dc3545";
      case "medium":
        return "#ffc107";
      case "low":
        return "#28a745";
      default:
        return "#6c757d";
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return "No date";
    
    // Convert to Malaysia timezone (UTC+8)
    const date = new Date(dateString);
    const malaysiaTime = new Date(date.toLocaleString('en-US', { timeZone: 'Asia/Kuala_Lumpur' }));
    
    const today = new Date();
    const malaysiaToday = new Date(today.toLocaleString('en-US', { timeZone: 'Asia/Kuala_Lumpur' }));
    
    const tomorrow = new Date(malaysiaToday);
    tomorrow.setDate(tomorrow.getDate() + 1);

    if (malaysiaTime.toDateString() === malaysiaToday.toDateString()) return "Today";
    if (malaysiaTime.toDateString() === tomorrow.toDateString()) return "Tomorrow";
    
    return malaysiaTime.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };

  const daysUntilDue = (dueDate) => {
    if (!dueDate) return null;
    
    // Convert to Malaysia timezone (UTC+8)
    const due = new Date(dueDate);
    const malaysiaDue = new Date(due.toLocaleString('en-US', { timeZone: 'Asia/Kuala_Lumpur' }));
    
    const today = new Date();
    const malaysiaToday = new Date(today.toLocaleString('en-US', { timeZone: 'Asia/Kuala_Lumpur' }));
    
    malaysiaToday.setHours(0, 0, 0, 0);
    malaysiaDue.setHours(0, 0, 0, 0);
    const diff = Math.floor((malaysiaDue - malaysiaToday) / (1000 * 60 * 60 * 24));
    return diff;
  };

  return (
    <div className="app-wrapper">
      {/* Header */}
      <header className="app-header">
        <div className="header-content">
          <div className="header-title">
            <h1>
              <span className="title-icon">✓</span> Task Manager
            </h1>
            <p className="subtitle">Stay organized and productive</p>
          </div>
          <button
            className="btn btn-primary btn-lg"
            onClick={() => setShowModal(true)}
            disabled={loading}
          >
            <span className="btn-icon">+</span> New Task
          </button>
        </div>
      </header>

      {/* Main Content */}
      <main className="app-container">
        {/* Messages */}
        {error && <div className="alert alert-error">{error}</div>}
        {successMessage && <div className="alert alert-success">{successMessage}</div>}

        {/* Stats Dashboard */}
        <section className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon stat-total">📋</div>
            <div className="stat-info">
              <div className="stat-label">Total Tasks</div>
              <div className="stat-value">{stats.total}</div>
            </div>
          </div>
          <div className="stat-card">
            <div className="stat-icon stat-completed">✓</div>
            <div className="stat-info">
              <div className="stat-label">Completed</div>
              <div className="stat-value">{stats.completed}</div>
            </div>
          </div>
          <div className="stat-card">
            <div className="stat-icon stat-overdue">⚠</div>
            <div className="stat-info">
              <div className="stat-label">Overdue</div>
              <div className="stat-value">{stats.overdue}</div>
            </div>
          </div>
          <div className="stat-card">
            <div className="stat-icon stat-pending">⏳</div>
            <div className="stat-info">
              <div className="stat-label">Pending</div>
              <div className="stat-value">{stats.total - stats.completed}</div>
            </div>
          </div>
        </section>

        {/* Controls */}
        <section className="controls-section">
          <div className="search-wrapper-full">
            <input
              type="text"
              className="search-input-full"
              placeholder="🔍 Search tasks by title or description..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </section>

        {/* Tasks List */}
        <section className="tasks-section">
          <h2 className="section-title">
            {filterStatus === "all" ? "All Tasks" : 
             filterStatus === "pending" ? "Pending Tasks" :
             filterStatus === "completed" ? "Completed Tasks" :
             "Overdue Tasks"}
            <span className="task-count">{displayedTodos.length}</span>
          </h2>

          {loading && <div className="loading">Loading tasks...</div>}

          {!loading && displayedTodos.length === 0 && (
            <div className="empty-state">
              <div className="empty-icon">📭</div>
              <p className="empty-text">No tasks found</p>
              <p className="empty-subtext">
                {searchTerm ? "Try a different search term" : "Create a new task to get started"}
              </p>
            </div>
          )}

          <div className="tasks-list">
            {displayedTodos.map((todo) => (
              <div
                key={todo.id}
                className={`task-card ${todo.isCompleted ? "completed" : ""} ${
                  isOverdue(todo) ? "overdue" : ""
                }`}
              >
                {/* Checkbox and Title */}
                <div className="task-header">
                  <div className="task-checkbox">
                    <input
                      type="checkbox"
                      checked={todo.isCompleted}
                      onChange={() => toggleStatus(todo)}
                      className="checkbox-input"
                    />
                  </div>

                  <div className="task-title-section">
                    <h3
                      className="task-title"
                      style={{
                        textDecoration: todo.isCompleted ? "line-through" : "none",
                        color: todo.isCompleted ? "#999" : "#000",
                      }}
                    >
                      {todo.title}
                    </h3>
                    {todo.description && (
                      <p className="task-description">{todo.description}</p>
                    )}
                  </div>

                  <div className="task-meta">
                    {todo.priority && (
                      <span
                        className="priority-badge"
                        style={{ backgroundColor: getPriorityColor(todo.priority) }}
                      >
                        {todo.priority.charAt(0).toUpperCase() + todo.priority.slice(1)}
                      </span>
                    )}
                  </div>
                </div>

                {/* Footer with Date and Actions */}
                <div className="task-footer">
                  <div className="task-date-info">
                    {todo.dueDate && (
                      <span
                        className={`date-label ${
                          isOverdue(todo) ? "overdue-label" : ""
                        } ${
                          daysUntilDue(todo.dueDate) === 0 ? "due-today-label" : ""
                        }`}
                      >
                        📅 {formatDate(todo.dueDate)}
                        {daysUntilDue(todo.dueDate) !== null &&
                          daysUntilDue(todo.dueDate) >= 0 && (
                            <span className="days-until">
                              {daysUntilDue(todo.dueDate) === 0
                                ? "(Today)"
                                : `(${daysUntilDue(todo.dueDate)} days)`}
                            </span>
                          )}
                      </span>
                    )}
                  </div>

                  <div className="task-actions">
                    <button
                      className="btn btn-view"
                      onClick={() => viewTodo(todo)}
                      title="View Details"
                    >
                      👁️
                    </button>
                    <button
                      className="btn btn-edit"
                      onClick={() => editTodo(todo)}
                      title="Edit Task"
                      disabled={loading}
                    >
                      ✎
                    </button>
                    <button
                      className="btn btn-delete"
                      onClick={() => removeTodo(todo.id)}
                      title="Delete Task"
                      disabled={loading}
                    >
                      🗑️
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </section>
      </main>

      {/* Add/Edit Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => resetForm()}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{editingId ? "Edit Task" : "Create New Task"}</h2>
              <button className="modal-close" onClick={() => resetForm()}>
                ✕
              </button>
            </div>

            <div className="modal-body">
              {error && <div className="alert alert-error">{error}</div>}

              <div className="form-group">
                <label htmlFor="title" className="form-label">
                  Task Title <span className="required">*</span>
                </label>
                <input
                  id="title"
                  type="text"
                  className="form-input"
                  placeholder="Enter task title"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  maxLength="100"
                />
                <div className="char-count">{title.length}/100</div>
              </div>

              <div className="form-group">
                <label htmlFor="description" className="form-label">
                  Description (Optional)
                </label>
                <textarea
                  id="description"
                  className="form-textarea"
                  placeholder="Enter task description"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  maxLength="500"
                  rows="3"
                />
                <div className="char-count">{description.length}/500</div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="dueDate" className="form-label">
                    Due Date
                  </label>
                  <input
                    id="dueDate"
                    type="date"
                    className="form-input"
                    value={dueDate}
                    onChange={(e) => setDueDate(e.target.value)}
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="priority" className="form-label">
                    Priority
                  </label>
                  <select
                    id="priority"
                    className="form-select"
                    value={priority}
                    onChange={(e) => setPriority(e.target.value)}
                  >
                    <option value="low">Low</option>
                    <option value="medium">Medium</option>
                    <option value="high">High</option>
                  </select>
                </div>
              </div>
            </div>

            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => resetForm()}
                disabled={loading}
              >
                Cancel
              </button>
              <button
                className="btn btn-primary"
                onClick={submitTodo}
                disabled={loading}
              >
                {loading
                  ? editingId
                    ? "Updating..."
                    : "Creating..."
                  : editingId
                  ? "Update Task"
                  : "Create Task"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* View Details Modal */}
      {showViewModal && viewingTodo && (
        <div className="modal-overlay" onClick={() => setShowViewModal(false)}>
          <div className="modal modal-view" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Task Details</h2>
              <button
                className="modal-close"
                onClick={() => setShowViewModal(false)}
              >
                ✕
              </button>
            </div>

            <div className="modal-body">
              <div className="detail-group">
                <label className="detail-label">Title</label>
                <p className="detail-value">{viewingTodo.title}</p>
              </div>

              {viewingTodo.description && (
                <div className="detail-group">
                  <label className="detail-label">Description</label>
                  <p className="detail-value">{viewingTodo.description}</p>
                </div>
              )}

              <div className="detail-row">
                <div className="detail-group">
                  <label className="detail-label">Status</label>
                  <p className="detail-value">
                    <span
                      className={`status-badge ${
                        viewingTodo.isCompleted ? "completed" : "pending"
                      }`}
                    >
                      {viewingTodo.isCompleted ? "✓ Completed" : "⏳ Pending"}
                    </span>
                  </p>
                </div>

                <div className="detail-group">
                  <label className="detail-label">Priority</label>
                  <p className="detail-value">
                    <span
                      className="priority-badge"
                      style={{
                        backgroundColor: getPriorityColor(viewingTodo.priority),
                      }}
                    >
                      {viewingTodo.priority
                        .charAt(0)
                        .toUpperCase() + viewingTodo.priority.slice(1)}
                    </span>
                  </p>
                </div>
              </div>

              {viewingTodo.dueDate && (
                <div className="detail-group">
                  <label className="detail-label">Due Date</label>
                  <p className="detail-value">
                    {formatDate(viewingTodo.dueDate)}
                    {isOverdue(viewingTodo) && (
                      <span className="overdue-label"> (Overdue)</span>
                    )}
                  </p>
                </div>
              )}

              <div className="detail-group">
                <label className="detail-label">Created</label>
                <p className="detail-value">
                  {new Date(viewingTodo.createdAt).toLocaleDateString("en-US", {
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </p>
              </div>
            </div>

            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowViewModal(false)}
              >
                Close
              </button>
              <button
                className="btn btn-edit"
                onClick={() => {
                  setShowViewModal(false);
                  editTodo(viewingTodo);
                }}
              >
                Edit Task
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
