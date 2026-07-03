<script setup lang="ts">


import {DailyFeelingsEnum} from "~/models/Enums/DailyFeelings";


const currentFeeling = ref<DailyFeelingsEnum>(DailyFeelingsEnum.Glad);
const feelingPayload = ref({
  type: '',
  description: '',
  voiceUrl: ''
})
const spinner = useSpinner()
const {$api} = useNuxtApp()
const alert = useAlerts()
const isRenderingVoiceRecordModal = ref(false)

function getVoiceMessage(voice: string) {
  feelingPayload.value.voiceUrl = voice
  createDailyFeeling()
}
async function createDailyFeeling() {
  try {
    spinner.renderSpinner()
    const previousFeeling = JSON.parse(JSON.stringify(feelingPayload.value.type))
    feelingPayload.value.type = DailyFeelingsEnum[feelingPayload.value.type]
    const response = await $api.dailyFeelings.createDailyFeeling(feelingPayload.value)
    if (response.isSuccess) {
      alert.success('احساست برای امروز ثبت شدش!')
      feelingPayload.value.type = previousFeeling
      isRenderingVoiceRecordModal.value = false
    } else {
      alerts.error(response.data)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    spinner.hideSpinner()
  }
}

</script>

<template>
  <div>
    <p class="text-center mt-2">حس امروزت رو انتخاب کن</p>
    <DashboardFeelings @click="createDailyFeeling" v-model="feelingPayload.type"></DashboardFeelings>
    <!--    <div class="w-full px-5">-->
    <!--      <UtilsInputsBaseInput-->
    <!--          v-model="feelingPayload.description"-->
    <!--          name="chatMessage"-->
    <!--          placeholder="در مورد احساست بنویس یا حرف بزن"-->
    <!--          label-class="!bg-[#ffff] rounded-lg  px-1 pl-2 border border-[#E0E4E8] mt-5 "-->
    <!--          input-class=" placeholder:text-[13px] "-->
    <!--          @keydown.enter="createDailyFeeling"-->
    <!--      >-->
    <!--        <template #prepend>-->
    <!--          <Icon-->
    <!--              :name="`feelings:${feelingsIconMap[feelingPayload.type]}`"-->
    <!--              size="35"-->
    <!--              class="w-10 h-10"-->
    <!--              color="#4C4C4C"-->
    <!--          />-->
    <!--        </template>-->
    <!--        <template #icon>-->
    <!--          <button type="button" @click="isRenderingVoiceRecordModal = true">-->
    <!--            <Icon name="icon:microphone" color="#01CED1"/>-->
    <!--          </button>-->
    <!--        </template>-->
    <!--      </UtilsInputsBaseInput>-->
    <!--    </div>-->
    <LazyUtilsDialogsVoiceRecordDialog @setVoiceMessage="getVoiceMessage"
                                       v-model="isRenderingVoiceRecordModal"></LazyUtilsDialogsVoiceRecordDialog>

  </div>

</template>

<style scoped>

</style>