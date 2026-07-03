<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IQuestionsListFilters} from "@/services/QuestionService";
import DeleteQuestionDialog from "@/components/Question/DeleteQuestionDialog.vue";
import {useAuthStore} from "@/stores/auth";


// LifeCycles
onMounted(() => {
  getAllQuestions()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const questionsList = ref<null | ItempQuestion[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempQuestion = ref<ItempQuestion>()
const authStore = useAuthStore()
const questionsListFilters = ref<IQuestionsListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
  userId:authStore.getUser.id

})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان تمرین', key: 'displayText'},
  {title: 'دسته بندی', key: 'questionCategoryName'},
  {title: 'عملیات', key: 'actions'},
]

function renderDeleteDialog(item: ICreateOrUpdateBrandPayload) {
  tempQuestion.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  questionsListFilters.value.pageNumber = +pageNumber
  await getAllQuestions()
}

async function getAllQuestions() {
  try {
    spinner.showSpinner()
    const response = await $api?.question.getAllQuestions(questionsListFilters.value)
    questionsList.value = response?.data.data.data as Array<ItempQuestion>
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
    @submitFilters="getAllQuestions"
  >
    <template #title>
      لیست سوالات
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="questionsListFilters.search"
          label="عنوان سوال"
          hide-details
          @keydown.enter="getAllQuestions"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="questionsList"
      :count="questionsListFilters.count"
      :page-number="questionsListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Questions/${data.item.id}`"
          icon="mdi-pencil"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempQuestion)"
        />
      </template>
    </CustomTable>
    <DeleteQuestionDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-question="tempQuestion"
      @refetch="getAllQuestions"
    />


  </PageWrapper>
</template>
