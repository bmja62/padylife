<script setup lang="ts">

onMounted(() => {
  useUtils().handlePWAInstallDialog()
})

</script>
<template>
  <div id="mainContainer w-full">
    <main class=" w-full mx-auto ">
      <slot />
    </main>
    <ClientOnly>
      <LazyUtilsDialogsPwaInstallDialog></LazyUtilsDialogsPwaInstallDialog>
    </ClientOnly>
  </div>
</template>
