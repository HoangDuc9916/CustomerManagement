<template>
  <div class="ms-input-wrapper">
    <!-- Label -->
    <label v-if="label">
      {{ label }}
      <span v-if="required" class="star">*</span>
    </label>

    <!-- Text / Date Input -->
    <input
      v-if="type !== 'select'"
      :type="type"
      :placeholder="placeholder"
      :disabled="disabled"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
      :class="{ 'input-error': error }"
    />

    <!-- Select Input -->
    <select
      v-else
      :disabled="disabled"
      :value="modelValue"
      @change="$emit('update:modelValue', $event.target.value)"
      :class="{ 'input-error': error }"
    >
      <option value="" disabled hidden>Chọn...</option>
      <option v-for="opt in options" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
    </select>

    <!-- Error Message -->
    <span v-if="error" class="error-text">{{ error }}</span>
  </div>
</template>

<script setup>
/**
 * Created by: Duchc - Hoàng Chí Đức - 24112025
 */
defineProps({
  modelValue: [String, Number, null],
  label: { type: String, required: false },
  required: { type: Boolean, default: false },
  type: { type: String, default: "text" }, // text, date, select
  placeholder: { type: String, default: "" },
  disabled: { type: Boolean, default: false },
  error: { type: String, default: "" },
  options: { type: Array, default: () => [] } // cho select
});

defineEmits(["update:modelValue"]);
</script>

<style scoped>
.ms-input-wrapper {
  display: flex;
  flex-direction: column;
  margin-bottom: 16px;
}
.ms-input-wrapper label {
  font-weight: 500;
  margin-bottom: 4px;
}
.ms-input-wrapper .star {
  color: red;
  margin-left: 2px;
}
.ms-input-wrapper input,
.ms-input-wrapper select {
  padding: 6px 12px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
}
.ms-input-wrapper input.input-error,
.ms-input-wrapper select.input-error {
  border-color: red;
}
.ms-input-wrapper .error-text {
  color: red;
  font-size: 12px;
  margin-top: 4px;
}
</style>
