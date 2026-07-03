<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {ICreateOrUpdatesliderPayload} from '@/services/BrandService'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {useUtils} from "@/composables/useUtils";

// Interfaces
interface Props {
  dialogState: boolean
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = defineProps<Props>()
const emit = defineEmits<Emit>()
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const imagePreview = ref('')
const sliderPayload = ref<ICreateOrUpdatesliderPayload>({
  imageFile: null,
  link: null,
})

const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

function setFilePreview(event) {
  sliderPayload.value.imageFile = event.target.files[0]
  imagePreview.value = URL.createObjectURL(event.target.files[0])
}

// Functions
async function createMainSlider() {
  let isValid = true
  Object.keys(sliderPayload.value).forEach((key) => {
    if (!sliderPayload.value[key]) {
      isValid = false
    }
  })
  if (isValid) {

    try {
      spinner.showSpinner()
      const myForm = useUtils().generateAnyFormData(sliderPayload.value)
      const response = await $api?.products.createMainSlider(myForm)
      if (response.data.isSuccess) {

        sliderPayload.value = {
          imageFile: null,
          link: null,
        }
        alert.success('بنر با موفقیت ایجاد شد!')
        emit('update:dialogState', false)
        emit('refetch')
      } else {
        alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
      }
    } catch (error: unknown) {
      if (isAxiosError(error))

        alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

      else
        console.error(error)
    } finally {
      spinner.hideSpinner()
    }
  } else {
    alert.error('لطفا همه ورودی ها را تکمیل کنید')
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد بنر جدید"
    @update:dialog-state="updateDialogState"
    @create="createMainSlider"
  >
    <VCol cols="12">
      <VTextField
        v-model.trim="sliderPayload.link"
        variant="outlined"
        density="compact"
        label="لینک"
        color="success"
        required
        hide-details
        @keydown.enter="createMainSlider"
      />
    </VCol>
    <VCol cols="12">
      <VFileInput
        ref="fileInputRef"
        :rules="[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']"
        accept="image/*"
        label="تصویر "
        @input="setFilePreview($event)"
        @click:clear="imagePreview = ''"
      ></VFileInput>

      <VImg :src="imagePreview" class="mt-2 rounded" height="10rem" width="100%"></VImg>
    </VCol>
  </CustomCreateDialog>
</template>
