<template>
  <header class="app-header d-flex flex-direction-row justify-content-space-between items-center">
    <div class="d-flex items-center gap-4 mt-8">
      <div class="page-start d-flex ">
        <div class="page-start-logo icon-logo"></div>
        <span class="page-start-text"></span>
      </div>
      <div class="menu-container d-flex gap-4">
        <ul class="menu-list">
          <li class="menu-list-item" v-for="(menu, index) in menus" :key="index">
            <a href="#" class="item-link" @click.prevent="setActive(index)">
              <div class="item-element"
                :class="[menu.margin ? menu.margin : '', { 'item-active-text': activeIndex === index }]">
                <div class="item-element-icon icon" :class="[menu.icon, { 'item-active-icon': activeIndex === index }]">
                </div>
                <span class="item-element-text ml-8">{{ menu.name }}</span>
              </div>

            </a>
          </li>
        </ul>

        <div class="menu-dot d-flex items-center">
          <i class="menu-dot-icon icon icon-other-grey"></i>
          <div class="menu-dot-text mt-3">Tất cả</div>
        </div>
      </div>
    </div>

    <div class="avatar-account mt-8">
      <img src="../assets/images/avartar.png" alt="Avatar" class="avatar-image" />
    </div>
  </header>
</template>

<script>
export default {
  data() {
    return {
      activeIndex: 1,
      menus: [
        { name: "Bàn làm việc", icon: "icon-dashboard", margin: "" },
        { name: "Khách hàng", icon: "icon-customers" },
        { name: "Cơ hội", icon: "icon-opportunitys" },
        { name: "Báo giá", icon: "icon-quote" },
        { name: "Đơn hàng", icon: "icon-sale-order" },
        { name: "Hàng hóa", icon: "icon-products" },
        { name: "Báo cáo", icon: "icon-report" }
      ]
    }
  },
  methods: {
    setActive(index) {
      this.activeIndex = index
    }
  }
}
</script>

<style scoped>

.app-header {
  background-color: #ffffff;
  height: 48px;
}

.page-start-logo {
  margin-left: 52px;
}

.page-start-text {
  font-family: MyFont, Arial, Helvetica, sans-serif;
  margin-top: 7px;
  margin-left: 13px;

  font-family: MyFont, Arial, Helvetica, sans-serif;
  color: #111827;
}

.page-start-text::before {
  content: "CRM";
  font-weight: 600;
  font-size: 17px;
   text-shadow:  0.02em 0 currentColor;
}

.menu-list {
  overflow: hidden;
  display: flex;
  margin-left: 16px;
}

.menu-list-item {
  list-style: none;
  align-items: center;

}

.item-link {
  border-radius: 4px;

  padding: 0 8px;
  height: 32px;
  text-decoration: none;
  font-size: 13px;
  color: #586074;
  display: inline-block;
  font-family: MyFont, Arial, Helvetica, sans-serif;
  opacity: 1;
}

.item-element {
  margin-top: 2px;
  width: 100%;
  height: 32px;
  color: #1f2229;
  line-height: 32px;
  display: flex;
}

.item-element-icon {
  padding: 0;
  margin: 0;
  margin-top: 5px; /* icon đi xuống 2px */
  display: inline-block; /* để margin-top áp dụng trong flex */
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
}



.menu-dot {
  cursor: pointer;
  border-radius: 4px;
  width: auto;
  margin-top: 7px;
  margin-left: 12px;
  background-position: center left;
}

.menu-dot-icon {
  padding: 0;
  margin: 0;
  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
}

.menu-dot-text {
  margin-left: 8px;

  font-size: 13px;
  font-family: MyFont, Arial, Helvetica, sans-serif;
}

.avatar-account {
  border-radius: 50%;
  cursor: pointer;
  width: 32px;
  height: 32px;
  overflow: hidden;
  border: none;
  margin-right: 20px;
}

.avatar-image {
  width: 100%;
  height: 100%;
  max-width: 32px;
  max-height: 32px;
}

img {
  vertical-align: middle;
  border: 0;
}

.item-active-icon {
  background-color: #4262f0;
}

.item-active-text {
  color: #4262f0;
}
.item-element-text {
  margin-top: 1px;
  font-size: 13px;

}

.item-element.item-active-text {
  position: relative;
}

.item-element.item-active-text::after {
  content: "";
  position: absolute;
  left: 0;

  width: 100%;
  height: 2px;
  background-color: #327bff;
  border-radius: 1px;
   bottom: -6px;
}
</style>
