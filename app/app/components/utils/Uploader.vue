<template>
  <div>
    <input :id="props.id" :accept="props.accept" :multiple="props.multiple" class="hidden" type="file"
           @input="handleFile">
    <label
        v-if="!medias.length"
        :class="customHeight"
        :for="props.id"
        class="rounded-xl shadow bg-white border flex gap-3 cursor-pointer items-center justify-center p-3"
    >
      <Icon
          name="icon:upload"
      />
      <small class=" text-gray-400">
        {{ props.title }}
      </small>
    </label>
    <div class="w-full flex flex-col gap-2" v-else>
      <label :for="props.id" v-for="(media,idx) in medias"
             class="w-full flex cursor-pointer hover:bg-gray-100 relative  rounded-lg transition-all items-center justify-between">
        <NuxtImg :src="media"
                 :class="customHeight"
                 class="rounded-lg  object-contain"/>
        <div @click.prevent="removeMedia(media,idx)" class="cursor-pointer absolute top-2 right-2">
          <Icon
              class="w-3 h-3"
              name="icon:close"
              color="#8C8C8C"
          />
        </div>
      </label>
    </div>
  </div>
</template>

<script lang="ts" setup>

import type {UploaderTypes} from "~/models/Enums/UplaoderTypes";

const {$api} = useNuxtApp()
const medias = ref([])
const props = defineProps({
  defaultMedia: {
    type: String
  },
  id: {
    type: String as PropType<string>,
    default: 'mediaHandler'
  },
  title: {
    type: String as PropType<string>,
    default: 'آپلود عکس'
  },
  customHeight: {
    type: String as PropType<string>,
    default: 'h-full'
  },
  fileType: {
    type: Number as PropType<UploaderTypes>,
    required: true
  },
  accept: {
    type: String as PropType<string>,
    default: 'image/*'
  },
  multiple: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})

onMounted(() => {
  if (props.defaultMedia) {
    if (Array.isArray(props.defaultMedia)) {
      medias.value = []
      medias.value = props.defaultMedia
    } else {
      medias.value = [props.defaultMedia]
    }
  }
})

const emits = defineEmits<{
  (e: 'setMedia', medias: []): void
}>()

defineExpose({
  exposeMedia
})

function exposeMedia() {
  emits("setMedia", medias.value)
}

async function removeMedia(media, idx) {
  try {
    useSpinner().renderSpinner()
    console.log(media)
    const response = await $api.uploader.delete(media.split('Get/')[1])
    if (response.isSuccess) {
      medias.value.splice(idx, 1)
    } else {
      useToast.error(response.message)
    }
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function uploadFiles(file) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.uploader.upload({
      file,
      fileType: props.fileType
    })
    if (props.multiple) {
      medias.value.push(response.data.url)
    } else {
      medias.value = [response.data.url]
    }
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function handleFile(event) {
  if (event.target.files.length) {
    for (let i = 0; i < event.target.files.length; i++) {
      await uploadFiles(event.target.files[i])
    }
  }
}
</script>

<style scoped>

</style>
