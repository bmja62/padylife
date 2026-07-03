<script lang="ts" setup>
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

// Interfaces
interface IProps {
  dialogState: boolean
  defaultSlide: any
}

interface IEmit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  defaultSlide: () => {
    return {}
  },
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const imagePreview = ref('')
const fileInputRef = ref(null)
const slideDTO = ref({
  base64: "",
  oldFileName: "",
  link: ''
})
const baseUrl = computed(() => {
  return import.meta.env.VITE_BASE_API_URL
})
watch(() => props.defaultSlide, async (val) => {
  if (val) {
    imagePreview.value = baseUrl.value + '/media/Gallery/slider/' + props.defaultSlide.pictureUrl
  }
}, {immediate: true})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

function setFilePreview() {
  const f = fileInputRef.value.files[0]
  imagePreview.value = URL.createObjectURL(f)
  const reader = new FileReader()
  reader.onload = (function (theFile) {
    return function () {
      const binaryData = reader.result
      slideDTO.value.base64 = window.btoa(binaryData)
      slideDTO.value.oldFileName = props.defaultSlide.pictureUrl
    }
  })(f)
  reader.readAsBinaryString(f)
}

async function updateSlide() {
  if (props.defaultSlide) {
    slideDTO.value.link = props.defaultSlide.link
    slideDTO.value.oldFileName = props.defaultSlide.pictureUrl
    try {
      spinner.showSpinner()
      const response = await $api?.users.updateSlides(slideDTO.value)
      alert.success('اسلاید با موفقیت ویرایش شد')
      emit('update:dialogState', false)
      emit('refetch')

    } catch (error: unknown) {
      if (isAxiosError(error))

        alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

      else
        console.error(error)
    } finally {
      spinner.hideSpinner()
    }
  }
}
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`ویرایش اسلاید`"
    @update="updateSlide"

    @update:dialog-state="updateDialogState"
  >
    <VCol cols="12">
      <VTextField
        v-model.trim="defaultSlide.link"
        color="success"
        label="لینک اسلاید"
      />
    </VCol>
    <VCol md="12">
      <VFileInput
        ref="fileInputRef"
        :rules="[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']"
        accept="image/png,image/jpg,image/webp,image/jpeg"
        label="تصویر بنر"
        @input="setFilePreview($event)"
        @click:clear="imagePreview = ''"
      ></VFileInput>

      <VImg :src="imagePreview" height="10rem" width="10rem"></VImg>
    </VCol>
  </CustomUpdateDialog>
</template>
