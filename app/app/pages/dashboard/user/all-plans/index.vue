<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";
import type { IUserPlan } from "~/services/PlanService";

definePageMeta({
  layout: "dashboard",
  auth: true
});
useHead({
  title: 'برنامه‌ها'
})
const { $api } = useNuxtApp<IApiProvider>()

// Variables
const spinner = useSpinner()
const authStore = useAuthStore()
const userPlans = ref<IUserPlan[]>([])
const filteredPlans = ref<IUserPlan[]>([])
const searchQuery = ref<string>('')
const openPlans = ref<number[]>([])

const togglePlan = (planId: number) => {
  if (openPlans.value.includes(planId)) {
    openPlans.value = openPlans.value.filter(id => id !== planId)
  } else {
    openPlans.value.push(planId)
  }
}

watch(searchQuery, (newQuery) => {
  const query = newQuery.trim();
  if (!query) {
    filteredPlans.value = userPlans.value;
    return;
  }

  filteredPlans.value = userPlans.value.filter(plan =>
    plan.planName?.includes(query) ?? false
  );
}, { immediate: true });

const beforeEnter = (el: Element) => {
  const element = el as HTMLElement
  element.style.height = '0'
}
const enter = (el: Element) => {
  const element = el as HTMLElement
  element.style.height = el.scrollHeight + 'px'
}
const beforeLeave = (el: Element) => {
  const element = el as HTMLElement
  element.style.height = el.scrollHeight + 'px'
}
const leave = (el: Element) => {
  const element = el as HTMLElement
  element.style.height = '0'
}

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
      filteredPlans.value = response.data.data;
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
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 147px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>مشاهده همه برنامه‌ها</template>
        </BaseNotificationHeader>
      </template>

      <div class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start p-2 gap-y-4">
        <div class="w-full grid grid-cols-1 ">
          <input
           v-model="searchQuery"
            class="col-span-10 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
            placeholder="جستجو در برنامه‌ها" />
        </div>
        <div class="w-full px-5">
          <div v-for="plan in filteredPlans" :key="plan.planId" class="space-y-[10px] h-auto relative">
            <div class="cursor-pointer select-none z-10 mt-5" @click="togglePlan(plan.planId)">
              <CoursesCourseCard :plan="plan" class="mt-5 z-10">
                <template #trailing>
                  <div
                    class="flex items-center justify-center w-10 h-10 rounded-full bg-gray-100 transition-colors duration-200"
                    :class="{ 'bg-gray-200': openPlans.includes(plan.planId) }">
                    <svg
                      class="w-5 h-5 text-gray-600 transition-transform duration-300 ease-in-out"
                      :class="{ 'rotate-180': openPlans.includes(plan.planId) }" fill="none" stroke="currentColor"
                      viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>
                </template>
              </CoursesCourseCard>
            </div>
            <transition
              name="accordion" @enter="enter" @leave="leave" @before-enter="beforeEnter"
              @before-leave="beforeLeave">
              <div v-if="openPlans.includes(plan.planId)" class="z-10">
                <div class="overflow-hidden rounded-xl shadow-lg">
                  <div>
                <CoursesCourseDetailCard 
                  v-for="(exercise, idx) in plan.lastAnswerExercises" :key="exercise.id"
                  :user-plan-id="plan.userPlanId" :image-url="exercise.imageUrl" :stars-count="3" class="mt-3">
                  <template #title>{{ exercise.title }}</template>
                  <template #description>
                    {{ idx + 1 }} از {{ plan.lastAnswerExercises.length }}
                  </template>
                </CoursesCourseDetailCard>
                  </div>
                </div>
              </div>
            </transition>
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style>
.accordion-enter-active,
.accordion-leave-active {
  transition: height 0.4s ease-in-out;
  overflow: hidden;
}

.accordion-enter-from,
.accordion-leave-to {
  height: 0 !important;
}
</style>
