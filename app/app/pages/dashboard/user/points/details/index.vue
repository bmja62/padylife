<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";

definePageMeta({
  layout: "dashboard",
  auth: true

});
useHead({
  title: 'امتیازات'
})
const authStore = useAuthStore()
const {$api} = useNuxtApp<IApiProvider>()
const pointsListFilters = ref<IGetnotificationFilters>({
  userId: authStore.getUser.id,
  pageNumber: 1,
  count: 50,
})
const pointsList = ref<any>([])
const pointsSummary = ref<any>(null)
const totalCount = ref(null)

async function getPointsReports() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.users.getUserPointsReport(pointsListFilters.value)
    pointsList.value = response.data.history
    totalCount.value = response.data.totalUsers
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getPointsSummary() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.users.getUserPointsSummary(pointsListFilters.value.userId)
    pointsSummary.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

await getPointsReports()
await getPointsSummary()
function changePage(page: number) {
  pointsListFilters.value.pageNumber = page
  getPointsReports()
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>امتیازات</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start  gap-y-4"
      >

        <div class="w-full flex flex-col gap-y-4">
          <div class="flex items-center gap-2">
            <div v-if="pointsSummary"
                 class="w-1/2 flex flex-row justify-between items-center bg-white rounded-[16px] p-2"
            >
              <div class="flex w-full flex-col items-start gap-y-2">
                <span class="text-gray-500 text-sm">امتیازات کسب شده در هفته</span>
                <div class="w-full flex items-center justify-between">
                  <span class="text-primary text-sm ">{{ pointsSummary.weeklyPoints }} امتیاز </span>
                  <div
                      class="w-8 h-8 flex text-sm items-center justify-center rounded-lg bg-indigo-100 text-indigo-600 font-bold">
                    W
                  </div>
                </div>
              </div>

            </div>
            <div v-if="pointsSummary"
                 class="w-1/2 flex flex-row justify-between items-center bg-white rounded-[16px] p-2"
            >
              <div class="flex w-full flex-col items-start gap-y-2">
                <span class="text-gray-500 text-sm">امتیازات کسب شده در ماه</span>
                <div class="w-full flex items-center justify-between">
                  <span class="text-primary text-sm ">{{ pointsSummary.monthlyPoints }} امتیاز </span>
                  <div
                      class="w-8 h-8 flex text-sm items-center justify-center rounded-lg bg-indigo-100 text-indigo-600 font-bold">
                    M
                  </div>
                </div>
              </div>

            </div>
          </div>
          <div v-if="pointsSummary"
               class="w-full flex flex-row justify-between items-center bg-white rounded-[16px] p-2"
          >
            <div class="flex w-full flex-col items-start gap-y-2">
              <span class="text-gray-500 text-sm">امتیازات کسب شده در کل</span>
              <div class="w-full flex items-center justify-between">
                <span class="text-primary text-sm ">{{ pointsSummary.totalPoints }} امتیاز </span>
                <div
                    class="w-8 h-8 flex text-sm items-center justify-center rounded-lg bg-indigo-100 text-indigo-600 font-bold">
                  T</div>
              </div>
            </div>

          </div>

          <template v-if="pointsList?.length">
            <div
                v-for="(point, index) in pointsList"
                class="w-full bg-white shadow rounded-2xl p-3 flex items-center gap-2">
              <div class="w-1/8">
                <Icon
                    name="icon:mail-fill"
                    color="#8C8C8C"
                    class="w-5 h-5"
                />
              </div>
              <div class="border-r-2  w-full border-gray-200 pr-3  flex flex-col gap-2">
                <div class="w-full flex items-center justify-between">
                  <span class="line-clamp-1 text-primary">{{ point.pointsEarned }} امتیاز</span>
                  <div class="flex items-center gap-2">
                    <Icon name="icon:black-clock" size="20"/>
                    <span>{{ new Date(point.date).toLocaleDateString('fa-IR') }}</span>
                  </div>
                </div>
                <div v-if="point.hourlyDetails" class="w-full flex flex-col gap-2 text-justify">
                <span class="pl-2 " v-for="reason in point.hourlyDetails.map((e)=> e.reasons)[0]">
                  {{ reason }}
                </span>
                </div>
              </div>
            </div>
            <div class="w-full flex items-center justify-center">
              <UtilsCustomPagination
                  :page-number="pointsListFilters.pageNumber"
                  :count="pointsListFilters.count"
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
