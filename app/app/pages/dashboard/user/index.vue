<script setup lang="ts">
import type { IActivityReport } from "~/services/ReportService";

const authStore = useAuthStore()
definePageMeta({
  layout: "dashboard",
  auth: true
});

useHead({
  title: 'اطلاعات کاربری'
})
const { $api } = useNuxtApp()
const activityReport = ref<IActivityReport>(null)

async function getUserActivityReport() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getUserActivityReport(authStore.getUser.id)
    activityReport.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getUserActivityReport()

</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper v-if="authStore.isLogged" style="--wrapper-header-height: 274px">
      <template #header>
        <LazyDashboardUserDashboardHeader> </LazyDashboardUserDashboardHeader>
        <div class="w-full flex flex-col items-center gap-y-2 pb-4 bg-cover bg-center bg-no-repeat">
          <LazySharedHonoredUser></LazySharedHonoredUser>
          <strong class="text-[#EFEFEF] text-sm">{{ authStore.getUser.userName }}</strong>
          <div class="w-full grid grid-cols-3">
            <div class="flex flex-col items-center gap-y-2">
              <span class="text-sm text-white">سن</span>
              <span class="text-[#EFEFEF]">{{ authStore.getUser.age }}</span>
            </div>
            <div class="flex flex-col items-center gap-y-2 border-r border-l border-gray-300">
              <span class="text-sm text-white">همراهی میشوم</span>
              <div class="flex flex-row justify-center items-center gap-x-1">
                <span class="text-[#EFEFEF]"> {{ authStore.getUser.supported }} </span>
              </div>
            </div>
            <div class="flex flex-col items-center gap-y-2">
              <span class="text-sm text-white">همراهی میکنم</span>
              <div class="flex flex-row justify-center items-center gap-x-1">
                <span class="text-[#EFEFEF]"> {{ authStore.getUser.accompanied }} </span>
              </div>
            </div>
          </div>
        </div>
      </template>

      <div v-if="activityReport"
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start p-2 gap-y-4">


        <div class="w-full grid grid-cols-2 gap-4">
          <div class="w-full flex flex-row justify-between items-center bg-white rounded-[16px] p-2">
            <div class="flex flex-col items-start gap-y-2">
              <span class="text-gray-500 text-sm">تکمیل شده</span>
              <span class="text-gray-700 text-sm">{{ activityReport.completedExercises }} تمرین</span>
            </div>
            <div class="h-full flex flex-row justify-center items-center">
              <NuxtImg class="w-[30px] h-[30px]" src="/junks/dart-done.png" />
            </div>
          </div>
          <div class="w-full flex flex-row justify-between items-center bg-white rounded-[16px] p-2">
            <div class="flex flex-col items-start gap-y-2">
              <span class="text-gray-500 text-sm">در حال انجام</span>
              <span class="text-gray-700 text-sm">{{ activityReport.totalExercises }} تمرین</span>
            </div>
            <div class="h-full flex flex-row justify-center items-center">
              <NuxtImg class="w-[30px] h-[30px]" src="/junks/dart.png" />
            </div>
          </div>
        </div>

        <div class="w-full flex flex-col bg-white rounded-[16px] p-2 gap-y-2">
          <div class="w-full flex flex-row justify-between items-center">
            <span class="text-gray-500">تبریک!</span>
            <Icon name="icon:black-clock" size="20" />
          </div>
          <div class="w-full flex flex-row justify-start items-center">
            <span class="text-gray-700 text-sm">تو این هفته %{{ Math.ceil(activityReport.exerciseCompletionRate) }}
              بیشتر از
              بقیه برای بهتر شدن خودت تلاش کردی</span>
          </div>
        </div>

        <div class="w-full grid grid-cols-2 gap-4">
          <div class="w-full flex flex-col items-center bg-white rounded-[16px] p-2">
            <NuxtImg src="/junks/happy.png" class="w-[3rem] h-[5rem] object-fit" />
            <span class="text-gray-600 text-sm text-justify">تو از {{ activityReport.leaderPercentile }}% افراد حاضر در
              اپلیکیشن
              بهتر عمل کردی.</span>
          </div>
          <!--          <div-->
          <!--            class="w-full flex flex-col items-center bg-white rounded-[16px] p-2 gap-y-2"-->
          <!--          >-->
          <!--            <span class="text-gray-600 text-sm">پیشرفت وضعیت سلامت</span>-->
          <!--            <ChartsRadialChart />-->
          <!--            &lt;!&ndash; <div class="radial-progress" style="&#45;&#45;value: 70" role="progressbar">-->
          <!--              70%-->
          <!--            </div> &ndash;&gt;-->
          <!--          </div>-->
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>
