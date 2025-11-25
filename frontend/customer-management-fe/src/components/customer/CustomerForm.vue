<template>
  <div class="customer-form-wrapper">
    <article class="form-title-header">
      <div class="customer-form-header d-flex justify-content-space-between align-items-center">

        <div class="d-flex flex-direction-row">
          <h2 class="form-title mt-3">{{ editing ? "Sửa Khách hàng" : "Thêm Khách hàng" }}</h2>
          <div class="ms-title-top select-layout ms-title align-items-center">
            <span v-if="!editing" class="ms-title-text">Mẫu tiêu chuẩn</span>
            <span v-if="!editing" class="select-layout-drop-down icon icon-arrow-down"></span>
          </div>
          <div class="crm-toolbar-right">
            <span class="crm-edit">Sửa bố cục</span>
          </div>
        </div>

        <!-- FORM ACTION BUTTONS -->
        <div class="form-actions-header d-flex align-items-center gap-8">

          <!-- Button Hủy bỏ -->
          <MSButton type="secondary" text="Hủy bỏ" @click="$emit('cancel')" />

          <!-- Button Lưu và thêm -->
          <div class="crm-button-import">
            <MSButton type="secondary3" text="Lưu và Thêm" @click="saveAndContinue" />
          </div>

          <!-- Button Lưu -->
          <MSButton type="primary" text="Lưu" @click="saveCustomer($event)" />

        </div>
      </div>
    </article>

    <!-- FORM -->
    <form class="form-layout">
      <div class="form-img-add">
        <div class="title-image">Ảnh</div>
        <div class="image-account avatar-account" @click="selectImage">
          <img v-if="previewImage" :src="previewImage" alt="avatar" />
          <div v-else class="icon-account"></div>
        </div>
        <input type="file" ref="fileInput" @change="onFileChange" accept="image/*" style="display:none" />
      </div>

      <span class="form-title-add">Thông tin chung</span>

      <!-- ROW 1 -->
      <div class="form-row">
        <div class="form-group">
          <label>Mã khách hàng</label>
          <div class="input-wrapper">
            <input v-model="model.customerCode" type="text" disabled placeholder="Mã tự sinh"
              :class="{ 'input-error': errors.customerCode }" />
            <span v-if="errors.customerCode" class="error-text">{{ errors.customerCode }}</span>
          </div>
        </div>

        <div class="form-group">
          <label>Tên khách hàng <span class="star">*</span></label>
          <div class="input-wrapper">
            <input v-model="model.fullName" type="text" @input="clearError('fullName')"
              :class="{ 'input-error': errors.fullName }" />
            <span v-if="errors.fullName" class="error-text">{{ errors.fullName }}</span>
          </div>
        </div>
      </div>

      <!-- ROW 2 -->
      <div class="form-row">
        <div class="form-group">
          <label>Loại khách hàng</label>
          <div class="input-wrapper">
            <select v-model="model.customerType">
              <option value="">Chọn loại khách hàng</option>
              <option value="VIP">VIP</option>
              <option value="LKHA">LKHA</option>
              <option value="NBH01">NBH01</option>
            </select>
            <span v-if="errors.customerType" class="error-text">{{ errors.customerType }}</span>
          </div>
        </div>

        <div class="form-group">
          <label>Số điện thoại <span class="star">*</span></label>
          <div class="input-wrapper">
            <input v-model="model.phone" type="text" @input="clearError('phone')"
              :class="{ 'input-error': errors.phone }" />
            <span v-if="errors.phone" class="error-text">{{ errors.phone }}</span>
          </div>
        </div>
      </div>

      <!-- ROW 3 -->
      <div class="form-row">
        <div class="form-group">
          <label>Email <span class="star">*</span></label>
          <div class="input-wrapper">
            <input v-model="model.email" type="email" @input="clearError('email')"
              :class="{ 'input-error': errors.email }" />
            <span v-if="errors.email" class="error-text">{{ errors.email }}</span>
          </div>
        </div>

        <div class="form-group">
          <label>Mã số thuế</label>
          <div class="input-wrapper">
            <input v-model="model.taxCode" type="text" />
          </div>
        </div>
      </div>

      <!-- ROW 4 -->
      <div class="form-row">
        <div class="form-group">
          <label>Địa chỉ giao hàng</label>
          <div class="input-wrapper">
            <input v-model="model.shippingAddress" type="text" />
          </div>
        </div>

        <div class="form-group">
          <label>Địa chỉ hoá đơn</label>
          <div class="input-wrapper">
            <input v-model="model.billingAddress" type="text" />
          </div>
        </div>
      </div>

      <!-- ROW 5 -->
      <div class="form-row">
        <div class="form-group">
          <label>Ngày mua gần nhất</label>
          <div class="input-wrapper">
            <input v-model="model.lastPurchaseDate" type="date" />
          </div>
        </div>

        <div class="form-group">
          <label>Tên hàng hóa đã mua</label>
          <div class="input-wrapper">
            <input v-model="model.lastPurchasedItem" type="text" />
          </div>
        </div>
      </div>

      <!-- ROW 6 -->
      <div class="form-row">
        <div class="form-group" style="flex: 0 0 49.5%;">
          <label>Hàng hóa đã mua</label>
          <div class="input-wrapper">
            <input v-model="model.purchasedSummary" type="text" />
          </div>
        </div>
      </div>

      <div v-if="errors.general" class="error-text">{{ errors.general }}</div>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref } from "vue";
import api from "@/api/customerApi";
import MSButton from "@/components/ms-button/MS-Button.vue"

const emit = defineEmits(["saved", "added-and-continue", "cancel"]);

const editing = ref(false);
const previewImage = ref(null);
const fileInput = ref(null);

const model = reactive({
  customerId: null,
  customerCode: "",
  fullName: "",
  phone: "",
  email: "",
  customerType: "",
  taxCode: "",
  shippingAddress: "",
  billingAddress: "",
  purchasedSummary: "",
  lastPurchasedItem: "",
  lastPurchaseDate: null,
  avatarUrl: ""  // sẽ lưu Base64 hoặc URL
});

const formatDate = (iso) => {
  if (!iso) return null;
  const d = new Date(iso);
  const day = String(d.getDate()).padStart(2, "0");
  const month = String(d.getMonth() + 1).padStart(2, "0");
  const year = d.getFullYear();
  return `${day}-${month}-${year}`;
};


const errors = reactive({
  fullName: "",
  phone: "",
  email: "",
  general: "",
});

const clearError = (field) => {
  errors[field] = "";
  errors.general = "";
};
/**
 * Created by: Duchc - Hoàng Chí Đức - 24112025
 */
const validateSequential = () => {
  // Reset lỗi trước khi validate
  errors.fullName = "";
  errors.phone = "";
  errors.email = "";
  errors.general = "";

  // 1) Validate FullName
  if (!model.fullName?.trim()) {
    errors.fullName = "Tên khách hàng không để trống";
    return false; // Dừng ở đây
  }

  // 2) Validate Phone
  if (!model.phone?.trim()) {
    errors.phone = "Số điện thoại không để trống";
    return false;
  } else if (!/^\d{10,11}$/.test(model.phone)) {
    errors.phone = "Số điện thoại phải 10–11 chữ số";
    return false;
  }

  // 3) Validate Email
  if (!model.email?.trim()) {
    errors.email = "Email không để trống";
    return false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(model.email)) {
    errors.email = "Email không hợp lệ";
    return false;
  }

  // Nếu tất cả hợp lệ
  return true;
};


// const validate = () => {
//   let valid = true;
//   Object.keys(errors).forEach(k => errors[k] = "");

//   if (!model.fullName?.trim()) {
//     errors.fullName = "Tên khách hàng không để trống";
//     valid = false;
//   }
//   if (!model.phone?.trim()) {
//     errors.phone = "Số điện thoại không để trống";
//     valid = false;
//   } else if (!/^\d{10,11}$/.test(model.phone)) {
//     errors.phone = "Số điện thoại phải 10–11 chữ số";
//     valid = false;
//   }
//   if (!model.email?.trim()) {
//     errors.email = "Email không để trống";
//     valid = false;
//   } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(model.email)) {
//     errors.email = "Email không hợp lệ";
//     valid = false;
//   }
//   return valid;
// };

// Reset form
const resetForm = () => {
  editing.value = false;
  Object.keys(model).forEach(k => model[k] = k === "customerId" || k === "avatarUrl" ? null : "");
  previewImage.value = null;
  Object.keys(errors).forEach(k => errors[k] = "");
};

// Prepare add new customer
const prepareNewCustomer = async () => {
  resetForm();
  editing.value = false;
  try {
    const code = await api.getNewCustomerCode();
    console.log("Generated code:", code);
    model.customerCode = code;
  } catch (err) {
    console.error("Generate customer code failed:", err);
    model.customerCode = "";
  }
};

// Load customer khi edit
const loadCustomer = async (id) => {
  try {
    const data = await api.getCustomerById(id);
    if (!data) return;

    editing.value = true;
    model.customerId = data.customerId;
    model.customerCode = data.customerCode;
    model.fullName = data.fullName;
    model.customerType = data.customerType;
    model.taxCode = data.taxCode;
    model.shippingAddress = data.shippingAddress;
    model.billingAddress = data.billingAddress || "";
    model.phone = data.phone;
    model.email = data.email || "";
    model.purchasedSummary = data.purchasedSummary;
    model.lastPurchasedItem = data.lastPurchasedItem;
    model.lastPurchaseDate = data.lastPurchaseDate ? data.lastPurchaseDate.split("T")[0] : null;
    model.avatarUrl = data.avatarUrl || "";
    previewImage.value = model.avatarUrl || null;
  } catch (err) {
    console.error("Load customer failed:", err);
  }
};
defineExpose({ loadCustomer, prepareNewCustomer });

const handleApiError = (err) => {
  Object.keys(errors).forEach(k => errors[k] = "");
  const msg = err?.response?.data?.userMsg || "";
  if (msg.includes("Email already exists")) errors.email = "Email đã tồn tại, vui lòng nhập email khác";
  if (msg.includes("Phone already exists")) errors.phone = "Số điện thoại đã tồn tại, vui lòng nhập số khác";
  if (!errors.email && !errors.phone && msg) errors.general = msg;
};

// Chọn ảnh
const selectImage = () => {
  fileInput.value?.click();
};

// Chọn file và chuyển thành Base64 để lưu vào avatarUrl
const onFileChange = (event) => {
  const file = event.target.files?.[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => previewImage.value = model.avatarUrl = e.target.result;
  reader.readAsDataURL(file);
};

const buildPayload = () => ({
  customerCode: model.customerCode,
  fullName: model.fullName,
  phone: model.phone,
  email: model.email,
  customerType: model.customerType,
  taxCode: model.taxCode,
  shippingAddress: model.shippingAddress,
  billingAddress: model.billingAddress,
  purchasedSummary: model.purchasedSummary,
  lastPurchasedItem: model.lastPurchasedItem,
  lastPurchaseDate: model.lastPurchaseDate || null,
  avatarUrl: model.avatarUrl || "",
  IsDelete: "1"
});

const saveCustomer = async (event) => {
  event.preventDefault();
  if (!validateSequential()) return;
  try {
    const payload = buildPayload();
    if (editing.value) {
      await api.updateCustomer(model.customerId, { ...payload, customerId: model.customerId });
    } else {
      // ADD NEW: không gửi customerId
      await api.createCustomer(payload);
    }
    emit("saved", {
      customer: {
        ...payload,
        lastPurchaseDateFormatted: formatDate(payload.lastPurchaseDate)
      },
      action: editing.value ? "edit" : "add"
    });

    resetForm();
  } catch (err) {
    handleApiError(err);
  }
};

const saveAndContinue = async () => {
  if (!validateSequential()) return;
  try {
    const payload = buildPayload();
    if (editing.value) {
      await api.updateCustomer(model.customerId, { ...payload, customerId: model.customerId });
    } else {
      await api.createCustomer(payload);
    }
    emit("added-and-continue", {
      customer: {
        ...payload,
        lastPurchaseDateFormatted: formatDate(payload.lastPurchaseDate)
      },
      action: editing.value ? "edit" : "add"
    });

    // Reset form + sinh mã mới
    await prepareNewCustomer();
  } catch (err) {
    handleApiError(err);
  }
};

</script>

<style scoped>
.form-title-header {
  background: #E2E4E9;
}

.customer-form-wrapper {
  background-color: #fff;
  border-radius: 4px;
}

.title-image {
  font-size: 18px !important;
  font-weight: 500;
  margin-bottom: 16px;
  text-shadow: 0.02em 0 0 currentColor;
}

.star {
  font-size: 0.45em;
  position: relative;
  top: -4px;
  color: red;
}

.customer-form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 0 16px;
  border-radius: 4px;
  height: 56px;
}

.form-title {
  font-size: 20px;
  font-weight: 500;
  color: #1f2229;
  margin-right: 8px;
  text-shadow: 0.02em 0 0 currentColor;
}

.form-title-add {
  font-size: large;
  font-weight: 500;
  color: #1f2229;
  margin-bottom: 8px;
  text-shadow: 0.02em 0 0 currentColor;
}

.form-img-add {
  height: 84px;
  color: #1f2229;
  margin-bottom: 32px;
}

.form-actions-header {
  display: flex;
  align-items: center;
  gap: 8px;
}

.form-actions-header .ms-button.btn.btn-primary {
  height: 32px;
  line-height: 20px;
  padding: 0 16px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  background-color: #4262f0;
  border: 1px solid #4262f0;
  color: #fff;
}

.form-actions-header .ms-button.btn.btn-secondary3 {
  height: 32px;
  line-height: 20px;
  padding: 0 14px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  color: linear-gradient(251deg, #4262F0 24.05%, #9F73F1 71.93%);
}

.form-actions-header .ms-button.btn.btn-secondary {
  height: 32px;
  line-height: 20px;
  padding: 0 17px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  background-color: #fff;
  border: 1px solid #d3d7de;
  color: #1f2229;
}

.avatar-account {
  border-radius: 50%;
  width: 48px;
  height: 48px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f0f0f0;
  /* màu nền khi chưa có ảnh */
}

.avatar-account img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  /* đảm bảo ảnh cover toàn bộ khung tròn */
  display: block;
}


.image-account {
  background-color: #fff;
  cursor: pointer;
}

/* Form layout */
.form-layout {
  display: flex;
  flex-direction: column;
  gap: 12px;
  height: 85.5vh;
  padding-top: 32px;
  margin-left: 56px;
}

.ms-title-top {
  margin: 0 8px 0 0 !important;
  font-weight: 500;
}

.select-layout {
  cursor: pointer;
  padding: 5px 0 0 8px;
  margin-left: 8px;
  font-size: 16px;
  color: #1f2229;
  font-weight: 500;
  border-radius: 4px;
  -moz-background-clip: padding;
  -webkit-background-clip: padding-box;
  background-clip: padding-box;
}

.ms-title {
  font-size: 20px;
  font-weight: 500;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  line-height: 1.428571429;
  padding: 0;
  margin: 0;
  color: #1f2229;
  text-shadow: 0.02em 0 0 currentColor;
}

.ms-title-text {
  font-size: 16px;
  font-weight: 500;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  line-height: 1.428571429;
  padding: 0;
  margin: 0;
  color: #1f2229;
  margin-left: 12px;
}

.select-layout-drop-down {
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  margin-left: 8px;

}

/* === Form rows === */
.form-row {
  display: flex;
  width: 80vw;
  gap: 0;
}

.form-group {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 8px;
  flex: 1;
  min-width: 0;
  position: relative;
  margin-bottom: 3px;
}

.form-group label {
  font-size: 13px;
  font-weight: 500;
  color: #1f2229;
  margin-bottom: 0;
  width: 160px;
  flex-shrink: 0;
}

.form-group input,
.form-group select {
  flex: 1 1 250px;
  min-width: 510px;
  /* tránh quá ngắn */

  height: 32px;
  font-size: 13px;
  padding: 0 12px;
  border: 1px solid #d3d7de;
  border-radius: 4px;
  outline: none;
  box-sizing: border-box;
}


/* Input focus */
.form-group input:focus,
.form-group select:focus {
  border-color: #4262f0;
  box-shadow: 0 0 0 2px rgba(66, 98, 240, 0.2);
}

/* Input có lỗi */
.form-group input.input-error,
.form-group select.input-error {
  border-color: red;
}

/* Error text */
.form-group .error-text {
  display: block;
  color: red;
  font-size: 12px;
  margin-top: 4px;
  width: calc(100% - 140px);
  /* align với input */
  width: auto;
  /* tự động theo input */
  position: static;
  /* thay vì absolute */

  left: 140px;
  bottom: -18px;
}

/* Half width form group */
.form-group.half-width {
  flex: 0 0 49.5%;
}

/* Trường input một mình */
.form-row.single-input .form-group {
  flex: 0 0 49.5%;
}

.crm-toolbar-right {
  text-align: center;
  margin-top: 2px;
  align-self: center !important;
}

/* Buttons */
.btn-secondary3 {
  border: 1px solid #4262f0 !important;
  min-width: 64px;
  font-weight: 500 !important;
  height: 32px;
  line-height: 20px !important;
  padding: 4px 8px;
  border-radius: 4px;
  background-clip: padding-box;
  background-color: #fff;
  color: #4262f0;
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  cursor: pointer;
  text-align: center;
  white-space: nowrap;
  overflow: hidden;
  transition: all .3s ease-in-out;
  position: relative;
}

.crm-button-import {
  display: inline-block;
  position: relative;
}

.crm-button-import::before {
  content: "";
  position: absolute;
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

.button-import-excel {
  top: 2px;
}

.button-cancel {
  display: inline-flex !important;
  align-items: center;
  transform: translateY(2px);
}

.button-save {
  line-height: normal !important;
  padding-top: 1px !important;
}

/* Date picker đẹp mắt */
input[type="date"] {
  padding: 8px 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 14px;
  color: #333;
  background-color: #fff;
  appearance: none;
  /* tắt style mặc định trình duyệt */
  -webkit-appearance: none;
  -moz-appearance: none;
  cursor: pointer;
  position: relative;
  width: 100%;
  box-sizing: border-box;
  transition: all 0.2s ease;
}

/* Calendar icon (hiển thị bên phải) */
input[type="date"]::-webkit-calendar-picker-indicator {
  opacity: 0.7;
  cursor: pointer;
  position: absolute;
  right: 10px;
  width: 20px;
  height: 20px;
}

/* Focus */
input[type="date"]:focus {
  border-color: #4a90e2;
  box-shadow: 0 0 0 2px rgba(74, 144, 226, 0.2);
  outline: none;
}


.crm-button-import:hover button {
  background: linear-gradient(90deg, rgba(66, 98, 240, 0.1) 0%, rgba(159, 115, 241, 0.1) 100%), #fff;
}

.crm-edit {
  vertical-align: middle;
  color: #586074;
  cursor: pointer;

  color: #4262f0 !important;
}


/* Responsive */
@media (max-width: 1024px) {
  .form-row {
    flex-wrap: wrap;
  }

  .form-group input,
  .form-group select {
    min-width: 200px;
    flex: 1 1 auto;
  }
}

@media (max-width: 768px) {
  .form-row {
    flex-direction: column;
  }

  .form-group {
    width: 100%;
  }

  .form-group input,
  .form-group select {
    min-width: 100%;
  }
}

@media (max-width: 480px) {
  .form-group label {
    font-size: 12px;
    width: 140px;
  }

  .form-group input,
  .form-group select {
    font-size: 12px;
    padding: 4px 8px;
  }
}
</style>
