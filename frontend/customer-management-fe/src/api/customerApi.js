// Created by: Duchc - Hoàng Chí Đức - 20112025
import api from "./apiConfig";

/**
 * Lấy danh sách khách hàng
 */
export const getCustomers = async () => {
  const res = await api.get("/customers");
  return res.data?.data || [];
};

/**
 * Lấy khách hàng theo ID
 */
export const getCustomerById = async (id) => {
  const res = await api.get(`/customers/${id}`);
  return res.data?.data || null;
};

/**
 * Tạo khách hàng mới
 * Không gửi customerId, BE tự sinh
 */
export const createCustomer = async (customer) => {
  const payload = { ...customer }; // không bọc entity
  const res = await api.post("/customers", payload);
  return res.data?.data || null;
};

/**
 * Cập nhật khách hàng
 * Phải gửi customerId hợp lệ
 */
export const updateCustomer = (id, customer) => {
  const payload = { ...customer };
  return api.put(`/customers/${id}`, payload);
};

/**
 * Xóa khách hàng (soft delete)
 */
export const deleteCustomer = (id) => api.delete(`/customers/${id}`);

// Gọi bulk delete BE vừa tạo
export const deleteCustomerBulk = (ids) => {
  return api.delete("/customers/bulk", { data: ids });
};


// customerApi.js
export const importCustomers = async (formData) => {
  // formData là instance của FormData, chứa file CSV
  const res = await api.post("/customers/import-csv", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return res.data?.data || null;
};

export const getCustomersSearch = async ({ pageNumber = 1, pageSize = 10, search = "", sortBy, sortDir, filterType } = {}) => {
  const res = await api.get("/customers/articles", {
    params: { pageNumber, pageSize, search, sortBy, sortDir,customerType: filterType || "" }
  });
  return res.data || { data: [], meta: { total: 0 } };
};

/**
 * Lấy mã khách hàng mới (BE sẽ sinh)
 */
export const getNewCustomerCode = async () => {
  const res = await api.get("/customers/new-code");
  // Nếu BE trả chuỗi thô, res.data chính là mã
  return res.data || "";
};


export default {
  getCustomers,
  getCustomerById,
  createCustomer,
  updateCustomer,
  deleteCustomer,
  importCustomers,
  deleteCustomerBulk,
  getCustomersSearch,
  getNewCustomerCode
};
