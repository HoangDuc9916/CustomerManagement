<template>
  <div v-if="modelValue" class="confirm-dialog-backdrop">
    <div class="confirm-dialog">
      <p class="confirm-text">{{ message }}</p>

      <div class="actions" :class="{ 'actions-center': hideCancel }">
        <!-- Nút Hủy chỉ hiện khi hideCancel = false -->
        <MSButton
          v-if="!hideCancel"
          text="Hủy bỏ"
          customClass="confirm-btn-cancel"
          @click="cancel"
        />

        <!-- Nút chính dùng confirmText + confirmClass -->
        <MSButton
          :text="confirmText || 'Xóa'"
          :customClass="confirmClass || 'confirm-btn-primary'"
          @click="confirm"
        />

      </div>
    </div>
  </div>
</template>

<script setup>
import MSButton from "@/components/ms-button/MS-Button.vue";

defineProps({
  message: String,
  modelValue: Boolean,
  confirmText: String,
  confirmClass: String,
  hideCancel: { type: Boolean, default: false } // prop mới
});

const emit = defineEmits(["update:modelValue", "confirm", "cancel"]);

const confirm = () => {
  emit("confirm");
  emit("update:modelValue", false);
};

const cancel = () => {
  emit("cancel");
  emit("update:modelValue", false);
};
</script>


<style scoped>
.confirm-dialog-backdrop {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.35);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.actions-center {
  justify-content: center;
}

.confirm-dialog {
  background-color: #fff;
  padding: 28px 32px;
  border-radius: 12px;
  min-width: 380px;
  text-align: center;
}

.confirm-text {
  font-size: 16px;
  margin-bottom: 24px;
  color: #333;
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.confirm-btn-cancel {
  height: 32px;
  line-height: 20px;
  padding: 0 17px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  background-color: #e2e4e9;
  border: 1px solid #e2e4e9;
  color: #1f2229;
}

.confirm-btn-primary {
  height: 32px;
  line-height: 20px;
  padding: 0 16px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  background-color: #ec4243;
  border: 1px solid #ec4243;
  color: #fff;
}

/* nút import xanh khi dùng toolbar */
.confirm-btn-import {
  height: 32px;
  line-height: 20px;
  padding: 0 16px;
  font-size: 13px;
  border-radius: 4px;
  cursor: pointer;
  background-color: #3a77f7;
  border: 1px solid #3a77f7;
  color: #fff;
}
</style>
