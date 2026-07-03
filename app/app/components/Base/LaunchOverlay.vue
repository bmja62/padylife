<script setup lang="ts">
const isRenderingLaunchPage = ref<boolean>(true);

onMounted(() => {
  setTimeout(() => {
    isRenderingLaunchPage.value = false;
  }, 1000);
});
</script>

<template>
  <div
    v-show="isRenderingLaunchPage"
    class="h-screen bg-primary flex items-center justify-center"
  >
    <Icon name="icon:logo-typography-vertical" size="250" color="white" />
  </div>
</template>
