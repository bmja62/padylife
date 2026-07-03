<script setup lang="ts">
import type {IUserPlan} from "~/services/PlanService";

definePageMeta({
  auth: true,
  layout: "dashboard",

})
useHead({
  title:'داشبورد'
})

const spinner = useSpinner()
const {$api} = useNuxtApp()
const authStore = useAuthStore()
const userPlans = ref<IUserPlan[]>([])
async function getUserPlans() {
  try {
    spinner.renderSpinner()
    const response = await $api.plan.getUserPlans({
      userId: authStore.getUser.id,
      pageNumber: 1,
      count: 10
    })
    if (response.isSuccess) {
      userPlans.value = response.data.data
    } else {
      alerts.error(response.data)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    spinner.hideSpinner()
  }
}

getUserPlans()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 96px">
      <template #header>
        <DashboardHeader></DashboardHeader>
      </template>
<LazyDashboardEarthSection></LazyDashboardEarthSection>
      <LazyDashboardDailyFeelings></LazyDashboardDailyFeelings>
      <div class="w-full px-5">

        <div v-for="plan  in userPlans" :key="plan.planId" class="space-y-[10px] h-auto relative">
          <div
            class="h-full border-r border-primary absolute top-0 right-8 z-[1]"
          ></div>
          <CoursesCourseDetailCard
              v-for="(exercise,idx) in plan.lastAnswerExercises"
              :key="exercise.id"
              :user-plan-id="plan.userPlanId"
            :image-url="exercise.imageUrl"
            class="z-10"
            :stars-count="3"
          >
            <template #title> {{ exercise.title }} </template>
            <template #description> {{ idx+1 }} از {{ plan.lastAnswerExercises.length }} </template>
          </CoursesCourseDetailCard>

        </div>

        <ClientOnly>
          <LazySharedBabakTheSloth slogan="سلام من بابک هستم. همراه هوشمند تو در پادی لایف"></LazySharedBabakTheSloth>
        </ClientOnly>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>

</style>
