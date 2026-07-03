<script setup lang="ts">
import {IGetExercise, IUpdateExercise} from "@/services/ExerciseService";
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";

const spinner = useSpinner()

const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const router = useRouter()
const route = useRoute()
const currentExercise = ref<IGetExercise | null>(null)
const exercisePayload = ref<IUpdateExercise>({
  id: null,
  title: "",
  exerciseType: "",
  imageUrl: '',
  documentLink: "",
  exerciseGoal: "",
  practiceMethod: "",
  exerciseCount: 0,
  exerciseEstimate: 0,
  exerciseCategoryId: 0
})

function generateUpdatePayload() {
  Object.keys(exercisePayload.value).forEach((key) => {
    if (currentExercise.value[key]) {
      exercisePayload.value[key] = currentExercise.value[key]
    }
  })
  updateExercise()
}

async function updateExercise() {
  try {
    spinner.showSpinner()
    const data = await $api?.exercise.updateExercise(exercisePayload.value)
    if (data.data.isSuccess) {
      alert.success('تمرین با موفقیت بروزرسانی شد')
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

async function getExerciseById() {
  try {
    spinner.showSpinner()
    const data = await $api?.exercise.getExerciseById(route.params.id)
    if (data.data.isSuccess) {
      currentExercise.value = data.data.data
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

onMounted(() => {
  getExerciseById()
})
</script>

<template>
  <PageWrapper
    v-if="currentExercise"
  >
    <template #title>
      ویرایش {{ currentExercise.title }}
    </template>

    <UpdateExerciseForm @updateExercise="generateUpdatePayload" v-model="currentExercise"
                        @refetch="getExerciseById"></UpdateExerciseForm>
  </PageWrapper>
</template>

<style scoped lang="scss">

</style>
