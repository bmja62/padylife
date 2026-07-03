<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IExerciseCategoryListFilters} from "@/services/ExerciseService";

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
  dropdownLabel: 'انتخاب دسته بندی',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
  getQuestionCategories()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const categoriesList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const model = defineModel()
const categoriesListFilters = ref<IExerciseCategoryListFilters>({
  pageNumber: 1,
  count: 100,
  search: '',
})


watch(() => categoriesListFilters.value.search, async () => {
  isLoading.value = true
  await getQuestionCategories()
  isLoading.value = false
})

// Functions
async function getQuestionCategories() {
  try {
    const response = await $api?.question.getAllQuestionCategories(categoriesListFilters.value)
    categoriesList.value = response?.data.data.data as Array<IUser>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  categoriesListFilters.value.name = value ? value.trim() : null
  getQuestionCategories()
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
    :items="categoriesList"
    :item-title="(item: any) => item.name"
    :item-value="(item: any) => item.id"
    :search-callback="handleSearch"
  />
</template>
