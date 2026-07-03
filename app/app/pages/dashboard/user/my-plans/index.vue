<script setup lang="ts">
import type { ICoursesProgressReport } from "~/services/ReportService";

definePageMeta({
  layout: "dashboard",
});
const { $api } = useNuxtApp()
const authStore = useAuthStore()
const coursesProgressReport = ref<ICoursesProgressReport>(null)
const router = useRouter();
function goBack() {
  router.go(-1);
}
useHead({
  title: 'برنامه های من'
})

async function getCourseProgressReport() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getCourseProgressReport(authStore.getUser.id)
    coursesProgressReport.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getCourseProgressReport()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">برنامه‌های من</p>
          </template>
        </BaseSpecialistHeader>
      </template>

      <div class="flex flex-col gap-4">
        <div class="w-full flex flex-row justify-start items-center">
          <strong class="text-gray-700">برنامه‌های در حال انجام</strong>
        </div>
        <div v-if="coursesProgressReport?.plansProgress?.length" class="w-full grid grid-cols-4 gap-4">
          <LazySharedUserPlanCard :course="course"
            v-for="course in coursesProgressReport?.plansProgress.filter(e => e.progressPercentage < 100)"  />

        </div>
        <span v-else class="text-gray-400 text-center">برنامه فعالی یافت نشد</span>

        <div class="w-full flex flex-row justify-start items-center">
          <strong class="text-gray-700">برنامه‌های تکمیل‌شده</strong>
        </div>
        <div v-if="coursesProgressReport?.plansProgress?.length" class="w-full grid grid-cols-4 gap-4">
          <LazySharedUserPlanCard :course="course"
            v-for="course in coursesProgressReport?.plansProgress.filter(e => e.progressPercentage === 100)" />
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.plans-custom-shadow {
  box-shadow: 0px 2px 6px 2px rgba(60, 64, 67, 0.15),
    0px 1px 2px 0px rgba(60, 64, 67, 0.3);
}
</style>
