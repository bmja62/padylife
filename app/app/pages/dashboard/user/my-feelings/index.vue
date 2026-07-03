<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IDailyFeeling, IGetDailyFeelingsParams} from "~/services/DailyFeelings";
import CustomDatePicker from "~/components/utils/CustomDatePicker.vue";

definePageMeta({
  auth: true
})

definePageMeta({
  layout: "dashboard",
});
useHead({
  title: 'احساسات ثبت شده من'
})
const {$api} = useNuxtApp<IApiProvider>()
const authStore = useAuthStore()
const dailyFeelingsFilters = ref<IGetDailyFeelingsParams>({
  pageNumber: 1,
  count: 10,
  userId: authStore.getUser.id,
  startDate: '',
  endDate: ''
})
const dailyFeelingsList = ref<IDailyFeeling[]>([])
const totalCount = ref(null)


async function getAllDailyFeelings() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.dailyFeelings.getAllDailyFeelings(dailyFeelingsFilters.value)
    dailyFeelingsList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getAllDailyFeelings()

function changePage(page: number) {
  dailyFeelingsFilters.value.pageNumber = page
  getAllDailyFeelings()
}

watch(() => dailyFeelingsFilters.value.startDate, async (val) => {
    dailyFeelingsFilters.value.pageNumber = 1
    getAllDailyFeelings()
})
watch(() => dailyFeelingsFilters.value.endDate, async (val) => {
    dailyFeelingsFilters.value.pageNumber = 1
    getAllDailyFeelings()
})
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>احساسات ثبت شده من</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <div class="w-full grid grid-cols-12 gap-2">
          <CustomDatePicker id="startDate" name="startDate"  class="col-span-6" v-model="dailyFeelingsFilters.startDate" simple placeholder=" تاریخ شروع"
                            label="تاریخ شروع"></CustomDatePicker>
          <CustomDatePicker id="endDate" name="endDate" class="col-span-6" v-model="dailyFeelingsFilters.endDate" simple placeholder=" تاریخ پایان"
                            label="تاریخ پایان"></CustomDatePicker>
        </div>

        <div class="w-full flex flex-col gap-y-4 py-5">
          <template v-if="dailyFeelingsList.length">
            <LazyDashboardDailyFeelingsCard
                v-for="dailyFeeling in dailyFeelingsList"
                :key="dailyFeeling.id"
                :dailyFeeling="dailyFeeling"
            >
            </LazyDashboardDailyFeelingsCard>
            <div class="w-full flex items-center justify-center">
              <UtilsCustomPagination
                  :page-number="dailyFeelingsFilters.pageNumber"
                  :count="dailyFeelingsFilters.count"
                  :total-count="totalCount"
                  @change-page="changePage"
              />
            </div>
          </template>
          <span v-else>  موردی یافت نشد</span>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
