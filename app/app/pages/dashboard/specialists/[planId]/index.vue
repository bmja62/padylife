<script setup lang="ts">

import type {IPrice} from "~/services/PlanService";

definePageMeta({
  auth:true
})
useHead({
  title:'متخصص ها'
})

const route = useRoute()
const specialistList = ref<IPrice[]>([]);
const {$api} = useNuxtApp()
const isRenderingSortsDialog = ref(false)
const isRenderingFiltersDialog = ref(false)
const debounceTimeout = ref(null)
const totalCount = ref(false)
const plansListFilter = ref({
  planId: route.params.planId,
  pageNumber: 1,
  count: 10,
  search: '',
  rateFilter: null,
  expertCompanions: null,
})
definePageMeta({
  layout: "dashboard",
});

async function getSpecialistPricesForPlan() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanPricesForUI(plansListFilter.value)
    if (response.isSuccess) {
      specialistList.value = response.data.data
      totalCount.value = response.data.totalCount
    } else {
      useAlerts().error(response.data)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

function setSorts(isPopular) {
  plansListFilter.value.rateFilter = isPopular
  plansListFilter.value.pageNumber = 1
  isRenderingSortsDialog.value = false
  getSpecialistPricesForPlan()
}

function setFilters(isExpert) {
  plansListFilter.value.expertCompanions = isExpert
  plansListFilter.value.pageNumber = 1
  isRenderingFiltersDialog.value = false
  getSpecialistPricesForPlan()
}

function changePage(page) {
  plansListFilter.value.pageNumber = page
  getSpecialistPricesForPlan()
}

const debouncedSearch = computed({
  get() {
    return plansListFilter.value.search;
  },
  set(val) {
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }
    debounceTimeout.value = setTimeout(() => {
      plansListFilter.value.search = val
      plansListFilter.value.pageNumber = 1

      getSpecialistPricesForPlan()
    }, 600);
  },
});

getSpecialistPricesForPlan()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title> انتخاب متخصص </template>
        </BaseNotificationHeader>
      </template>
      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <div class="w-full grid grid-cols-12 gap-2">
          <input
              v-model="debouncedSearch"
            class="col-span-9 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
            placeholder="جستجو در متخصصین"
          />
          <div class="col-span-3 flex flex-row justify-start gap-2">
            <div
                @click="isRenderingFiltersDialog = true"
              class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"
            >
              <Icon name="icon:filter" size="20" class="[&_*]:stroke-black" />
            </div>
            <div
                @click="isRenderingSortsDialog = true"
              class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"
            >
              <Icon name="icon:sort" size="20" class="[&_*]:stroke-black" />
            </div>
          </div>
        </div>

        <div v-if="specialistList?.length" class="w-full flex flex-col gap-y-4">
          <DashboardSpecialistCard
            v-for="(specialist, index) in specialistList"
            :key="index"
            :specialist="specialist"
          >
          </DashboardSpecialistCard>
          <div class="w-full flex items-center justify-center">
            <UtilsCustomPagination
                :page-number="plansListFilter.pageNumber"
                :count="plansListFilter.count"
                :total-count="totalCount"
                @change-page="changePage"
            />
          </div>
        </div>
        <span v-else>هیچ متخصصی روی این پلن قیمت گذاری نکرده است</span>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsSpecialistFiltersDialog @set-filter="setFilters"
                                             v-model="isRenderingFiltersDialog"></LazyUtilsDialogsSpecialistFiltersDialog>
    <LazyUtilsDialogsSpecialistSortsDialog @set-filter="setSorts"
                                           v-model="isRenderingSortsDialog"></LazyUtilsDialogsSpecialistSortsDialog>
  </div>
</template>

<style scoped></style>
