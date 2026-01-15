import axios from "axios";

// Your backend API URL (update if using a different port)
const API_URL = "https://localhost:5001/api/todos";

// API functions
export const getTodos = () => axios.get(API_URL);
export const createTodo = (todo) => axios.post(API_URL, todo);
export const updateTodo = (id, todo) => axios.put(`${API_URL}/${id}`, todo);
export const deleteTodo = (id) => axios.delete(`${API_URL}/${id}`);
