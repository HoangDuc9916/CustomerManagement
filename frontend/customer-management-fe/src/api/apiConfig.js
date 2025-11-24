// Created by: Duchc - Hoàng Chí Đức - 20112025
import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7166/api", // URL backend
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10000, // 10 giây timeout, tuỳ chỉnh
});

// Nếu cần interceptor (ví dụ log lỗi hoặc auth token) có thể thêm ở đây
api.interceptors.response.use(
  response => response,
  error => {
    console.error("API error:", error);
    return Promise.reject(error);
  }
);

export default api;
