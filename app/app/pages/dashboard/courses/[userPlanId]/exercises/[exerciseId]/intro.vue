<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IExerciseDetail} from "~/services/ExerciseService";

const router = useRouter();
const route = useRoute()
const currentExercise = ref<IExerciseDetail>(null)
const {$api}: IApiProvider = useNuxtApp()

function goBack() {
  router.back(-1);
}

definePageMeta({
  auth: true
})


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

onMounted(() => {
  getCurrentExerciseDetail()
})
function nextStep() {
  router.push(`/dashboard/courses/${route.params.userPlanId}/exercises/${route.params.exerciseId}/companions`);
}
useHead({
  title:'معرفی تمرین'
})

</script>

<template>
  <div
      class="w-full h-full custom-pattern-bg-image overflow-y-auto overflow-x-hidden"
  >
    <BaseMinimalHeader @go-back="goBack"></BaseMinimalHeader>
    <div
        v-if="currentExercise"
        class="w-full h-[calc(100svh-64px)] bg-[#F7F8FE] px-5 py-4 rounded-t-[32px] flex flex-col justify-between"
    >
      <div class="px-5">
        <p class="pt-4 text-[#333333] font-medium text-justify">
          لطفا قبل از شروع، متن زیر را مطالعه کنید.
        </p>
        <h4 class="my-6">توضیحات تمرین</h4>

        <p style="overflow-wrap: anywhere" v-html="currentExercise.practiceMethod"></p>

        <div class="flex flex-col gap-y-4 my-6">
          <div class="flex items-center gap-x-3">
            <div
                class="w-10 h-10 rounded-full flex items-center justify-center border border-[#212121]"
            >
              <Icon name="icon:page" size="26" class="[&_*]:stroke-[#212121]"/>
            </div>
            <p class="text-[#565E6D]">
              <span class="ml-1"> {{ currentExercise.exerciseCount }} </span>

              مرحله
            </p>
          </div>

          <div class="flex items-center gap-x-3">
            <div
                class="w-10 h-10 rounded-full flex items-center justify-center border border-[#212121]"
            >
              <Icon
                  name="icon:clock"
                  size="26"
                  class="[&_*]:stroke-[#212121]"
              />
            </div>
            <p class="text-[#565E6D]">
              <bdi class="ml-1"> {{ currentExercise.exerciseEstimate }}</bdi>
            </p>
          </div>

        </div>
      </div>
      <button
          type="button"
          class="w-full btn btn-primary bottom-0 !rounded-[28px]"
          @click="nextStep"
      >
        بعدی
      </button>
    </div>
  </div>
</template>
