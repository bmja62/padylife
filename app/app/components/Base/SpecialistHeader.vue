<script setup lang="ts">
interface IEmits {
  (e: "goBack"): void;
}

const emits = defineEmits<IEmits>();

function emitBackButton() {
  emits("goBack");
}
</script>

<template>
  <header class="w-full flex items-center justify-between px-5 py-6">
    <div class="flex flex-row justify-center items-center gap-x-2">
      <button
        type="button"
        class="w-8 h-8 flex flex-row cursor-pointer justify-center items-center p-1 rounded-full border bg-white"
        @click="emitBackButton"
      >
        <Icon name="icon:chevron-right" size="15" />
      </button>
      <slot name="title"></slot>
    </div>
    <div class="flex items-center gap-2">
      <LazyDashboardBasketIdentifier></LazyDashboardBasketIdentifier>
      <LazyDashboardNotificationIdentifier></LazyDashboardNotificationIdentifier>
    </div>
  </header>
</template>
