<script setup lang="ts">
import {useAuthStore} from '@/stores/auth'

// Interfaces
interface IProps {
  defaultEditorContent?: string | null
  editorPlaceholder?: string
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  editorPlaceholder: 'شروع به نوشتن کنید',
})

// Variables
const baseUrl = import.meta.env.VITE_BASE_API_URL
const auth = useAuthStore()

const froalaConfig = ref<Record<string, unknown>>({
  direction: 'rtl',
  placeholderText: props.editorPlaceholder,
  imageUpload: true,
  imageUploadURL: `${baseUrl}/api/v1/Uploaders/UploadCkEditor/UploadCkEditor`,
  imageUploadParam: 'File',
  imageUploadParams: {
    WithWaterMark: false
  },
  attribution: false,
  fontFamily: {
    Peyda: "Peyda",
  },
  videoUpload: true,
  videoUploadURL: `${baseUrl}/api/v1/Uploaders/UploadCkEditor/UploadCkEditor`,

  videoUploadParam: "File",
  requestHeaders: {
    Authorization: `Bearer ${auth.getToken}`,
  },
})

// Models
const model = defineModel<string>()
</script>

<template>
  <div class="w-100">
    <Froala
      id="edit"
      v-model:value="model"
      :config="froalaConfig"
    />
  </div>
</template>

<style>

</style>
