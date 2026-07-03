<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {IAttribute} from "@/services/ProductAttributes";


onMounted(() => {
  getAttributes()
})
// Interfaces
interface IProps {
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  chips?: boolean
  returnObject?: boolean
  required?: boolean
  static?: boolean
}



// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'ویژگی محصول',
  dropdownColor: 'success',
  multiple: false,
  chips: false,
  returnObject: false,
  required: false,
  static: false,
})


// LifeCycles

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const propertyList = ref<null | IAttribute[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const filters = ref<IGlobalGridRequest>({
  count: 100,
  pageNumber: 1,
  search: ''
})
const staticProperties = ref([
  {
    name: "سایز",
    description: "سایز های XS , S , M ,L ,XL ,XXL",
    type: "Size",
    id: 247
  },
  {
    name: "رنگ",
    description: "تمامی رنگ ها",
    type: "Color",
    id: 248
  }
])

// Functions
async function getAttributes() {
  try {

    const response = await $api?.productAttributes.getAllProductAttributes(filters.value)
    propertyList.value = response?.data.data.data as Array<IAttribute>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  filters.value.search = value
  getAttributes()
}

function handleSearch(value: string) {
  debouncedRequest(value)
}

const propertyModel = defineModel()

</script>

<template>
  <CustomPicker
    v-model="propertyModel"
    :multiple="props.multiple"
    :chips="props.chips"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :is-loading="isLoading"
    :items="props.static ? staticProperties : propertyList"
    :item-title="(item: IAttribute) => item.name"
    :item-value="(item: IAttribute) => item.id"
    :search-callback="handleSearch"
  />
</template>
