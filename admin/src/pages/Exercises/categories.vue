<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IExerciseCategoryListFilters} from "@/services/ExerciseService";


// LifeCycles
onMounted(() => {
  getAllExerciseCategories()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const exerciseCategoryList = ref<null | ItempCategory[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempCategory = ref<ItempCategory>()

const exerciseCategoryFilters = ref<IExerciseCategoryListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام', key: 'name'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: ICreateOrUpdateBrandPayload) {
  tempCategory.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: ICreateOrUpdateBrandPayload) {
  tempCategory.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  exerciseCategoryFilters.value.pageNumber = +pageNumber
  await getAllExerciseCategories()
}

async function getAllExerciseCategories() {
  try {
    spinner.showSpinner()

    const response = await $api?.exercise.getAllExerciseCategories(exerciseCategoryFilters.value)
    exerciseCategoryList.value = response?.data.data.data as Array<ItempCategory>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
  >
    <template #title>
      دسته بندی های تمرینات
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد دسته بندی جدید
      </VBtn>
    </template>

    <CustomTable
      :items-list="exerciseCategoryList"
      :count="exerciseCategoryFilters.count"
      :page-number="exerciseCategoryFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item as ItempCategory)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempCategory)"
        />
      </template>
    </CustomTable>

    <UpdateExerciseCategoryDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :default-category="tempCategory"
      @refetch="getAllExerciseCategories"
    />

    <DeleteExerciseCategoryDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-category="tempCategory"
      @refetch="getAllExerciseCategories"
    />

    <CreateExerciseCategoryDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getAllExerciseCategories"
    />
  </PageWrapper>
</template>
