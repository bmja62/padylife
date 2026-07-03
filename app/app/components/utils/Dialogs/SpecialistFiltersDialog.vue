<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const {$api}: IApiProvider = useNuxtApp()
const isExpertCompanions = ref(false)
const emits = defineEmits<{
  (e: 'setFilter', isExpertCompanions: boolean): void
}>()

function setSelectedCategoryFilter() {
  emits('setFilter', isExpertCompanions.value)
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="specialistFilters" v-model="isRendering">
    <template #title>
      <span>فیلتر متخصصین</span>
    </template>
    <template #default>
      <div class="w-full flex items-center justify-between">
        <span>دارای همراه فعال</span>
        <input type="checkbox" v-model="isExpertCompanions" class="toggle toggle-primary"/>
      </div>
      <button type="button" class="btn btn-primary w-full mt-2 " @click="setSelectedCategoryFilter">اعمال</button>

    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>