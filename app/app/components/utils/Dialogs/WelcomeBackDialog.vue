<script setup lang="ts">
const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'continue'): void
}>()
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="welcomeBack" v-model="isRendering">
    <template #title>
      <span>خوش اومدی!</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">
       <span>چه خوب که دوباره اینجایی!😊
همه چیز مثل قبل آمادس، پس بیا ادامه بدیم!

اگر کمکی یا سوالی داری، ما همیشه کنارت هستیم.</span>

        <button type="button" class="btn btn-primary" @click="emits('continue')">
          ورود به اپلیکیشن
        </button>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>