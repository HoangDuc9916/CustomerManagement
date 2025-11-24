<template>
  <section class="customer-container d-flex flex-direction-column flex-grow">
    <!-- Toolbar -->
    <customer-toolbar
      :selected-rows="selectedRows"
      @delete-selected="onDeleteSelected"
      @add="$emit('add')"
      @search="handleSearch"
      @filter="handleFilter"
    />

    <!-- Customer Table -->
    <customer-table
      ref="tableRef"
      :table-data="tableData"
      :selected-rows="selectedRows"
      :page="page"
      :page-size="pageSize"
      :total="total"
      @edit-customer="$emit('edit-customer', $event)"
      @delete-customer="onDeleteCustomer"
      @page-change="handlePageChange"
      @update:selected-rows="selectedRows = $event"
      @sort-change="handleSortChange"
    />

    <!-- Confirm Dialog -->
    <ConfirmDialog
      v-model="confirmDialog.show"
      :message="confirmDialog.message"
      @confirm="handleConfirmDelete"
    />

    <!-- Toast -->
    <div v-if="toast.show" :class="['toast', toast.type]">
      {{ toast.message }}
    </div>
  </section>
</template>
<script setup>
import { ref } from "vue";
import CustomerToolbar from "@/components/customer/CustomerToolbar.vue";
import CustomerTable from "@/components/customer/CustomerTable.vue";
import ConfirmDialog from "@/components/common/ConfirmDialog.vue";
import {
  deleteCustomer,
  deleteCustomerBulk,
  getCustomersSearch
} from "@/api/customerApi";

const tableRef = ref(null);
const selectedRows = ref([]);

const searchText = ref("");
const sortBy = ref(null);
const sortDir = ref(null);
const filterType = ref("");

const page = ref(1);
const pageSize = ref(10);
const total = ref(0);
const tableData = ref([]);

const formatDate = (dateStr) => {
  if (!dateStr) return null;
  const d = new Date(dateStr);
  const dd = String(d.getDate()).padStart(2, "0");
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const yyyy = d.getFullYear();
  return `${dd}-${mm}-${yyyy}`;
};


// Map cột sort trước khi gửi lên BE
const mapSortColumn = (col) => {
  return col === "type" ? "customer_type"
       : col === "code" ? "customer_code"
       : col === "name" ? "full_name"
       : col === "tax" ? "tax_code"
       : col === "address" ? "shipping_address"
       : col === "phone" ? "phone"
       : col === "buyDate" ? "last_purchase_date"
       : col === "products" ? "purchased_summary"
       : col === "productNames" ? "last_purchased_item"
       : col; // mặc định gửi nguyên
};

// ======================= LOAD DATA =======================
const loadData = async () => {
  try {
    const res = await getCustomersSearch({
      pageNumber: page.value,
      pageSize: pageSize.value,
      search: searchText.value,
      sortBy: mapSortColumn(sortBy.value),
      sortDir: sortDir.value,
      filterType: filterType.value
    });

    tableData.value = (res.data ?? []).map(item => ({
      customerId: item.customerId,
      type: item.customerType,
      code: item.customerCode,
      name: item.fullName,
      tax: item.taxCode,
      address: item.shippingAddress,
      phone: item.phone,
      buyDate: item.lastPurchaseDate,

      lastPurchaseDateFormatted: formatDate(item.lastPurchaseDate),
      products: item.purchasedSummary,
      productNames: item.lastPurchasedItem,
      email: item.email || "",
      billingAddress: item.billingAddress || "",
    }));

    total.value = res.meta?.total ?? 0;
  } catch (err) {
    console.error("Load data error:", err);
    tableData.value = [];
    total.value = 0;
  }
};

// ======================= SEARCH =======================
const handleSearch = async (search) => {
  searchText.value = search || "";
  page.value = 1;
  await loadData();
};

// ======================= SORT =======================
const handleSortChange = async ({ sortBy: col, sortDir: dir }) => {
  sortBy.value = col;
  sortDir.value = dir;
  page.value = 1;
  await loadData();
};

// ======================= FILTER =======================
const handleFilter = async (type) => {
  filterType.value = type;
  page.value = 1;
  await loadData();
};

// ======================= PAGING =======================
const handlePageChange = async (newPage, newPageSize) => {
  page.value = newPage;
  pageSize.value = newPageSize;
  await loadData();
};

// ======================= CONFIRM DELETE =======================
const confirmDialog = ref({ show: false, message: "", id: null });

const toast = ref({ show: false, message: "", type: "success" });
const showToast = (message, type = "success") => {
  toast.value = { show: true, message, type };
  setTimeout(() => (toast.value.show = false), 3000);
};

const onDeleteCustomer = (id) => {
  confirmDialog.value = {
    show: true,
    message: "Bạn có chắc muốn xóa khách hàng này?",
    id,
  };
};

const handleConfirmDelete = async () => {
  try {
    if (confirmDialog.value.isBulk) {
      // Xóa bulk
      await deleteCustomerBulk(selectedRows.value);
      showToast(`Xóa ${selectedRows.value.length} khách hàng thành công!`);
      selectedRows.value = [];
      await loadData();
    } else {
      // Xóa từng khách
      await deleteCustomer(confirmDialog.value.id);
      showToast("Xóa khách hàng thành công!");
      await loadData();
      selectedRows.value = [];
    }
  } catch (err) {
    console.error(err);
    showToast("Xóa thất bại!", "error");
  } finally {
    confirmDialog.value.show = false;
  }
};

// ======================= DELETE MULTIPLE =======================
const onDeleteSelected = () => {
  if (!selectedRows.value.length) return;

  confirmDialog.value = {
    show: true,
    message: `Bạn có muốn xóa ${selectedRows.value.length} khách hàng đã chọn không `,
    id: null, // bulk thì không cần id riêng
    isBulk: true // thêm flag để phân biệt bulk
  };
};

// ======================= INIT LOAD =======================
loadData();
</script>

<style scope>
.customer-container {
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  background-color: #e2e4e9;
  height: 95vh;
}

.toast {
  position: fixed;
  top: 80px;
  left: 50%;
  transform: translateX(-50%);
  background: #333;
  color: #fff;
  padding: 10px 16px;
  border-radius: 6px;
  opacity: 0.95;
  z-index: 9999;
  transition: 0.3s;
}
.toast.success { background-color: #4caf50; }
.toast.error { background-color: #f44336; }

</style>
