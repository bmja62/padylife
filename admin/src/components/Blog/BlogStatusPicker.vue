<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'


interface IProps {
  required: boolean
  defaultTagName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  returnObject?: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'وضعیت انتشار',
  dropdownColor: 'success',
  required: false,
  multiple: false,
  returnObject: false,
})

const options = ref([
  {
    name: 'پیش نویس',
    value: 'PreRelease'
  },
  {
    name: 'منتشر شده',
    value: 'Publish'
  },

])

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isLoading = ref<boolean>(false)
const model = defineModel()

// Functions

function debouncedRequest(value: string | null) {

}

function handleSearch(value: string) {
  debouncedRequest(value)
}
</script>

<template>
  <CustomPicker
    v-model="model"
    :chips="false"
    :multiple="props.multiple"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :is-loading="isLoading"
    :items="options"
    :return-object="props.returnObject"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :required="props.required"
    :item-title="(item:any) => item.name"
    :item-value="(item:any) => item.value"
    :search-callback="handleSearch"
  />
</template>
