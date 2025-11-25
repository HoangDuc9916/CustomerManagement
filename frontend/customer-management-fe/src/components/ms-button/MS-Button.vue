<template>
  <button
    class="ms-button btn"
    :class="[buttonClass, customClass, { 'btn-block': fullWidth, disabled: disabled }]"
    :disabled="disabled"
    @click="handleClick"
  >
    <div class="ms-button-content d-flex align-items-center">
      <!-- Icon -->
      <i v-if="icon" class="ms-button-icon icon" :class="icon"></i>
      <!-- Text -->
      <span v-if="!onlyIcon" class="ms-button-text mt-3">{{ text }}</span>
    </div>
  </button>
</template>

<script setup>
/**
 * Created by: Duchc - Hoàng Chí Đức - 24/11/2025
 */
import { computed } from "vue";

const props = defineProps({
  type: { type: String, default: "primary" }, // primary, secondary, secondary3
  icon: { type: String, default: "" },
  text: { type: String, default: "" },
  fullWidth: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false },
  customClass: { type: String, default: "" },
  onlyIcon: { type: Boolean, default: false }
});

const emit = defineEmits(["click"]);

const buttonClass = computed(() => {
  switch (props.type) {
    case "primary":
      return "btn-primary";
    case "secondary":
      return "btn-secondary";
    case "secondary3":
      return "btn-secondary3";
    default:
      return "btn-primary";
  }
});

const handleClick = (event) => {
  if (!props.disabled) {
    // phát ra event kèm native event để có thể dùng preventDefault
    emit("click", event);
  }
};
</script>
