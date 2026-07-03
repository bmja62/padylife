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
  dropdownLabel: 'انتخاب نوع گام',
  dropdownColor: 'success',
  required: false,
  multiple: false,
  returnObject: false,
})

const stepOptionTypes = ref([
  {
    name: 'چند گزینه ای',
    value: 'CreateMultiple'
  },
  {
    name: 'ویدیو',
    value: 'CreateVideo'
  },
  {
    name: 'تکلیف',
    value: 'CreateTask'
  },
  {
    name: 'تعاملی',
    value: 'CreateAction'
  },
  {
    name: 'متن‌دار',
    value: 'CreateText'
  },
  {
    name: 'عکس',
    value: 'CreateImage'
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
    :items="stepOptionTypes"
    :return-object="props.returnObject"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :required="props.required"
    :item-title="(item:any) => item.name"
    :item-value="(item:any) => item.value"
    :search-callback="handleSearch"
  />
</template>
