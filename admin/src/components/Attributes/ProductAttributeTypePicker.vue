<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {productAttributeTypes} from "@/services/ProductAttributes";

// Interfaces
interface IProps {
  defaultPropertyIds?: IProperty[] | null | undefined
  defaultPropertyName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  chips?: boolean
  filterMode: any
  isHidingSelected?: boolean,
  returnObject?: boolean,
  required?: boolean,
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'نوع ویژگی',
  dropdownColor: 'success',
  multiple: false,
  chips: false,
  filterMode: 'intersection',
  isHidingSelected: false,
  returnObject: false,
  required: false,
})

const emits = defineEmits<{
  (e: 'getProperties', value: IProperty[]): void
}>()
// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const options = ref<null | []>(productAttributeTypes)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const propertyModel = defineModel()

// Functions

function debouncedRequest(value: string | null) {

}

function handleSearch(value: string) {
  debouncedRequest(value)
}


</script>

<template>
  <CustomPicker
    v-model="propertyModel"
    :multiple="props.multiple"
    :chips="props.chips"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :filter-mode="props.filterMode"
    :custom-filter="props.customFilter"
    :is-loading="isLoading"
    :items="options"
    :rules="[(value) => !!value || 'فیلد اجباری است']"
    :required="required"
    :returnObject="returnObject"
    :item-title="(item: IProperty) => item.label"
    :item-value="(item: IProperty) => item.value"
    :search-callback="handleSearch"
  />
</template>
