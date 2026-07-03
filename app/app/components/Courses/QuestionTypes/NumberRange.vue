<script setup lang="ts">
const selectedNumber = ref<number>(1);

function selectNumber(selectedItem: number) {
  selectedNumber.value = selectedItem;
}
</script>

<template>
  <div class="w-full h-auto">
    <p class="font-extrabold text-center text-[#2E2622] text-[120px]">
      {{ selectedNumber }}
    </p>
    <div
      class="w-full h-16 rounded-full bg-white flex items-center justify-start gap-x-5 px-4 overflow-x-auto mt-1"
    >
      <button
        v-for="i in 10"
        :key="i"
        type="button"
        class="flex items-center justify-center font-extrabold text-lg w-[50px] min-w-[50px] min-h-[50px] h-[50px]"
        :class="
          selectedNumber == i
            ? 'bg-[#00ABFB] rounded-full text-white custom-selected-number-shadow'
            : 'text-[#2E2622]'
        "
        @click="selectNumber(i)"
      >
        {{ i }}
      </button>
    </div>
  </div>
</template>
<style scoped>
.custom-selected-number-shadow {
  box-shadow: 0px 0px 0px 4px #c6e9fd;
}
</style>
