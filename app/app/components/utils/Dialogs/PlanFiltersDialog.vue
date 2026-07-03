<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const {$api}: IApiProvider = useNuxtApp()
const selectedCategoryId = ref(null)
const emits = defineEmits<{
  (e: 'setFilter', selectedCategoryId: string): void
}>()

function setSelectedCategoryFilter() {
emits('setFilter',selectedCategoryId.value)
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog full-height dialog-id="specialistFilters" v-model="isRendering">
    <template #title>
      <span>فیلتر برنامه ها</span>
    </template>
    <template #default>

      <LazyUtilsPickersPlanCategoryPicker  v-model="selectedCategoryId"></LazyUtilsPickersPlanCategoryPicker>
      <button type="button" class="btn btn-primary w-full mt-2 " @click="setSelectedCategoryFilter">اعمال</button>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>