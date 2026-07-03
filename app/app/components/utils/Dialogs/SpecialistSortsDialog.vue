<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const {$api}: IApiProvider = useNuxtApp()
const isPopularExpert = ref(false)
const emits = defineEmits<{
  (e: 'setFilter', isPopularExpert: boolean): void
}>()

function setSelectedCategoryFilter() {
  emits('setFilter', isPopularExpert.value)
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="specialistFilters" v-model="isRendering">
    <template #title>
      <span>مرتب سازی متخصصین</span>
    </template>
    <template #default>
      <div class="w-full flex items-center justify-between">
        <span>بر اساس محبوب ترین متخصص</span>
        <input type="checkbox" v-model="isPopularExpert" class="toggle toggle-primary"/>
      </div>
      <button type="button" class="btn btn-primary w-full mt-2 " @click="setSelectedCategoryFilter">اعمال</button>

    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>