<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IQuestionsListFilters} from "@/services/QuestionService";

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
  dropdownLabel: 'انتخاب سوال',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
  getAllQuestions()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const questionsList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const model = defineModel()
const questionsListFilters = ref<IQuestionsListFilters>({
  pageNumber: 1,
  count: 100,
  search: '',
})


watch(() => questionsListFilters.value.search, async () => {
  isLoading.value = true
  await getAllQuestions()
  isLoading.value = false
})

// Functions
async function getAllQuestions() {
  try {
    const response = await $api?.question.getAllQuestions(questionsListFilters.value)
    questionsList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
  }
}

function debouncedRequest(value: string | null) {
  questionsListFilters.value.search = value ? value.trim() : null
  getAllQuestions()
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
    :items="questionsList"
    :item-title="(item: any) => item.displayText"
    :item-value="(item: any) => item.id"
    :search-callback="handleSearch"
  />
</template>
