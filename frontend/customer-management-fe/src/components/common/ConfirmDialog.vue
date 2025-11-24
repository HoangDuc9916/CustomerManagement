<template>
  <div v-if="modelValue" class="confirm-dialog-backdrop">
    <div class="confirm-dialog">
      <p>{{ message }}</p>
      <div class="actions">
        <button class="btn btn-secondary" @click="cancel">Hủy</button>
        <button class="btn btn-danger" @click="confirm">Xóa</button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  message: { type: String, default: "Bạn có chắc muốn xóa?" },
  modelValue: { type: Boolean, default: false }
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
  animation: fadeIn 0.2s ease;
}

.confirm-dialog {
  background-color: #fff;
  padding: 20px 28px;
  border-radius: 10px;
  box-shadow: 0 8px 20px rgba(0,0,0,0.25);
  min-width: 300px;
  max-width: 90%;
  text-align: center;
  animation: slideUp 0.2s ease;
}

.confirm-dialog p {
  font-size: 16px;
  margin-bottom: 20px;
  color: #333;
}

.confirm-dialog .actions {
  display: flex;
  justify-content: space-around;
}

.confirm-dialog .btn {
  padding: 6px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 500;
  transition: 0.2s;
}

.btn-danger {
  background-color: #f44336;
  color: #fff;
}
.btn-danger:hover {
  background-color: #e53935;
}

.btn-secondary {
  background-color: #e0e0e0;
  color: #000;
}
.btn-secondary:hover {
  background-color: #d5d5d5;
}

@keyframes fadeIn { from {opacity: 0;} to {opacity:1;} }
@keyframes slideUp { from {transform: translateY(10px);} to {transform: translateY(0);} }
</style>
