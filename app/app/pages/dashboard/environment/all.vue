<script setup lang="ts">

definePageMeta({
  auth: true
})

definePageMeta({
  layout: "dashboard",
});
useHead({
  title: 'اخبار زیست محیطی'
})

</script>

<template>
  <LazySharedEnvironmentNewsPage></LazySharedEnvironmentNewsPage>
</template>

<style scoped></style>
