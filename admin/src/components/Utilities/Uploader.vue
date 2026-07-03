<template>
  <VFileInput
    id="fileInput"
    :label="props.label"
    :multiple="props.multiple"
    :required="props.required"
    :accept="props.accept"
    :rules="props.required?[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']:''"
    @input="setFilePreview($event)"
  ></VFileInput>
  <div v-if="files.length>0 && props.withPreview" class="d-flex mt-2 flex-wrap gap-2">
    <div v-for="(item,idx) in files" class="position-relative" style="width: 10rem;height: 10rem">
<!--      <div style="cursor: pointer;z-index: 999!important" @click="deleteMedia(item,idx)">-->
<!--        <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>-->
<!--      </div>-->
      <video v-if="props.isVideo" :src="item" id="videoElement" class=" h-100" controls></video>
      <VImg  v-else :src="item" class="w-100 h-100"
            style="object-fit: cover"></VImg>
    </div>
  </div>
  <div v-if="!props.withPreview" class="d-flex mt-2 w-100  flex-column">
    <div v-for="(item,idx) in files" class="position-relative d-flex align-content-center justify-space-between" >
<!--      <div style="cursor: pointer;z-index: 999!important" @click="deleteMedia(item,idx)">-->
<!--        <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>-->
<!--      </div>-->
      <span>{{ item }}</span>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {PropType} from "vue";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {useAuthStore} from "@/stores/auth";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

const files = ref([])
const authStore = useAuthStore()
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const spinner = useSpinner()
defineExpose({
  getFiles
})
const props = defineProps({
  accept:{
    type: String as PropType<string>,
    default: '/*'
  },
  multiple: {
    type: Boolean as PropType<boolean>,
    default: false
  },
  label: {
    type: String as PropType<string>,
  },
  isVideo:{
    type: Boolean as PropType<boolean>,
    default: false
  },
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  },
  withPreview: {
    type: Boolean as PropType<boolean>,
    default: true
  },
  sizeLimit: {
    type: Number as PropType<number>,
    default: 0
  },
  fileType: {
    type: Number as PropType<UploaderTypes>,
    default: 1,
    required: true
  },

  defaultMedias: {}

})
onMounted(() => {
  if (props.defaultMedias) {
    if (Array.isArray(props.defaultMedias)) {
      props.defaultMedias.forEach((file) => {
        if (file.base64) {
          files.value.push(file.base64)
        }
      })
    } else {
      files.value.push(props.defaultMedias)
    }
  }
})

const emits = defineEmits<{
  (e: 'getFiles', fileArray: []): void
}>()

function getFiles() {
  emits('getFiles', files.value)
  files.value = []
}

function setFilePreview(event) {
  const inputFiles = event.target.files
  Array.prototype.forEach.call(inputFiles, (file) => {
    uploadMedia(file)
  })

}

async function uploadMedia(file: File) {
  if (props.sizeLimit) {
    if (file.size > props.sizeLimit) {
      alert.error(`اندازه عکس انتخاب شده نمیتواند بزرگتر از ${props.sizeLimit / 1000} مگابایت باشد`)
      return
    }
  }
  try {
    spinner.showSpinner()
    const myForm = new FormData()
    myForm.append('file', file)
    myForm.append('FileType', props.fileType)
    const response = await $api?.uploader.upload(myForm)
    if (response.data.isSuccess) {

      if (!props.multiple) {
        files.value = []
      }
      files.value.push(response.data.data.url)
    } else {
      alert.error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function deleteMedia(item: string, idx: number) {
  try {
    spinner.showSpinner()
    const response = await $api?.uploader.delete(item.split('/').filter(e=> e)[6])
    if (response.data.isSuccess) {
      alert.success('عکس با موفقیت حذف شد')
      files.value.splice(idx, 1)
    } else {
      alert.error(response.data.errorMessage)
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
