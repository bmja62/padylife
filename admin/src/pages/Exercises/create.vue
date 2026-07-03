<script setup lang="ts">
import {IExercise} from "@/services/ExerciseService";
import CreateExerciseForm from "@/components/Exercise/CreateExerciseForm.vue";
import ExerciseSteps from "@/components/Exercise/ExerciseSteps.vue";
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";

const spinner = useSpinner()
const exercisePayload = ref<IExercise>({
  title: "",
  exerciseType: "",
  documentLink: "",
  exerciseGoal: "",
  practiceMethod: "",
  imageUrl: "",
  exerciseCount: null,
  exerciseEstimate: null,
  exerciseCategoryId: null,
  stepIds: []
})
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const currentSlug = ref('CreateExerciseForm')
const router = useRouter()

function setStepIds(stepIds: []) {
  exercisePayload.value.stepIds = stepIds
  createExercise()
}

function changeSlug(slugName: string) {
  currentSlug.value = slugName
}

async function createExercise() {
  try {
    spinner.showSpinner()
    const data = await $api?.exercise.createExercise(exercisePayload.value)
    if (data.data.isSuccess) {
      alert.success('تمرین با موفقیت ساخته شد')
      router.push('/Exercises/list')
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
  >
    <template #title>
      ایجاد یک تمرین جدید
    </template>

    <CreateExerciseForm v-if="currentSlug==='CreateExerciseForm'" v-model="exercisePayload"
                                @changeSlug="changeSlug"></CreateExerciseForm>
    <ExerciseSteps @changeSlug="changeSlug" @setStepIds="setStepIds" v-if="currentSlug==='ExerciseSteps'"
                   :steps-count="+exercisePayload.exerciseCount"></ExerciseSteps>
  </PageWrapper>
</template>

<style scoped lang="scss">

</style>
