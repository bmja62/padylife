<template>
  <VFileInput
    :label="props.label"
    accept="image/*"
    :id="props.inputId"
    :multiple="props.multiple"
    :required="props.required"
    :rules="props.required?[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']:''"
    @input="setFilePreview($event)"
  ></VFileInput>
  <div v-if="files.length>0" class="d-flex mt-2 flex-wrap gap-2">
    <div v-for="(item,idx) in files" class="position-relative" style="width: 10rem;height: 10rem">
      <template v-if="item.pictureUrl">
        <div style="cursor: pointer;z-index: 999!important" @click="deleteMedia(item,idx)">
          <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>
        </div>
        <VImg :src="`${item.pictureUrl}`" class="w-100 h-100"
              style="object-fit: cover"></VImg>
      </template>
      <template v-else>
        <div style="cursor: pointer;z-index: 999!important" @click="deleteMediaWithMediaId(item)">
          <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>
        </div>
        <VImg :src="`${baseUrl}/${item.base64}`" class="w-100 h-100"
              style="object-fit: cover"></VImg>
      </template>
    </div>

  </div>
</template>

<script lang="ts" setup>
import {PropType} from "vue";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {useAuthStore} from "@/stores/auth";
import {useUtils} from "@/composables/useUtils";

const baseUrl = computed(() => {
  return import.meta.env.VITE_BASE_API_URL
})
const files = ref([])
const authStore = useAuthStore()
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const spinner = useSpinner()
defineExpose({
  getFiles
})
const props = defineProps({
  multiple: {
    type: Boolean as PropType<boolean>,
    default: false,
    required: false
  },
  inputId: {
    type: String as PropType<string>,
    required: true,
    default: 'fileInputRef'
  },
  label: {
    type: String as PropType<string>,
    required: false
  },
  required: {
    type: Boolean as PropType<boolean>,
    required: false,
    default: false
  },
  live: {
    type: Boolean as PropType<boolean>,
    required: false,
    default: false
  },
  sizeLimit: {
    required: false,
    type: Number as PropType<number>,
    default: 0
  },
  fileType: {
    type: Number as PropType<number>,
    default: 1,
    required: false
  },


  defaultMedias: {}

})
watch(() => props.defaultMedias, async (val) => {
  if (val) {
    if (Array.isArray(val)) {
      files.value = val
    } else {
      files.value.push(val)
    }
  }
}, {immediate: true})
const emits = defineEmits<{
  (e: 'getFiles', fileArray: []): void
  (e: 'removeOriginalFile', idx: number): void
}>()

function getFiles() {
  emits('getFiles', files.value)
}

function setFilePreview() {
  const inputFiles = document.getElementById(props.inputId).files
  Array.prototype.forEach.call(inputFiles, (file) => {
    uploadMedia(file)
  })

}

async function uploadMedia(file: File) {
  let tempObj = {
    base64: '',
    pictureUrl: ''
  }
  if (props.sizeLimit) {
    if (file.size > props.sizeLimit) {
      alert.error(`اندازه عکس انتخاب شده نمیتواند بزرگتر از ${props.sizeLimit / 1000} مگابایت باشد`)
      return
    }
  }
  tempObj.pictureUrl = URL.createObjectURL(file)
  tempObj.base64 = await useUtils().makeBase64(file)
  if (!props.multiple) {
    files.value = []
  }
  files.value.push(tempObj)
  tempObj = {
    base64: '',
    pictureUrl: ''
  }
  if (props.live) {
    getFiles()
  }
}

async function deleteMedia(item: string, idx: number) {
  files.value.splice(idx, 1)
  if (props.live) {
    getFiles()
  }
}

async function deleteMediaWithMediaId(media, idx) {
  try {
    spinner.showSpinner()
    const response = await $api?.products.deleteMediaByMediaId(media.mediaId)
    if (response.data.isSuccess) {
      emits('removeOriginalFile', media.mediaId)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

</script>

<style scoped>

</style>
