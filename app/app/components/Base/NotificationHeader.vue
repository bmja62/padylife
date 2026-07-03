<script setup lang="ts">
const router = useRouter();

function goBack() {
  router.go(-1);
}
</script>

<template>
  <header class="w-full flex items-center justify-between px-5 py-6">
    <div class="grid grid-cols-12 gap-3 ">
      <div class="col-span-2">
      <button
        type="button"
        class="w-8 h-8 flex cursor-pointer flex-row justify-center items-center p-1 rounded-full bg-white"
        @click="goBack"
      >
        <Icon name="icon:chevron-right" size="15" />
      </button>
      </div>
      <strong class="text-white self-center col-span-10">
        <slot name="title" />
      </strong>
    </div>
    <div class="flex items-center gap-2">
    <LazyDashboardBasketIdentifier></LazyDashboardBasketIdentifier>
    <LazyDashboardNotificationIdentifier></LazyDashboardNotificationIdentifier>
    </div>
  </header>
</template>
