<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IEntity} from "@/services/CountriesService";

// Interfaces
interface IProps {
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  chips?: boolean
  returnObject?: boolean
  required?: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب کشور',
  dropdownColor: 'success',
  multiple: false,
  chips: false,
  returnObject: false,
  required: false,
})


// LifeCycles

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const options = ref<null | IEntity[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)


// Functions
async function getEntities() {
  try {
    spinner.showSpinner()

    const response = await $api?.countries.getAll({
      pageNumber: 1,
      count: 100,
    })
    options.value = response?.data.data.data as Array<IProperty>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function debouncedRequest(value: string | null) {

}

function handleSearch(value: string) {
  debouncedRequest(value)
}

const propertyModel = defineModel()

onMounted(() => {
  getEntities()
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
    :return-object="props.returnObject"
    :item-title="(item: IEntity) => item.countryNameFa"
    :item-value="(item: IEntity) => item.id"
    :search-callback="handleSearch"
  />
</template>
