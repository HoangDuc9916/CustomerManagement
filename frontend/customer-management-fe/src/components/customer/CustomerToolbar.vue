<template>
  <article class="customer-toolbar">
    <div class="customer-toolbar-content d-flex flex-direction-row justify-content-space-between align-items-center">

      <!-- LEFT CONTENT -->
      <div class="toolbar-content-left d-flex flex-direction-row">
        <div class="crm-toolbar">

          <!-- LEFT CONTENT KHI KHÔNG CHỌN ROW -->
          <div v-if="!selectedRows.length" class="crm-toolbar-left d-flex flex-direction-row ml-16 align-items-center"
            @click="toggleDropdown" style="cursor: pointer;">
            <span class="icon icon-folder"></span>
            <h1 class="crm-title ml-8 mr-8">{{ selectedLabel }}</h1>
            <span class="icon icon-arrow-down"></span>
          </div>

          <!-- LEFT CONTENT KHI CÓ ROW ĐƯỢC CHỌN -->
          <div v-else class="toolbar-content-left d-flex flex-direction-row ml-8">
            <button class="ms-button ms-button-import btn btn-secondary3 d-flex align-items-center ml-7">

              <div class="button-import-excel mr-8 mt-3">Export Excel</div>
              <i class="ms-button-icon icon-export"></i>
            </button>
            <div class="crm-toolbar-left d-flex flex-direction-row ml-7 align-items-center">
              <span class="icon-delete crm-delete" @click="onDeleteSelected"></span>
            </div>
          </div>


          <!-- DROPDOWN chỉ hiển thị khi left content gốc đang hiện -->
          <div v-if="dropdownOpen && !selectedRows.length" class="filter-dropdown">
            <div class="filter-item" @click="selectFilter('')">Tất cả khách hàng</div>
            <div class="filter-item" @click="selectFilter('VIP')">VIP</div>
            <div class="filter-item" @click="selectFilter('LKHA')">LKHA</div>
            <div class="filter-item" @click="selectFilter('NBH01')">Nhà bán hàng 01</div>
          </div>

          <!-- Edit / Refresh chỉ khi không chọn row -->
          <div class="crm-toolbar-right" v-if="!selectedRows.length">
            <span class="crm-edit">Sửa</span>
            <span class="crm-refresh icon icon-refresh"></span>
          </div>

        </div>

        <!-- Toast Messages -->
        <div class="toast-wrapper">
          <ToastMessage v-for="toast in toasts" :key="toast.id" :message="toast.message" :type="toast.type" />
        </div>
      </div>

      <!-- RIGHT CONTENT -->
      <div class="toolbar-content-right d-flex flex-direction-row flex-end mr-8">

        <!-- Search -->
        <div class="toolbar-search">
          <div class="crm-ai-search">
            <div class="crm-ai-search-container">
              <div class="crm-ai-search-icon icon icon-search"></div>
              <input type="text" class="crm-ai-search-input" placeholder="Tìm kiếm thông minh" autocomplete="combobox"
                aria-autocomplete="list" aria-expanded="false" aria-haspopup="true" v-model="searchTerm"
                @keyup.enter="onSearch">
              <div class="ai-icon-wrapper">
                <div class="icon-ai-search"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Statistic / Avatar -->
        <div class="star-inserted ml-8">
          <div class="tooltip-trigger">
            <div class="statistic-ava">
              <button class="statistic-ava-btn">
                <div class="statistic-ava-btn-icon"></div>
              </button>
            </div>
          </div>
        </div>

        <!-- Buttons: Add + Import -->
        <div class="btn-group-form ml-8 mr-8 d-flex">
          <div class="crm-button" haspermisson="Add">
            <button class="ms-button btn btn-primary" @click="$emit('add')">
              <div class="ms-button-content d-flex align-items-center">
                <i class="ms-button-icon icon icon-add-white mr-8 ml-8"></i>
                <span class="ms-button-text mr-14 mt-3">Thêm</span>
              </div>
            </button>
          </div>

          <div class="crm-button" haspermisson="Import">
            <div class="crm-button-import">
              <input type="file" ref="fileInput" @change="handleFileUpload" accept=".csv" style="display: none" />
              <button class="ms-button ms-button-import btn btn-secondary3 d-flex align-items-center ml-7"
                @click="triggerFileInput">
                <i class="ms-button-icon icon icon-import-blue mr-8"></i>
                <div class="button-import-excel mr-11 mt-3">Nhập từ Excel</div>
              </button>
            </div>
          </div>
        </div>

        <!-- More Menu -->
        <div class="crm-show-menu align-self mt-1">
          <button class="ms-button btn btn-secondary btn-more d-flex align-items-center">
            <i class="icon icon-btn-more"></i>
          </button>
        </div>

        <!-- Switch View -->
        <div class="crm-show-menu">
          <button class="ms-button btn btn-secondary switch-view-type d-flex flex-direction-row mt-1">
            <div class="icon icon-dropdown"></div>
            <div class="icon icon-arrow-down ml-5"></div>
          </button>
        </div>

      </div>
    </div>
  </article>
</template>

<script setup>
import { ref, reactive } from "vue";
import ToastMessage from "../common/ToastMessage.vue";
import { importCustomers } from "@/api/customerApi";

// SEARCH
const searchTerm = ref("");

// Nhận prop selectedRows từ parent
const { selectedRows } = defineProps({
  selectedRows: { type: Array, default: () => [] }
});

const toasts = reactive([]);   // mảng toast
const emit = defineEmits(["add", "delete-selected", "search", "filter"]);

// ================= SEARCH =================
const onSearch = () => {
  emit("search", searchTerm.value);
};

// ================= DROPDOWN FILTER =================
const dropdownOpen = ref(false);
const selectedLabel = ref("Tất cả khách hàng"); // default

const toggleDropdown = () => {
  dropdownOpen.value = !dropdownOpen.value;
};

const selectFilter = (value) => {
  dropdownOpen.value = false;

  if (value === "") selectedLabel.value = "Tất cả khách hàng";
  else if (value === "VIP") selectedLabel.value = "VIP";
  else if (value === "LKHA") selectedLabel.value = "Liên kết hợp tác";
  else if (value === "NBH01") selectedLabel.value = "Nhà bán hàng 01";

  emit("filter", value); // gửi về CustomerView.vue
};

// ================= IMPORT FILE =================
const fileInput = ref(null);
const triggerFileInput = () => {
  fileInput.value.click();
};

const handleFileUpload = async (event) => {
  const file = event.target.files[0];
  if (!file) return;

  try {
    const formData = new FormData();
    formData.append("file", file);

    const response = await importCustomers(formData);
    if (response && response.data) {
      const { inserted, duplicatePhones, duplicateEmails } = response.data;
      showToast(`Import thành công! Thêm: ${inserted}`, "add");
      if (duplicatePhones) showToast(`Trùng số điện thoại: ${duplicatePhones}`, "info");
      if (duplicateEmails) showToast(`Trùng email: ${duplicateEmails}`, "info");
    } else {
      showToast("Import thất bại. Kiểm tra lại file CSV.", "delete");
    }
  } catch (err) {
    console.error(err);
    showToast("Có lỗi xảy ra khi import file.", "delete");
  } finally {
    event.target.value = "";
  }
};

// ================= DELETE MULTIPLE =================
const onDeleteSelected = () => {
  emit("delete-selected"); // gửi event về parent (View) xử lý xóa bulk
};

// ================= TOAST =================
const showToast = (message, type = "info") => {
  const id = Date.now();
  toasts.push({ id, message, type });
  setTimeout(() => {
    const index = toasts.findIndex(t => t.id === id);
    if (index !== -1) toasts.splice(index, 1);
  }, 5000);
};
</script>


<style scoped>
.filter-dropdown {
  position: absolute;
  top: 48px;
  left: 16px;
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 4px;
  width: 180px;
  z-index: 1000;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

.filter-item {
  padding: 8px 12px;
  cursor: pointer;
}

.filter-item:hover {
  background: #f3f3f3;
}

.toast-wrapper {
  position: fixed;
  top: 60px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.customer-toolbar {
  min-height: 56px;
  max-height: 56px;
}

.crm-toolbar {
  width: 100%;
  margin-top: 12px;
  height: 100%;
  display: flex !important;
  flex-direction: column !important;
  height: auto;
  position: relative;
  border-radius: 4px;
  -moz-background-clip: padding;
  -webkit-background-clip: padding-box;
  background-clip: padding-box;
  -ms-box-sizing: border-box;
  -o-box-sizing: border-box;
  box-sizing: border-box;
  flex-direction: row !important;
}

.crm-toolbar-left {
  background: #fff;
  border: 1px solid #D3D7DE;
  padding-right: 8px !important;
  padding-left: 12px !important;
  height: 30px;
  border-radius: 4px;
  justify-content: space-between;
  cursor: pointer;
}

.toolbar-content-right {
  font-size: 13px;
  margin-top: 12px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  align-self: center !important;
}



.crm-title {
  margin-right: 8px;
  margin-left: 8px;
  margin-top: 3px;
  margin-bottom: initial;
  cursor: pointer;
  font-size: 13px;
  font-weight: 700;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  line-height: 1.428571429;
  color: #1f2229;
}

.crm-toolbar-right {
  text-align: center;
  margin-top: 2px;
  align-self: center !important;
}

.crm-edit {
  vertical-align: middle;
  color: #586074;
  cursor: pointer;
  margin-left: 16px;
  color: #4262f0 !important;
}

.crm-refresh {
  vertical-align: middle;
  margin-left: 16px;
  display: inline-block;
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.crm-ai-search-container {
  width: 282px;
  height: 32px;
  display: block;
  position: relative;
  padding: 1px;
  border-radius: 4px;
}

.crm-ai-search-container::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(251deg, #4262F0 24.05%, #9F73F1 71.93%);
  border-radius: inherit;

}

.crm-ai-search-input {
  height: 100%;
  width: 100%;
  outline: none;
  border-radius: 3px;
  padding: 0 16px 0 32px;
  position: relative;
  border: none;
  font-size: 13px;
  box-sizing: border-box;
}

.crm-ai-search-input::placeholder {
  text-overflow: ellipsis;
}

.crm-ai-search-input:not(:focus) {
  background: linear-gradient(90deg, rgba(66, 98, 240, .1) 0%, rgba(159, 115, 241, .1) 100%), #FFF;
}

.ai-icon-wrapper {
  position: absolute;
  top: 8px;
  right: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.tooltip-trigger {
  padding: 0;
  margin: 0;
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  box-sizing: border-box;
}

.statistic-ava {
  padding: 1px;
  position: relative;
  height: 32px;
  border-radius: 4px;
}

.statistic-ava::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(251deg, #4262F0 24.05%, #9F73F1 71.93%);
  border-radius: inherit;
}


.crm-button-import {
  display: inline-block;
  position: relative;
}

.crm-button-import::before {
  content: "";
  position: absolute;
  background: linear-gradient(251deg, #4262F0 24.05%, #9F73F1 71.93%);
  border-radius: inherit;
  z-index: 0;
  transition: opacity 0.2s ease-in-out;
}

.crm-button-import button {
  position: relative;
  z-index: 1;
  background: #fff;
  border: none;
  border-radius: 3px;
  padding: 4px 12px;
  display: flex;
  align-items: center;
  font-size: 13px;
  outline: none;
  transition: background 0.2s ease-in-out;
}

.crm-button-import i,
.crm-button-import .button-import-excel {
  position: relative;
  z-index: 1;
}

.crm-button-import:hover button {
  background: linear-gradient(90deg, rgba(66, 98, 240, 0.1) 0%, rgba(159, 115, 241, 0.1) 100%), #fff;
}


.statistic-ava-btn {
  background: linear-gradient(90deg, #E7EBFD 0%, #ECE7FD 32.21%, #E5F7FF 66.11%, #FDEFE7 100%);
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 0 7px;
  border: none;
  height: 100%;
  position: relative;
  border-radius: 3px;
}

.statistic-ava-btn-icon {
  display: inline-block;
  width: 16px;
  height: 16px;

}

.statistic-ava-btn-icon::before {
  content: "";
  display: inline-block;
  width: 16px;
  height: 16px;
  background: transparent url("../../assets/images/ICON.svg") no-repeat -16px -1425px;
}

.btn-group-form {
  align-self: center !important;
}

.crm-delete {
  cursor: pointer;
  color: red;
  display: inline-block;
}

.ms-button.btn.btn-primary {
  text-transform: none !important;
  font-weight: 500 !important;
  height: 32px;
  line-height: 20px !important;
  -moz-background-clip: padding;
  -webkit-background-clip: padding-box;
  background-clip: padding-box;
  border: 1px solid #4262f0;
  -ms-box-sizing: border-box;
  -o-box-sizing: border-box;
  box-sizing: border-box;
  background-color: #4262f0;
  color: #fff;
  font-size: 13px;
  line-height: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  font-weight: 400;
  font-feature-settings: normal;
  font-variant: normal;
  cursor: pointer;
  text-align: center;
  white-space: nowrap;
  overflow: hidden;
  transition: all .3s ease-in-out;
  -moz-transition: all .3s ease-in-out;
  -webkit-transition: all .3s ease-in-out;
  position: relative;
  -ms-box-sizing: border;
  -o-box-sizing: border;
  box-sizing: border;
  border-radius: 4px;
}

.ms-button.btn.btn-secondary3 {
  border: 1px solid #4262f0 !important;
  min-width: 64px;
  font-weight: 500 !important;
  height: 32px;
  line-height: 20px !important;
  padding: 4px 8px;
  border-radius: 4px;
  -moz-background-clip: padding;
  -webkit-background-clip: padding-box;
  background-clip: padding-box;
  border: 1px solid #ffffff;
  -ms-box-sizing: border-box;
  -o-box-sizing: border-box;
  box-sizing: border-box;
  background-color: #fff;
  color: #4262f0;
  font-size: 13px;
  line-height: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  font-weight: 400;
  font-feature-settings: normal;
  font-variant: normal;
  cursor: pointer;
  text-align: center;
  white-space: nowrap;
  overflow: hidden;
  transition: all .3s ease-in-out;
  -moz-transition: all .3s ease-in-out;
  -webkit-transition: all .3s ease-in-out;
  position: relative;
  -ms-box-sizing: border;
  -o-box-sizing: border;
  box-sizing: border;
}

.crm-show-menu {
  position: relative;
  margin-right: 8px;
  display: block;
}

.btn-more {
  border-radius: 4px;
  border: 1px solid #d3d7de;
  overflow: hidden;
  min-width: 36px !important;
  width: 36px !important;
  height: 32px !important;
}

.ms-button.btn.btn-secondary {
  text-transform: none !important;
  font-weight: 500 !important;
  border: 1px solid #d3d7de !important;
  min-width: 64px;
  height: 32px;
  line-height: 20px !important;
  padding: 5px 8px;
  border-radius: 4px;
  -moz-background-clip: padding;
  -webkit-background-clip: padding-box;
  background-clip: padding-box;
  border: 1px solid #e2e4e9;
  -ms-box-sizing: border-box;
  -o-box-sizing: border-box;
  box-sizing: border-box;
  background-color: #fff;
  color: #1f2229;
  font-size: 13px;
  line-height: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  font-weight: 400;
  font-feature-settings: normal;
  font-variant: normal;
  cursor: pointer;
  text-align: center;
  white-space: nowrap;
  overflow: hidden;
  transition: all .3s ease-in-out;
  -moz-transition: all .3s ease-in-out;
  -webkit-transition: all .3s ease-in-out;
  position: relative;
  -ms-box-sizing: border;
  -o-box-sizing: border;
  box-sizing: border;
}

.switch-view-type {
  padding: 7px !important;
  min-width: 31px !important;
  width: 55px !important;
}
</style>
