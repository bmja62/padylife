<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import StepOptionTypePicker from "@/components/StepOption/StepOptionTypePicker.vue";
import CreateVideo from "@/components/StepOption/CreateVideo.vue";
import CreateAction from "@/components/StepOption/CreateAction.vue";
import CreateImage from "@/components/StepOption/CreateImage.vue";
import CreateMultipleChoice from "@/components/StepOption/CreateMultipleChoice.vue";
import CreateTask from "@/components/StepOption/CreateTask.vue";
import CreateText from "@/components/StepOption/CreateText.vue";
import {optionsForShow} from "@/services/StepOptionService";
import DeleteStepOptionDialog from "@/components/StepOption/DeleteStepOptionDialog.vue";


// LifeCycles
onMounted(() => {
  getExerciseById()
  getStepOptions()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const route = useRoute()
const currentExercise = ref(null)
const currentSlug = ref('')
const slugs = shallowRef({
  'CreateVideo': CreateVideo,
  'CreateAction': CreateAction,
  'CreateImage': CreateImage,
  'CreateMultiple': CreateMultipleChoice,
  'CreateTask': CreateTask,
  'CreateText': CreateText
})
const totalCount = ref(null)
const stepOptionsListFilters = ref({
  stepId: route.params.id,
  type: null,
  search: null,
  pageNumber: 1,
  count: 10,
})
const stepOptions = ref([])
const selectedStepOption = ref(null)
const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان', key: 'title'},
  {title: 'نوع تمرین', key: 'type',value:(item)=> `${optionsForShow[item.type]}`},
  {title: 'عملیات', key: 'actions'},
]


async function getExerciseById() {
  try {
    spinner.showSpinner()
    const response = await $api?.exercise.getExerciseById(route.params.id)
    currentExercise.value = response.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function renderDeleteDialog(data) {
  isRenderingDeleteDialog.value = true
  selectedStepOption.value = JSON.parse(JSON.stringify(data))
}

const computedSlug = computed(() => {
  return slugs.value[currentSlug.value]
})

function refetch() {
  currentSlug.value = ''
  getStepOptions()
}

async function getStepOptions() {
  try {
    spinner.showSpinner()
    const response = await $api?.stepOption.getAllStepOptions(stepOptionsListFilters.value)
    stepOptions.value = response.data.data.data
    totalCount.value = response.data.data.totalCount
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function changePage(page) {
  stepOptionsListFilters.value.pageNumber = page
  getStepOptions()
}
// watch(()=>stepOptionsListFilters.value.type,async (val)=>{
//   stepOptionsListFilters.value.pageNumber=1
//   stepOptionsListFilters.value.type = optionsForPicker[stepOptionsListFilters.value.type]
//   getStepOptions()
// })
</script>

<template>
  <PageWrapper
  >
    <template #title>
      گام‌های مرحله
    </template>
    <VRow>
      <template v-if="!stepOptions.length">

        <VCol cols="12">
        <StepOptionTypePicker v-model="currentSlug"></StepOptionTypePicker>
      </VCol>
        <VDivider></VDivider>
      <VCol cols="12">
        <component :is="computedSlug" @refetch="refetch"></component>
      </VCol>
      </template>
      <VDivider></VDivider>
<!--      <VCol cols="12">-->
<!--        <VRow>-->
<!--          <VCol cols="12" md="3">-->
<!--            <StepOptionTypePicker v-model="stepOptionsListFilters.type"></StepOptionTypePicker>-->
<!--          </VCol>-->
<!--        </VRow>-->
<!--      </VCol>-->
      <VCol cols="12">
        <CustomTable
          :items-list="stepOptions"
          :count="stepOptionsListFilters.count"
          :page-number="stepOptionsListFilters.pageNumber"
          :table-headers="tableHeaders"
          :total-count="totalCount"
          @change-page="changePage"
        >
          <template #actions="data">

            <VBtn
              color="transparent"
              elevation="0"
              icon="mdi-delete"
              @click="renderDeleteDialog(data.item)"
            />
          </template>
        </CustomTable>

      </VCol>
    </VRow>
    <DeleteStepOptionDialog v-model:dialogState="isRenderingDeleteDialog"
                            :selectedStepOption="selectedStepOption"
                            @refetch="getStepOptions"
    ></DeleteStepOptionDialog>
  </PageWrapper>
</template>
