<template>
  <transition name="toast-fade">
    <div v-if="visible" :class="['toast-message', type]" :style="styleObj">
      <span>{{ message }}</span>
      <button class="close-btn" @click="close">×</button>
    </div>
  </transition>
</template>

<script setup>
import { reactive } from 'vue';

defineProps({
  message: String,
  type: {
    type: String,
    default: 'info' // add | edit | delete | info
  }
});

const visible = reactive({ value: true });

const styleObj = reactive({
  position: 'fixed',
  top: '60px', // ngang tầm toolbar
  left: '50%',
  transform: 'translateX(-50%)',
  padding: '8px 16px',
  color: '#fff',
  borderRadius: '4px',
  zIndex: 9999,
  whiteSpace: 'nowrap',
  fontWeight: 500,
});

const close = () => visible.value = false;

// tự ẩn 5s
setTimeout(() => close(), 5000);
</script>

<style scoped>
.toast-message {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 80px;
  max-width: 400px;
  font-size: 14px;
}

.toast-message.add {
  background-color: #4caf50; /* xanh */
}
.toast-message.edit {
  background-color: #31b491; /* cam */
}
.toast-message.delete {
  background-color: #f44336; /* đỏ */
}
.toast-message.info {
  background-color: #607d8b; /* xám */
}

.close-btn {
  margin-left: 12px;
  background: transparent;
  border: none;
  color: #fff;
  font-size: 16px;
  cursor: pointer;
}

.toast-fade-enter-active, .toast-fade-leave-active {
  transition: opacity 0.3s;
}
.toast-fade-enter-from, .toast-fade-leave-to {
  opacity: 0;
}
</style>
