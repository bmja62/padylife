<script setup lang="ts">
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {IStepsListFilters} from "@/services/StepsService";
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
  dropdownLabel: 'انتخاب مرحله',
  dropdownColor: 'success',
  multiple: false,
  required: false,
  returnObject: false
})


// LifeCycles
onMounted(() => {
  getSteps()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const stepsList = ref<null | IUser[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isLoading = ref<boolean>(false)
const model = defineModel()
const authStore = useAuthStore()
const stepsListFilters = ref<IStepsListFilters>({
  pageNumber: 1,
  count: 100,
  search: '',
  allUsers:authStore.getUser.roles.filter(e=> e.name === 'Admin').length ? true : false

})


watch(() => stepsListFilters.value.search, async () => {
  isLoading.value = true
  await getSteps()
  isLoading.value = false
})

// Functions
async function getSteps() {
  try {
    spinner.showSpinner()

    const response = await $api?.steps.getAllStepsByFilter(stepsListFilters.value)

    stepsList.value = response?.data.data.data as Array<IUser>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function debouncedRequest(value: string | null) {
  stepsListFilters.value.name = value ? value.trim() : null
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
    :return-object="props.returnObject"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :required="props.required"
    :is-loading="isLoading"
    :items="stepsList"
    :item-title="(item: any) => item.name"
    :item-value="(item: any) => item.id"
    :search-callback="handleSearch"
  />
</template>
