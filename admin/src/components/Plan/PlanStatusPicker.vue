<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {PlanStatus, planStatusesForPicker} from "@/services/PlanService";

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
  dropdownLabel: 'انتخاب وضعیت',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isLoading = ref<boolean>(false)
const route = useRoute()
const selectedValue = defineModel()


function debouncedRequest(value: string | null) {
  // questionsListFilters.value.search = value ? value.trim() : null
  // getAllQuestions()
}

function handleSearch(value: string) {
  debouncedRequest(value)
}
</script>

<template>
  <CustomPicker
    v-model="selectedValue"
    :chips="false"
    :multiple="props.multiple"
    :color="props.dropdownColor"
    :label="props.dropdownLabel"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :return-object="props.returnObject"
    :required="props.required"
    :is-loading="isLoading"
    :items="planStatusesForPicker"
    :item-title="(item: any) => item.name"
    :item-value="(item: any) => item.value"
    :search-callback="handleSearch"
  />
</template>
