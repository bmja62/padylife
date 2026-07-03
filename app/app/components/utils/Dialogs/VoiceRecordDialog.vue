<script setup lang="ts">
import {UploaderTypes} from "~/models/Enums/UplaoderTypes";

const emits = defineEmits<{
  (e: 'setVoiceMessage', voiceMessage: string): void
}>()
const isRendering = defineModel()
const isRecording = ref(false)
const recorder = useTemplateRef('recorder')
const audioStopWatch = ref(null)
const hours = ref(0)
const minutes = ref(0)
const {$api} = useNuxtApp()
const seconds = ref(0)
const alert = useAlerts()
const spinner = useSpinner()
const voiceMessage = ref(null)
const generateAudioSrc = computed(() => {
  if (voiceMessage.value) {
    return URL.createObjectURL(voiceMessage.value)
  }
})

function setAudioBlob(blob) {
  voiceMessage.value = blob
}

function startRecording() {
  recorder.value.startRecording()
  if (recorder.value.isPlaying) {
  isRecording.value = true
  if (isRecording.value) {
    audioStopWatch.value = setInterval(() => {
      seconds.value++
      if (seconds.value === 59) {
        seconds.value = 0
        minutes.value++
        if (minutes.value === 59) {
          hours.value++
        }
      }
    }, 1000)
  }
  }

}

function stopRecording() {
  clearInterval(audioStopWatch.value)
  isRecording.value = false
  seconds.value = 0
  hours.value = 0
  minutes.value = 0
  voiceMessage.value = null
  recorder.value.stopRecording()
}

async function uploadVoice() {
  try {
    spinner.renderSpinner()
    const response = await $api.uploader.upload({
      file: voiceMessage.value,
      fileType: UploaderTypes.DailyFeelingVoice
    })
    if (response.isSuccess) {
      emits('setVoiceMessage', response.data.url)
    } else {
      alert.error(response.data)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog v-model="isRendering" dialog-id="previewImage">
    <template #title>
      <span>ضبط صدا</span>
    </template>
    <template #default>
      <span>در مورد احساست یک ویس کوتاه بگیر و ثبت کن</span>
      <LazyUtilsAudioRecorder ref="recorder" @get-audio-blob="setAudioBlob"></LazyUtilsAudioRecorder>
      <div class="w-full h-[10rem] flex flex-col  items-center justify-center rounded-2xl ">
        <Icon v-if="!isRecording" size="40" name="icon:custom-play-new" @click.stop="startRecording"/>
        <Icon v-else size="40" name="icon:custom-pause" @click="stopRecording"/>
        <div class="flex items-center flex-row-reverse gap-3 my-2">
          <small>{{ hours }}</small>
          :
          <small>{{ minutes }}</small>
          :
          <small>{{ seconds }}</small>
        </div>
        <audio v-if="voiceMessage" :src="generateAudioSrc" controls></audio>
        <button v-if="voiceMessage" @click="uploadVoice" type="button"
                class="btn w-full mt-3 bg-primary text-white border-none">
          ارسال
        </button>
      </div>

    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>