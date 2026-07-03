<script setup lang="ts">
import {inject} from 'vue'
import type {FilterFunction, FilterMode} from 'vuetify'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IProperty} from '@/services/ProductPropertyService'

// Interfaces
interface IProps {
  defaultPropertyIds?: IProperty[] | null | undefined
  defaultPropertyName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  returnObject?: boolean
  chips?: boolean
  customFilter?: FilterFunction
  filterMode: FilterMode
  isHidingSelected?: boolean
  categoryId: number | null
}

interface IEmits {
  (e: 'getProperties', value: IProperty[]): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب نقش',
  dropdownColor: 'success',
  multiple: false,
  returnObject: false,
  chips: false,
  customFilter: undefined,
  filterMode: 'intersection',
  isHidingSelected: false,
  categoryId: null
})
const emits = defineEmits<IEmits>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const options = ref([
  {
    name: 'کاربر عادی',
    value: 'User'
  },
  {
    name: 'ادمین',
    value: 'Admin'
  },
  {
    name: 'متخصص',
    value: 'Specialist'
  },


])
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)


// Functions

function debouncedRequest(value: string | null) {

}

function handleSearch(value: string) {
  debouncedRequest(value)
}

const model = defineModel()

</script>

<template>
  <CustomPicker
    v-model="model"
    :multiple="props.multiple"
    :chips="props.chips"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :filter-mode="props.filterMode"
    :custom-filter="props.customFilter"
    :is-loading="isLoading"
    :return-object="props.returnObject"
    :items="options"
    :item-title="(item) => item.name"
    :item-value="(item) => item.value"
    :search-callback="handleSearch"
  />
</template>
