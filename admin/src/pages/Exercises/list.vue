<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IExercisesListFilters} from "@/services/ExerciseService";
import DeleteExerciseDialog from "@/components/Exercise/DeleteExerciseDialog.vue";
import {ExerciseTypes} from "../../models/Enums/ExerciseTypes";
import {useAuthStore} from "@/stores/auth";


// LifeCycles
onMounted(() => {
  getAllExercises()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const exerciseList = ref<null | ItempExercise[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempExercise = ref<ItempExercise>()
const authStore = useAuthStore()
const exerciseListFilters = ref<IExercisesListFilters>({
  pageNumber: 1,
  count: 10,
  searchByTitle: null,
  userId:authStore.getUser.id

})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان تمرین', key: 'title'},
  {title: 'دسته بندی', key: 'name'},
  {title: 'نوع تمرین', key: 'exerciseType'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: ICreateOrUpdateBrandPayload) {
  tempExercise.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: ICreateOrUpdateBrandPayload) {
  tempExercise.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  exerciseListFilters.value.pageNumber = +pageNumber
  await getAllExercises()
}

async function getAllExercises() {
  try {
    spinner.showSpinner()
    const response = await $api?.exercise.getAllExercises(exerciseListFilters.value)
    exerciseList.value = response?.data.data.data as Array<ItempExercise>
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
    has-filters
    @submitFilters="getAllExercises"
  >
    <template #title>
      لیست تمرینات
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="exerciseListFilters.search"
          label="عنوان تمرین"
          hide-details
          @keydown.enter="getAllExercises"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="exerciseList"
      :count="exerciseListFilters.count"
      :page-number="exerciseListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #exerciseType="data">
        <VChip color="primary">{{ ExerciseTypes[data.item.exerciseType] }}</VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Exercises/${data.item.id}`"
          icon="mdi-pencil"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempExercise)"
        />
      </template>
    </CustomTable>
    <DeleteExerciseDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-exercises="tempExercise"
      @refetch="getAllExercises"
    />


  </PageWrapper>
</template>
