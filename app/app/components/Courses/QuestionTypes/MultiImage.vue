<script setup lang="ts">
const selectedAnswer = ref<number | null>(null);

function selectAnswer(selectedItem: number) {
  selectedAnswer.value = selectedItem;
}
</script>

<template>
  <div class="w-full grid grid-cols-2 gap-4 justify-items-center">
    <div
      v-for="item in 4"
      :key="item"
      class="w-[164px] h-[137px] rounded-[32px] flex items-center justify-center custom-multi-image-shadow"
      :class="
        selectedAnswer == item
          ? 'bg-[#ECEFFF] border-4 border-[#00ABFB]'
          : 'bg-white'
      "
      @click="selectAnswer(item)"
    >
      <img
        :src="
          selectedAnswer == item ? '/junks/dummy-light.png' : '/junks/dummy.png'
        "
      />
    </div>
  </div>
</template>
<style scoped>
.custom-multi-image-shadow {
  box-shadow: 0px 8px 16px -5px rgba(75, 52, 37, 0.5);
}
</style>
