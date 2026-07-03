<script setup lang="ts">
const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'continue'): void
}>()
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="welcomeBack" v-model="isRendering">
    <template #title>
      <span>قصد هدایت به کدام قسمت را دارید؟</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">

        <nuxt-link @click="isRendering = false" to="/dashboard/plans/add" class="btn btn-primary">
          برنامه‌ها
        </nuxt-link>
        <nuxt-link @click="isRendering = false" to="/dashboard/challenges" class="btn btn-primary">
          چالش‌ها
        </nuxt-link>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>