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
  dropdownLabel: 'انتخاب وضعیت',
  dropdownColor: 'success',
  required: false,
  multiple: false,
  returnObject: false,
})

const entityTypes = ref([
  {
    name: 'فعال',
    value: true
  },
  {
    name: 'غیرفعال',
    value: false
  }
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
    :items="entityTypes"
    :clearable="false"
    :return-object="props.returnObject"
    :required="props.required"
    :item-title="(item:any) => item.name"
    :item-value="(item:any) => item.value"
    :search-callback="handleSearch"
  />
</template>
