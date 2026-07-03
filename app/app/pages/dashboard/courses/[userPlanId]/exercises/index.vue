<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";
import type { IRelatedPlanDetail, IUserPlanInfo } from "~/services/PlanService";
import type { IExercise, IExerciseDetail } from "~/services/ExerciseService";
import { LazyIconsChevronLeftIcon } from "#components";

definePageMeta({
  layout: "dashboard",
  auth: true
});
useHead({
  title: 'تمرین'
})
const isRenderingPlanLeaderboard = ref<boolean>(true);
const isRenderingRelatedPlanDialog = ref<boolean>(false);
const { $api }: IApiProvider = useNuxtApp()
const relatedPlan = ref<IRelatedPlanDetail>(null)
const leaderboards = ref(null)
const route = useRoute()
onMounted(() => {
  setTimeout(() => {

    getExerciseById()
  }, 300);

});
const router = useRouter();
const planInfo = ref<IUserPlanInfo>(null)
const currentExercise = ref<IExerciseDetail>(null)
function startCourse() {
  router.push(`/dashboard/courses/${route.params.userPlanId}/exercises/${currentExercise.value.id}/companions`);
}

async function getExerciseById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getUserPlanExercises(route.params.userPlanId)
    planInfo.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    nextTick(() => {
      if (planInfo.value.exercises.length) {
        getCurrentExerciseDetail(planInfo.value.exercises[0])
      }
      if (planInfo.value.isCompleted) {
        getPlanRelatedPlans()
      }
    })
    useSpinner().hideSpinner()
  }
}

function renderPlanLeaderboard() {
  isRenderingPlanLeaderboard.value = true

}


async function getCurrentExerciseDetail(exercise: IExercise) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.exercises.getExerciseById(exercise.exerciseId)
    currentExercise.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getPlanRelatedPlans() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanRelatedPlans(planInfo.value.planId)
    relatedPlan.value = response.data
    if (relatedPlan.value.nextPlans?.length) {
      if (!relatedPlan.value.nextPlans[0].hasPlan) {
        isRenderingRelatedPlanDialog.value = true
      }
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <div class="w-full  custom-pattern-bg-image overflow-x-hidden">
    <CoursesHeader></CoursesHeader>
    <ClientOnly>
      <div v-if="currentExercise" class="w-full h-full relative overflow-x-hidden bg-[#F7F8FE] rounded-t-[32px]">
        <div class="w-full px-8 pt-5">
          <div class="w-full flex items-center justify-between">
            <p v-if="planInfo.planLevel">
              <span class="font-bold mr-1"> {{ planInfo.planLevel }} </span>
            </p>
            <!--            <p>-->
            <!--              <span class="font-bold ml-1"> 24 </span>-->
            <!--              امتیاز-->
            <!--            </p>-->
          </div>
          <div class="flex mt-10">
            <div class="w-1/2 pt-8">
              <h3 class="font-semibold">توضیحات مرحله</h3>
              <div class="flex flex-col w-full mt-6 gap-y-5">
                <div class="w-full flex items-center gap-x-4">
                  <div class="w-10 h-10 flex items-center justify-center rounded-full border border-[#333333]">
                    <Icon name="icon:page" class="[&_*]:stroke-[#212121]" />
                  </div>
                  <p>{{ currentExercise.exerciseCount }} مرحله</p>
                </div>

                <div class="w-full flex items-center gap-x-4">
                  <div class="w-10 h-10 flex items-center justify-center rounded-full border border-[#333333]">
                    <Icon name="icon:clock" class="[&_*]:stroke-[#212121]" />
                  </div>
                  <p>{{ currentExercise.exerciseEstimate }}</p>
                </div>

                <div class="w-full flex items-center gap-x-4">
                  <div class="w-10 h-10 flex items-center justify-center rounded-full border border-[#333333]">
                    <Icon name="icon:star-outline-new" color="#212121" />
                  </div>
                  <p>{{ currentExercise.exerciseCount * 10 }} امتیاز</p>
                </div>

                <button type="button" class="bg-[#00ABFB] rounded-[32px] w-[108px] h-9 font-semibold text-white"
                  @click="startCourse">
                  شروع
                </button>
              </div>
              <!--              <ClientOnly>-->
              <!--                <LazySharedBabakTheSloth lvh="h-[23lvh]" slogan="همیشه بهترین خودت باش"></LazySharedBabakTheSloth>-->
              <!--              </ClientOnly>-->
            </div>
            <div class="w-1/2 h-[335px] relative">
              <!--              <CoursesCourseCircle :circle-items="planInfo.exercises"></CoursesCourseCircle>-->
              <LazyCoursesHalfCircle :current-exercise="currentExercise" :circle-items="planInfo.exercises"
                @set-selected-item="getCurrentExerciseDetail"></LazyCoursesHalfCircle>
            </div>
          </div>
          <div class="w-full grid grid-cols-3 justify-items-center">
            <!--            <NuxtLink-->
            <!--                to="/dashboard/environment"-->
            <!--                class="flex flex-col items-center gap-y-4"-->
            <!--            >-->
            <!--              <img-->
            <!--                  class="w-14 h-14 object-contain"-->
            <!--                  src="/common/environment.png"-->
            <!--              />-->
            <!--              <p class="text-sm font-semibold">محیط زیست</p>-->
            <!--            </NuxtLink>-->
            <NuxtLink to="/dashboard/challenges" class="flex flex-col items-center gap-y-4">
              <img class="w-14 h-14 object-contain" src="/common/challenges.png" />
              <p class="text-sm font-semibold">چالش‌ها</p>
            </NuxtLink>
            <NuxtLink :to="`/dashboard/specialists/${planInfo.planId}`" class="flex flex-col items-center gap-y-4">
              <img class="w-14 h-14 object-contain" src="/common/support.png" />
              <p class="text-sm font-semibold">انتخاب متخصص</p>
            </NuxtLink>
            <div @click="renderPlanLeaderboard" class="flex cursor-pointer flex-col items-center gap-y-4">
              <img class="w-14 h-14 object-contain" src="/common/shiny-diamond.webp" />
              <p class="text-sm font-semibold">لیدربورد</p>
            </div>
          </div>
          <div class="divider"></div>
          <template v-if="!isRenderingPlanLeaderboard">
            <strong v-if="planInfo?.planExperts?.length">متخصصان شما برای این پلن</strong>
            <div v-if="planInfo?.planExperts?.length" class="w-full flex flex-col gap-2 my-3">
              <LazyDashboardSpecialistUserPlanSpecialistCard v-for="expert in planInfo.planExperts" :expert="expert">
              </LazyDashboardSpecialistUserPlanSpecialistCard>
            </div>
          </template>

          <div v-if="isRenderingPlanLeaderboard" class="w-full mt-8">
            <div class="w-full flex items-center justify-end">
              <div @click="isRenderingPlanLeaderboard = false">
                <LazyIconsChevronLeftIcon class="fill-primary cursor-pointer" />
              </div>
            </div>
            <LazyDashboardPointsPlanLeaderBoard :planId="planInfo.planId"></LazyDashboardPointsPlanLeaderBoard>
          </div>
        </div>
      </div>
    </ClientOnly>
    <LazyUtilsDialogsRelatedPlansDialog :relatedPlans="relatedPlan" v-model="isRenderingRelatedPlanDialog">
    </LazyUtilsDialogsRelatedPlansDialog>
  </div>
</template>
<style scoped>
@keyframes babak-move {
  0% {
    right: -100%;
  }

  100% {
    right: -2.6rem;
  }
}

.babak-enter-active {
  animation: babak-move 1s ease-out;
}

.babak-leave-active {
  animation: babak-move reverse 1s ease-out;
}
</style>
