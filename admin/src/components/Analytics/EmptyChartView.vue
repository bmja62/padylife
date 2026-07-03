<script setup lang="ts">
const props = defineProps({
  icon: {
    type: String as PropType<string>,
    default: 'tabler-chart-line'
  }
})
</script>

<template>
  <div class="d-flex w-100 flex-column justify-space-center align-content-center flex-wrap gap-2">
    <VIcon style="width: 10rem;height: 10rem" :icon="props.icon"></VIcon>
    <span class="text-center">داده ای یافت نشد</span>
  </div>
</template>

<style scoped>

</style>
