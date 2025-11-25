<template>
  <div class="customer-table-wrapper">
    <div class="table-container">
      <table class="ms-table">
        <thead>
          <tr>
            <th>
              <!-- Checkbox all trên page -->
              <input type="checkbox" class="ml-32" :checked="allSelected" @change="toggleSelectAll" />
            </th>
            <th v-for="col in columns" :key="col.key" class="sortable" @mouseover="hoverColumn = col.key"
              @mouseleave="hoverColumn = ''">
              <div class="sortable-header" :ref="getHeaderRef(col.key)">
                <span>{{ col.label }}</span>
                <span v-if="hoverColumn === col.key" class="icon-sort ml-5"
                  @click.stop="toggleSortMenu(col.key)"></span>
              </div>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in tableData" :key="item.customerId" class="table-row" @dblclick="onEdit(item)">
            <td class="checkbox-style">
              <!-- Checkbox từng dòng -->
              <input type="checkbox" class="ml-32" :checked="isChecked(item.customerId)"
                @change="() => toggleOne(item.customerId)" />
            </td>
            <td>{{ item.type || "-" }}</td>
            <td><a class="code">{{ item.code }}</a></td>
            <td><a class="name">{{ item.name }}</a></td>
            <td>{{ item.tax || "-" }}</td>
            <td>{{ item.address || "-" }}</td>
            <td><span class="icon-tel mr-8 mt-8"></span><a class="phone">{{ item.phone || "-" }}</a></td>
            <td>{{ item.lastPurchaseDateFormatted || "-" }}</td>
            <td>{{ item.products || "-" }}</td>
            <td>{{ item.productNames || "-" }}</td>
          </tr>
          <tr v-if="!tableData.length">
            <td colspan="10" class="text-center">Không có dữ liệu</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Hover actions -->
    <div v-if="hoverItem" class="floating-actions" :style="{ top: actionTop + 'px' }">
      <span class="action-text icon-edit" @click="onEdit(hoverItem)"></span>
      <span class="action-text icon-delete" @click="() => onDelete(hoverItem.customerId)"></span>
    </div>

    <!-- Sort Menu (nổi) -->
    <div v-if="showSortMenu && sortColumn" class="sort-menu select-style"
      :style="{ top: sortMenuTop + 'px', left: sortMenuLeft + 'px' }">
      <div @click="changeSort(sortColumn, 'asc')"><span class="icon-up"></span> Tăng dần</div>
      <div @click="changeSort(sortColumn, 'desc')"><span class="icon-down"></span> Giảm dần</div>
    </div>

    <!-- Paging -->
    <div class="paging-wrapper">
      <div class="paging-left">
        <span class="icon-sliders"></span> Tổng số: <span class="strong-total">{{ total }}</span>
      </div>

      <div class="paging-right-container" ref="pageMenuRef">
        <div class="paging-center select-style" @click.stop="togglePageMenu">
          {{ pageSize }} Bản ghi trên trang
          <span class="icon icon-arrow-down"></span>
          <div v-if="showPageMenu" class="page-size-menu select-style">
            <div class="menu-item" :class="{ selected: pageSize === 10 }" @click.stop="setPageSize(10)">
              10 Bản ghi trên trang
              <span v-if="pageSize === 10" class="selected-icon">
                <span class="icon-ticker"></span>
              </span>
            </div>
            <div class="menu-item" :class="{ selected: pageSize === 20 }" @click.stop="setPageSize(20)">
              20 Bản ghi trên trang
              <span v-if="pageSize === 20" class="selected-icon">
                <span class="icon-ticker"></span>
              </span>
            </div>
            <div class="menu-item" :class="{ selected: pageSize === 50 }" @click.stop="setPageSize(50)">
              50 Bản ghi trên trang
              <span v-if="pageSize === 50" class="selected-icon">
                <span class="icon-ticker"></span>
              </span>
            </div>
            <div class="menu-item" :class="{ selected: pageSize === 100 }" @click.stop="setPageSize(100)">
              100 Bản ghi trên trang
              <span v-if="pageSize === 100" class="selected-icon">
                <span class="icon-ticker"></span>
              </span>
            </div>
          </div>
        </div>

        <div class="paging-nav">
          <button @click="prevPage" :disabled="page === 1">&lt;</button>
          <span>{{ pageStart }} - {{ pageEnd }}</span>
          <button @click="nextPage" :disabled="page === totalPage">&gt;</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick } from "vue";

const props = defineProps({
  tableData: { type: Array, default: () => [] },
  selectedRows: { type: Array, default: () => [] },
  page: { type: Number, default: 1 },
  pageSize: { type: Number, default: 10 },
  total: { type: Number, default: 0 },
});

const emit = defineEmits([
  "edit-customer",
  "delete-customer",
  "update:selected-rows",
  "page-change",
  "sort-change",
]);

// ---------------- CHECKBOX MULTI-PAGE ----------------
const selectedIdsSet = ref(new Set([...props.selectedRows]));

// watch prop selectedRows để sync khi parent update
watch(() => props.selectedRows, (newVal) => {
  selectedIdsSet.value = new Set(newVal);
});

// checkbox row
const isChecked = (id) => selectedIdsSet.value.has(id);

// checkbox all trên page
const allSelected = computed(() =>
  props.tableData.length && props.tableData.every(i => selectedIdsSet.value.has(i.customerId))
);

// toggle all trên page
const toggleSelectAll = () => {
  const idsOnPage = props.tableData.map(i => i.customerId);
  const isAllSelected = idsOnPage.every(id => selectedIdsSet.value.has(id));
  if (isAllSelected) {
    idsOnPage.forEach(id => selectedIdsSet.value.delete(id));
  } else {
    idsOnPage.forEach(id => selectedIdsSet.value.add(id));
  }
  emitSelected();
};

// toggle 1 row
const toggleOne = (id) => {
  if (selectedIdsSet.value.has(id)) selectedIdsSet.value.delete(id);
  else selectedIdsSet.value.add(id);
  emitSelected();
};

// emit selectedIdsSet ra parent
const emitSelected = () => {
  emit("update:selected-rows", Array.from(selectedIdsSet.value));
};

// ---------------- EDIT / DELETE ----------------
const onEdit = (item) => emit("edit-customer", item.customerId);
const onDelete = (id) => emit("delete-customer", id);

// ---------------- PAGING ----------------
const totalPage = computed(() => Math.ceil(props.total / props.pageSize));
const pageStart = computed(() => (props.total === 0 ? 0 : (props.page - 1) * props.pageSize + 1));
const pageEnd = computed(() => Math.min(props.page * props.pageSize, props.total));
const prevPage = () => emit("page-change", props.page - 1, props.pageSize);
const nextPage = () => emit("page-change", props.page + 1, props.pageSize);

const showPageMenu = ref(false);
const pageMenuRef = ref(null);
const togglePageMenu = () => { showPageMenu.value = !showPageMenu.value; };
const setPageSize = (size) => { emit("page-change", 1, size); showPageMenu.value = false; };

// ---------------- SORT ----------------
const hoverColumn = ref('');
const showSortMenu = ref(false);
const sortColumn = ref('');
const sortMenuTop = ref(0);
const sortMenuLeft = ref(0);
const headerRefs = ref({});

const columns = [
  { key: "type", label: "Loại khách hàng" },
  { key: "code", label: "Mã khách hàng" },
  { key: "name", label: "Tên khách hàng" },
  { key: "tax", label: "Mã số thuế" },
  { key: "address", label: "Địa chỉ (Giao hàng)" },
  { key: "phone", label: "Điện thoại" },
  { key: "buyDate", label: "Ngày mua hàng gần nhất" },
  { key: "products", label: "Hàng hóa đã mua" },
  { key: "productNames", label: "Tên hàng hóa đã mua" }
];

const getHeaderRef = (key) => (el) => { if (el) headerRefs.value[key] = el; };

const toggleSortMenu = (col) => {
  if (sortColumn.value === col) showSortMenu.value = !showSortMenu.value;
  else showSortMenu.value = true;
  sortColumn.value = col;

  nextTick(() => {
    const headerEl = headerRefs.value[col];
    if (headerEl) {
      const rect = headerEl.getBoundingClientRect();
      sortMenuTop.value = rect.bottom + window.scrollY;
      sortMenuLeft.value = rect.left + window.scrollX;
    }
  });
};

const changeSort = (col, dir) => {
  emit("sort-change", { sortBy: col, sortDir: dir });
  showSortMenu.value = false;
};
</script>

<style scoped>
.customer-table-wrapper {
  display: flex;
  flex-direction: column;
  flex: 1;
  border: 1px solid #dcdcdc;
  border-radius: 4px;
  background: #fff;
  min-height: 0;
}

.table-container {
  flex: 1;
  overflow-y: auto;
  min-height: 0;
  position: relative;
}

.ms-table th:first-child,
.ms-table td:first-child {
  width: 40px;
  text-align: center;
}

.strong-total {
  font-weight: 600;
}

/* Checkbox custom */
.ms-table th:first-child input,
.ms-table td:first-child input {
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  width: 16px;
  /* tăng kích thước */
  height: 16px;
  border: 1px solid #7c869c;
  border-radius: 4px;
  background-color: #fff;
  position: relative;
  cursor: pointer;
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
}

/* Hover: viền xanh */
.ms-table th:first-child input:hover,
.ms-table td:first-child input:hover {
  border-color: #0f2fbd;
  box-shadow: 0 0 0 2px rgba(15, 47, 189, 0.2);
}

/* Checked state */
.ms-table th:first-child input:checked,
.ms-table td:first-child input:checked {
  background-color: #4262f0;
  border-color: #0f2fbd;

}

/* Checked dấu tick */
.ms-table th:first-child input:checked::after,
.ms-table td:first-child input:checked::after {
  content: '';
  position: absolute;
  top: 3px;
  left: 6px;
  width: 4px;
  height: 8px;
  border: solid #fff;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.ms-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
}

.ms-table td,
.ms-table th {
  max-width: 200px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.checkbox-style {
  max-width: 50px;
}

.ms-table a.code,
.ms-table a.name,
.ms-table .phone {
  display: inline-block;
  max-width: 100%;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.ms-table th {
  padding: 12px 16px;
  border-bottom: 1px solid #f0f0f0;
  background: #f5f5f5;
  font-weight: 700;
  text-align: left;
  position: sticky;
  top: 0;
  z-index: 5;
}

.ms-table td {
  padding: 6px 16px;
  vertical-align: middle;
  border-bottom: 1px solid #f0f0f0;
}

.ms-table td input[type="checkbox"] {
  transform: translateY(-1px);
}

.phone,
.code,
.name {
  color: #0F2FBD;
  cursor: pointer;
  text-align: center;
}

.table-row {
  transition: background-color 0.3s;
}

.table-row:hover {
  background-color: #E7EBFD;
}

.actions-cell {
  position: relative;
  width: 1px;
}

.row-actions {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  gap: 10px;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.2s;
}

.table-row:hover .row-actions {
  opacity: 1;
  pointer-events: auto;
}

.sortable-header {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
}

.icon-sort {
  margin-left: 5px;
  cursor: pointer;
}

.sort-menu {
  position: fixed;
  /* Quan trọng */
  z-index: 2000;
  background: #fff;
  border: 1px solid #ccc;
  border-radius: 4px;
  min-width: 140px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

.sort-menu div {
  padding: 8px 12px;
  cursor: pointer;
  font-size: 14px;
}

.sort-menu div:hover {
  background-color: #f5f5f5;
}

/* Paging styles giữ nguyên của bạn */
.paging-wrapper {
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-top: 1px solid #e5e5e5;
  background: #fff;
}

.paging-left {
  font-size: 14px;
}

.paging-wrapper.paging-left strong {
  font-weight: 500;
}

.paging-right-container {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
}

/* --- Paging center chỉnh icon sát text --- */
.paging-center {
  padding: 6px 0px;
  border: 1px solid #ccc;
  border-radius: 4px;
  padding-left: 15px !important;

  /* CHỈNH QUAN TRỌNG */
  display: flex;
  align-items: center;
  gap: 4px;
  justify-content: space-around;
  cursor: pointer;
  user-select: none;
  position: relative;
  width: fit-content;
  min-width: 170px;
  padding-right: 10px;
  padding-left: 10px;
}

.paging-center .icon-arrow-down {
  margin-left: 2px;
  position: relative;
  top: 1px;
}


.page-size-menu {
  position: absolute;
  bottom: calc(100% + 1px);
  left: 0;
  width: 100%;
  min-width: 100%;
  background: #fff;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
  z-index: 999;
}


/* --- Item được chọn --- */
.menu-item.selected {
  color: #4290f2 !important;
  font-weight: 600;
}

/* Tick lấy luôn màu từ item selected */
.menu-item.selected span {
  color: #4290f2 !important;
}

.menu-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  /* Căn icon và text bằng nhau */
  padding: 8px 12px;
  padding-left: 19px !important;
  cursor: pointer;
}

.menu-item .selected-icon {
  display: flex;
  align-items: center;
}

.menu-item .icon-ticker {
  margin-left: 4px;
  position: relative;
  top: 1px;
  /* Hạ icon xuống 1 chút */
}

/* Màu khi selected */
.menu-item.selected {
  color: #4290f2;
}

.menu-item.selected .icon-ticker {
  color: #4290f2;
}


.menu-item:hover {
  background: #f3f3f3;
}

.check {
  color: #2ca01c;
  font-weight: bold;
}

.arrow {
  font-size: 12px;
}

.paging-nav {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Hover underline cho các thẻ a trong tbody */
.ms-table tbody a.code:hover,
.ms-table tbody a.name:hover,
.ms-table tbody a.phone:hover {
  text-decoration: underline;
  text-decoration-color: currentColor;
}

.paging-nav button {
  padding: 4px 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  background: #fff;
  cursor: pointer;
}

.paging-nav button:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}
</style>
