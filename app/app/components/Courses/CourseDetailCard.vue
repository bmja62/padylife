<script setup lang="ts">
import type {  IExerciseDetail } from "~/services/ExerciseService";
import type { IApiProvider } from "~/models/IApiProvider";
import type { IRelatedPlanDetail, IUserPlanInfo } from "~/services/PlanService";
interface IProps {
  starsCount: number;
  isLocked?: boolean;
  imageUrl: string;
  userPlanId: number;
}

const { $api }: IApiProvider = useNuxtApp()

const props = withDefaults(defineProps<IProps>(), {
  isLocked: false,
});
const route = useRoute()
const currentExercise = ref<IExerciseDetail>(null)
const planInfo = ref<IUserPlanInfo>(null)
const relatedPlan = ref<IRelatedPlanDetail>(null)
const isRenderingRelatedPlanDialog = ref<boolean>(false);

async function getCurrentExerciseDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.exercises.getExerciseById(planInfo.value.exercises[0].id)
    currentExercise.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getExerciseById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getUserPlanExercises(props.userPlanId)
    planInfo.value = response.data
  } catch (e) {
    console.error(e)
  } finally {

      if (planInfo.value.exercises.length) {
          await getCurrentExerciseDetail()
      }
      if (planInfo.value.isCompleted) {
        getPlanRelatedPlans()
      }
    
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

const router = useRouter();
async function startCourse() {
  try {
    useSpinner().renderSpinner()

    await getExerciseById() 

    if (!currentExercise.value) {
      console.error("No exercise steps available")
      return
    }

    router.push(
      `/dashboard/courses/${props.userPlanId}/exercises/${currentExercise.value.id}/detail/${currentExercise.value.exerciseStepsDTOs[0].stepId}`
    )
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>


<template>
  <div
    class="w-full bg-[#ffff] rounded-xl p-2 flex items-center justify-between relative cursor-pointer custom-course-detail-card-shadow"
    @click="startCourse">
    <div class="flex items-center gap-x-4">
      <NuxtImg v-if="props.imageUrl" :src="props.imageUrl" class="w-12 h-12 rounded-full object-cover" />
      <NuxtImg v-else src="/common/no-image.png" class="w-12 h-12 rounded-full object-cover shadow" />
      <div class="flex flex-col justify-between">
        <p :class="props.isLocked ? 'text-[#8B8B8B]' : 'text-black'">
          <slot name="title" />
        </p>
        <p class="text-sm" :class="props.isLocked ? 'text-[#8B8B8B]' : 'text-black'">
          <slot name="description" />
        </p>
      </div>
    </div>
    <div class="pl-2 space-y-4">
      <div class="flex items-center justify-end">
        <Icon name="icon:chevron-right" :color="props.isLocked ? '#737373' : '#000000'" class="rotate-180" size="15" />
      </div>
      <!--      <div class="flex items-center gap-x-1">-->
      <!--        <Icon-->
      <!--          v-for="(item, index) in 5"-->
      <!--          :key="index"-->
      <!--          name="icon:star"-->
      <!--          size="15"-->
      <!--          :color="index + 1 >= props.starsCount ? '#FFDC18' : '#C0C0C0'"-->
      <!--        />-->
      <!--      </div>-->
    </div>
  </div>
</template>

<style scoped>
.custom-course-detail-card-shadow {
  box-shadow: 0px 1px 2px 0px rgba(60, 64, 67, 0.3),
    0px 2px 6px 2px rgba(60, 64, 67, 0.15);
}
</style>
