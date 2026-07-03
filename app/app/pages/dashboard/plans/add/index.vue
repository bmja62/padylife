<script setup lang="ts">
import type {IGetPlansForUIParams, IPlan} from "~/services/PlanService";
import type {IApiProvider} from "~/models/IApiProvider";

definePageMeta({
  layout: "dashboard",
  auth:true
});
useHead({
  title: 'افزودن برنامه جدید'
})
const {$api} = useNuxtApp<IApiProvider>()

// Variables
const isRenderingFiltersDialog = ref(false)
const plans = ref<IPlan[]>([])
const totalCount = ref(null)
const debounceTimeout = ref(null)
const plansFilters = ref<IGetPlansForUIParams>({
  search: '',
  pageNumber: 1,
  count: 10,
  containSginUpPlans:false,
  containUserPlans: true,
  categoryId: null
})

async function getAllPlansForUI() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getAllPlansForUI(plansFilters.value)
    plans.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (e) {
    console.error()
  } finally {
    useSpinner().hideSpinner()
  }
}

const debouncedSearch = computed({
  get() {
    return plansFilters.value.search;
  },
  set(val) {
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }
    debounceTimeout.value = setTimeout(() => {
      plansFilters.value.search = val
      plansFilters.value.pageNumber = 1
      getAllPlansForUI()
    }, 600);
  },
});
getAllPlansForUI()

function changePage(page: number) {
  plansFilters.value.pageNumber = page
  getAllPlansForUI()
}


function setFilters(categoryId: number) {
  plansFilters.value.pageNumber = 1
  plansFilters.value.categoryId = categoryId
  isRenderingFiltersDialog.value = false
  getAllPlansForUI()
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 147px">
      <template #header>
        <BaseNotificationHeader>
          <template #title> افزودن برنامه جدید </template>
        </BaseNotificationHeader>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start px-4 pt-5 gap-y-4"
      >
        <div class="w-full grid grid-cols-12 gap-2">
          <input
              v-model="debouncedSearch"
              class="col-span-10 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
            placeholder="جستجو در برنامه‌ها"
          />
          <div class="col-span-1 flex flex-row justify-start gap-2">
            <div
                @click="isRenderingFiltersDialog = true"
                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"
            >
              <Icon name="icon:filter" size="20" class="[&_*]:stroke-black"/>
            </div>
            <!--                      <div-->
            <!--                        class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"-->
            <!--                      >-->
            <!--                        <Icon name="icon:sort" size="20" class="[&_*]:stroke-black" />-->
            <!--                      </div>-->
          </div>
        </div>
        <div v-if="plans.length" class="w-full flex flex-col gap-y-4">
          <LazyDashboardPlanDropDown
              v-for="plan in plans"
              :key="plan.id"
              :plan="plan"
          />
          <div class="w-full flex items-center justify-center">
            <UtilsCustomPagination
                :page-number="plansFilters.pageNumber"
                :count="plansFilters.count"
                :total-count="totalCount"
                @change-page="changePage"
            />
          </div>
        </div>
        <span v-else>برنامه فعالی یافت نشد</span>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsPlanFiltersDialog @setFilter="setFilters"
                                       v-model="isRenderingFiltersDialog"></LazyUtilsDialogsPlanFiltersDialog>

  </div>
</template>
