<script setup lang="ts">
interface IProps {
  isLastComponent?: boolean
}

const props = withDefaults(defineProps<IProps>(), {
  isLastComponent: false
})

</script>

<template>
  <div class="w-full h-full flex flex-col">
    <strong class="text-[#303030] mx-3">
      <slot name="title"></slot>
    </strong>
    <div class=" mt-4">
      <slot name="chart"></slot>
    </div>
    <hr class="my-4 bg-[#A9A9A9]" v-show="!isLastComponent">
  </div>

</template>

<style scoped>

</style>