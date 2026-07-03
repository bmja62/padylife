<script setup lang="ts">
const route = useRoute();

function share() {
  navigator.share({
    title: 'پادی لایف',
    url: route.fullPath
  })
}
</script>

<template>
  <button
      type="button"
      class="btn w-full bg-white border border-primary mb-2 !text-primary !rounded-[28px] mt-auto"
      @click="share"
  >
    اشتراک گذاری
  </button>
</template>

<style scoped>

</style>