<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IPlan, IPlansListFilters} from "@/services/PlanService";
import {useAuthStore} from "@/stores/auth";

interface IProps {
  required: boolean
  defaultTagName?: string | null | undefined
  dropdownLabel?: string
  dropdownColor?: string
  multiple?: boolean
  returnObject?: boolean
  roleFilter?: roleNames | null
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  dropdownLabel: 'انتخاب پلن',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
  getAllPlans()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const options = ref<null | IPlan[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const model = defineModel()
const authStore = useAuthStore()
const plansListFilters = ref<IPlansListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
  userId:authStore.getUser.id

})

// Functions
async function getAllPlans() {
  try {
    const response = await $api?.plan.getAllPlans(plansListFilters.value)
    options.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  plansListFilters.value.search = value ? value.trim() : null
  getAllPlans()
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
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :return-object="props.returnObject"
    :required="props.required"
    :is-loading="isLoading"
    :items="options"
    :item-title="(item: IPlan) => item.title"
    :item-value="(item: IPlan) => item.id"
    :search-callback="handleSearch"
  />
</template>
