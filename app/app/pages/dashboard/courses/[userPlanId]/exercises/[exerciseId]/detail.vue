<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IStepDetail} from "~/services/StepService";
import type {IExerciseDetail} from "~/services/ExerciseService";

definePageMeta({
  auth: true
})
useHead({
  title:'جزئیات تمرین'
})

const route = useRoute()
const {$api} = useNuxtApp<IApiProvider>()
const stepDetail = ref<IStepDetail>(null)
const currentExercise = ref<IExerciseDetail>(null)

async function getStepDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.step.getStepDetailById(route.params.stepId)
    stepDetail.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getCurrentExerciseDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.exercises.getExerciseById(route.params.exerciseId)
    currentExercise.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

const getCurrentStepIndex = computed(() => {
  const idx = currentExercise.value.exerciseStepsDTOs.findIndex(e => e.stepId === stepDetail.value.id)
  if (idx > -1)
    return idx
})
const getNextStepId = computed(() => {
  if (currentExercise.value.exerciseStepsDTOs[getCurrentStepIndex.value + 1]) {
    return currentExercise.value.exerciseStepsDTOs[getCurrentStepIndex.value + 1]
  }
})

const getParams = computed(() => {
  return route.params
})
watch(() => getParams.value.stepId, async (val) => {
getCurrentExerciseDetail()
getStepDetail()
},{immediate:true})
</script>

<template>
  <NuxtPage
      v-if="currentExercise && stepDetail" :step-detail="stepDetail" :next-step="getNextStepId"
      :current-step-index="getCurrentStepIndex"
      :current-exercise="currentExercise"
      :steps-count="currentExercise.exerciseStepsDTOs.length"></NuxtPage>
</template>

<style scoped>

</style>