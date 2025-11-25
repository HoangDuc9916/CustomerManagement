<template>
  <div class="customer-table-wrapper">
    <div class="table-container">
      <table class="ms-table">
        <thead></thead>
      </table>
    </div>
  </div>

  <!-- Paging -->
    <!-- <div class="paging-wrapper">
      <div class="paging-left">Tổng số: <span class="strong-total">{{ total }}</span></div>

      <div class="paging-right-container" ref="pageMenuRef">
        <div class="paging-center select-style" @click.stop="togglePageMenu">
          {{ pageSize }} Bản ghi trên trang
          <span class="arrow">▾</span>
          <div v-if="showPageMenu" class="page-size-menu select-style">
            <div class="menu-item" @click.stop="setPageSize(10)">10 Bản ghi <span v-if="pageSize === 10">✓</span></div>
            <div class="menu-item" @click.stop="setPageSize(20)">20 Bản ghi <span v-if="pageSize === 20">✓</span></div>
            <div class="menu-item" @click.stop="setPageSize(50)">50 Bản ghi <span v-if="pageSize === 50">✓</span></div>
            <div class="menu-item" @click.stop="setPageSize(100)">100 Bản ghi <span v-if="pageSize === 100">✓</span>
            </div>
          </div>
        </div>

        <div class="paging-nav">
          <button @click="prevPage" :disabled="page === 1">&lt;</button>
          <span>{{ pageStart }} - {{ pageEnd }} </span>
          <button @click="nextPage" :disabled="page === totalPage">&gt;</button>
        </div>
      </div>
    </div> -->
</template>
