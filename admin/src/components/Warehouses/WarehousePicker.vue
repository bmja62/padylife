<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IWarehouse} from "@/services/WarehouseService";

// Interfaces
interface IProps {
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  chips?: boolean
  returnObject?: boolean
  required?: boolean
}

interface IEmits {
  (e: 'getWarehouses', value: IProperty[]): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب انبار',
  dropdownColor: 'success',
  multiple: false,
  chips: false,
  returnObject: false,
  required: false,
})

const emits = defineEmits<IEmits>()

// LifeCycles

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const warehousesList = ref<null | IWarehouse[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)


// Functions
async function getWarehouses() {
  try {
    spinner.showSpinner()

    const response = await $api?.warehouse.getWarehouses({
      count: 100,
      pageNumber: 1,
    })

    warehousesList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
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
  getWarehouses()
})
</script>

<template>
  <CustomPicker
    v-model="propertyModel"
    :multiple="props.multiple"
    :chips="props.chips"
    :color="props.dropdownColor"
    :returnObject="props.returnObject"
    :label="props.dropdownLabel"
    :is-loading="isLoading"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :items="warehousesList"
    :item-title="(item: IWarehouse) => item.name"
    :item-value="(item: IWarehouse) => item.id"
    :search-callback="handleSearch"
  />
</template>
