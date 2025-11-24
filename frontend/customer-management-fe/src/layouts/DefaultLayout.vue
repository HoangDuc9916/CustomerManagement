<template>
  <div class="default-layout">
    <AppHeader />
    <main class="content">
      <!-- CustomerView vẫn hiện khi currentView === 'list' -->
      <CustomerView v-if="currentView === 'list'" ref="customerViewRef" @add="currentView = 'add'"
        @edit-customer="onEditCustomer" />

      <!-- CustomerForm hiện khi currentView === 'add' -->
      <CustomerForm v-if="currentView === 'add' || currentView === 'edit'" ref="customerFormRef"
        @cancel="currentView = 'list'" @saved="onCustomerSaved" @added-and-continue="onCustomerAddedAndContinue" />

      <!-- Toast -->
      <ToastMessage v-if="toast.visible" :type="toast.type" :message="toast.message" />
    </main>
  </div>
</template>

<script setup>
import { ref, reactive, nextTick, watch } from 'vue';
import CustomerForm from '@/components/customer/CustomerForm.vue';
import AppHeader from '@/layouts/AppHeader.vue';
import CustomerView from '@/views/customer/CustomerView.vue';
import ToastMessage from '@/components/common/ToastMessage.vue';

const currentView = ref("list"); // list | add | edit
const customerFormRef = ref(null);

watch(currentView, async (newView) => {
  if (newView === 'add') {
    await nextTick(); // DOM render xong
    if (customerFormRef.value?.prepareNewCustomer) {
      customerFormRef.value.prepareNewCustomer();
    } else {
      // fallback nếu ref chưa mount
      const unwatch = watch(customerFormRef, async (val) => {
        if (val?.prepareNewCustomer) {
          await val.prepareNewCustomer();
          unwatch(); // stop watching
        }
      });
    }
  }
});

// State toast
const toast = reactive({
  visible: false,
  message: "",
  type: "info"
});

// Ref tới table

const customerViewRef = ref(null);

const onCustomerSaved = ({ customer: newCustomer, action }) => {
  if(action === "edit"){
    customerViewRef.value?.$refs.customerTableRef.updateCustomerInTable(newCustomer); // nếu có hàm update
  } else {
    customerViewRef.value?.$refs.customerTableRef.addCustomerToTop(newCustomer);
  }

  toast.message = action === "edit" ? "Cập nhật khách hàng thành công!" : "Thêm khách hàng thành công!";
  toast.type = action === "edit" ? "edit" : "add";
  toast.visible = true;
  setTimeout(() => { toast.visible = false; }, 5000);

  currentView.value = 'list';
};

const onCustomerAddedAndContinue = ({ customer: newCustomer, action }) => {
  customerViewRef.value?.addCustomerToTop(newCustomer);

  toast.message = action === "edit" ? "Cập nhật khách hàng thành công!" : "Thêm khách hàng thành công!";
  toast.type = action === "edit" ? "edit" : "add";
  toast.visible = true;
  setTimeout(() => { toast.visible = false; }, 5000);
};


//EDIT LOAD DỮ LIỆU
const onEditCustomer = async (customerId) => {
  currentView.value = 'edit';

  await nextTick();

  if (customerFormRef.value) {
    await customerFormRef.value.loadCustomer(customerId);
  }
};
</script>
