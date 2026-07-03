<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IExercisesListFilters} from "@/services/ExerciseService";

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
  dropdownLabel: 'انتخاب تمرین',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
  getAllExercises()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const exercisesList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const model = defineModel()
const exerciseListFilters = ref<IExercisesListFilters>({
  pageNumber: 1,
  count: 100,
  search: '',
})


watch(() => exerciseListFilters.value.search, async () => {
  isLoading.value = true
  await getAllExercises()
  isLoading.value = false
})

// Functions
async function getAllExercises() {
  try {
    const response = await $api?.exercise.getAllExercises(exerciseListFilters.value)
    exercisesList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  exerciseListFilters.value.search = value ? value.trim() : null
  getAllExercises()
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
    :items="exercisesList"
    :item-title="(item: any) => item.title"
    :item-value="(item: any) => item.id"
    :search-callback="handleSearch"
  />
</template>
