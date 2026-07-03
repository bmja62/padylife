<script setup lang="ts">
interface IProps {
  userRole?: UserRole;
}


const props = withDefaults(defineProps<IProps>(), {
  userRole: "user",
});

const isRenderingSidebar = ref<boolean>(false);

function renderSidebar() {
  isRenderingSidebar.value = true;
}
function closeSidebar() {
  isRenderingSidebar.value = false;
}

</script>

<template>
  <header class="w-full flex items-center justify-between px-5 py-6">
    <div class="flex flex-row justify-center items-center gap-x-2">
      <div class="w-8 h-8 flex flex-row cursor-pointer justify-center items-center p-1 rounded-full">
        <Icon name="icon:bars-new" color="white" size="15" @click="renderSidebar" />
      </div>
    </div>
    <template v-if="$slots.title">
      <slot name="title"></slot>
    </template>
    <div class="flex items-center gap-2">
      <LazyDashboardBasketIdentifier></LazyDashboardBasketIdentifier>
      <LazyDashboardNotificationIdentifier></LazyDashboardNotificationIdentifier>
    </div>
    <transition appear name="slide-side" mode="out-in">
      <DashboardUserDashboardSideBar v-if="isRenderingSidebar" :user-role="props.userRole"
        @close-side-bar="closeSidebar">
      </DashboardUserDashboardSideBar>
    </transition>
  </header>
</template>

<style scoped></style>
