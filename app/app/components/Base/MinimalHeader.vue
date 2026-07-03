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
  <div class="w-full flex items-center justify-between py-4 px-5">
    <button
      type="button"
      class="w-8 h-8 flex items-center justify-center bg-white border border-[#D9D9D9] rounded-full"
      @click="emitBackButton"
    >
      <Icon name="icon:chevron-right" size="15" />
    </button>
    <Icon name="icon:logo-typography-horizontal" color="white" />
  </div>
</template>
