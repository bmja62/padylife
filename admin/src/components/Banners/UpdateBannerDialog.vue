<script lang="ts" setup>
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

// Interfaces
interface IProps {
  dialogState: boolean
  defaultBanner: any
}

interface IEmit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  defaultBanner: () => {
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
      props.defaultBanner.fileData = window.btoa(binaryData)
    }
  })(f)
  reader.readAsBinaryString(f)
}

async function updateBanner() {
  if (props.defaultBanner) {
    try {
      spinner.showSpinner()
      if (props.defaultBanner.fileData.includes('.webp')) {
        props.defaultBanner.fileData = ''
      }
      const response = await $api?.users.updateBanner(props.defaultBanner)
      alert.success('بنر موفقیت ویرایش شد')
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
    :title="`ویرایش بنر`"
    @update="updateBanner"
    @update:dialog-state="updateDialogState"
  >
    <VCol cols="12">
      <VTextField
        v-model.trim="defaultBanner.link"
        color="success"
        label="لینک بنر"
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
