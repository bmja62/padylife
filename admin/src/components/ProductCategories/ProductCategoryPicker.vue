<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IProperty} from '@/services/ProductPropertyService'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {IProductCategory} from "@/services/ProductCategories";

// Interfaces
interface IProps {
  defaultPropertyIds?: IProperty[] | null | undefined
  defaultPropertyName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  chips?: boolean
  returnObject?: boolean
  required?: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب دسته بندی',
  dropdownColor: 'success',
  multiple: false,
  chips: false,
  returnObject: false,
  required: false,
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const options = ref<null | []>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const productCategoriesFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})


// Functions
async function getOptions() {
  try {
    const response = await $api?.productCategories.getAllProductCategories(productCategoriesFilters.value)
    options.value = response?.data.data.data as []
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  productCategoriesFilters.value.search = value
  getOptions()
}

function handleSearch(value: string) {
  debouncedRequest(value)
}

const propertyModel = defineModel()

onMounted(() => {
  getOptions()
})

</script>

<template>
  <CustomPicker
    v-model="propertyModel"
    :multiple="props.multiple"
    :chips="props.chips"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :is-loading="isLoading"
    :items="options"
    :rules="props.required ? [(value) => !!value || 'این فیلد اجباری است']:''"
    :required="props.required"
    :returnObject="returnObject"
    :item-title="(item: IProductCategory) => item.name"
    :item-value="(item: IProductCategory) => item.id"
    :search-callback="handleSearch"
  />
</template>
