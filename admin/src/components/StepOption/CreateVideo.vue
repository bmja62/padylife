<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateVideoOption} from "@/services/StepOptionService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const emits = defineEmits<{
  (e: 'refetch')
}>()

const refVForm = ref(null)
const videoUrl = ref(null)
const spinner = useSpinner()
const thumbnailUrl = ref(null)
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const route = useRoute()

async function validateData() {
  videoUrl.value.getFiles()
  thumbnailUrl.value.getFiles()
  const vidElement = document.getElementById('videoElement')
  if (vidElement) {
    videoPayload.value.duration = `${Math.floor(vidElement.duration / 60 / 60)}:${Math.floor(vidElement.duration / 60)}:${Math.floor(vidElement.duration)}`
  }
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createVideoStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

function setVideoFile(medias) {
  if (medias.length) {
    videoPayload.value.videoUrl = medias[0]
  }
}

function setVideoThumbnail(medias) {
  if (medias.length) {
    videoPayload.value.thumbnailUrl = medias[0]
  }
}

const videoPayload = ref<ICreateVideoOption>({
  videoUrl: "",
  thumbnailUrl: "",
  duration: "",
  allowDownload: false,
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

async function createVideoStep() {
  try {
    spinner.showSpinner()
    const response = await $api?.stepOption.createVideo(videoPayload.value)
    if (response.data.isSuccess) {
      alert.success('گام با موفقیت ساخته شد')
      emits('refetch')
    } else {
      alert.error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <VForm
    ref="refVForm"
    @submit.prevent="validateData"
  >
    <VRow>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="videoPayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="videoPayload.description"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12">
        <Uploader is-video accept="video/mp4" required :default-medias="videoPayload.videoUrl" ref="videoUrl"
                  @getFiles="setVideoFile" label="فایل ویدیو"
                  :file-type="UploaderTypes.StepOption"></Uploader>
      </VCol>
      <VCol cols="12">
        <CustomSwitch v-model="videoPayload.allowDownload" :has-tooltip="false"
                      label="آیا فایل ویدیو قابل دانلود می‌باشد؟"></CustomSwitch>
      </VCol>
      <VCol cols="12">
        <Uploader accept="image/*" required :default-medias="videoPayload.thumbnailUrl" ref="thumbnailUrl"
                  @getFiles="setVideoThumbnail" label="عکس پوستر ویدیو"
                  :file-type="UploaderTypes.StepOption"></Uploader>
      </VCol>
      <VCol md="12" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="green"
        >
          ثبت
        </VBtn>
      </VCol>
    </VRow>
  </VForm>
</template>

<style scoped lang="scss">

</style>
