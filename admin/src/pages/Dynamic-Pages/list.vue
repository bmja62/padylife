<script lang="ts" setup>
import {isAxiosError} from 'axios'
import {inject, onMounted, ref} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IDynamicPage, IGetAllDynamicPagesFilters} from '@/services/DynamicPageService'
import {useAlerts} from '@/composables/alert'
import DynamicPageDeleteConfirmationDialog from '@/components/Dynamic-Pages/DynamicPageDeleteConfirmationDialog.vue'

// LifeCycles
onMounted(() => {
  getDynamicPageList()
})

// Variables
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const spinner = useSpinner()
const isRenderingDeleteDialog = ref<boolean>(false)
const dynamicPagesList = ref<null | IDynamicPage[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedDynamicPage = ref<IDynamicPage | null>(null)

const tableHeaders: ITableHeaders = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان', key: 'title'},
  {title: 'عملیات', key: 'actions'},

]

const dynamicPagesFilters = ref<IGetAllDynamicPagesFilters>({
  pageNumber: 1,
  count: 10,
  seoTitle: ''
})

// Functions
async function getDynamicPageList() {
  try {
    spinner.showSpinner()

    const response = await $api?.dynamicPages.getAllDynamicPages(dynamicPagesFilters.value)

    dynamicPagesList.value = response?.data.data as Array<IDynamicPage>
    totalCount.value = response?.data.data.total as number
  } catch (error) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function renderDeleteConfirmationModal(selectedPage: IDynamicPage) {
  selectedDynamicPage.value = JSON.parse(JSON.stringify(selectedPage))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  dynamicPagesFilters.value.pageNumber = +pageNumber
  await getDynamicPageList()
}
</script>

<template>
  <PageWrapper has-filters @submit-filters="getDynamicPageList">

    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="dynamicPagesFilters.seoTitle"
          label="نام برگه"
          hide-details
          @keydown.enter="getDynamicPageList"
        />
      </VCol>
    </template>
    <template #title>
      لیست برگه های وبسایت
    </template>
    <CustomTable
      :count="dynamicPagesFilters.count"
      :items-list="dynamicPagesList"
      :page-number="dynamicPagesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          :to="`/dynamic-pages/${data.item.id}`"
          color="transparent"
          elevation="0"
          icon
        >
          <VIcon
            size="20"
            color="blue-accent-4"
            icon="mdi-open-in-new"
          />
        </VBtn>
        <VBtn
          color="transparent"
          elevation="0"
          icon
          @click="renderDeleteConfirmationModal(data.item)"
        >
          <VIcon
            size="20"
            color="error"
            icon="mdi-delete"
          />
        </VBtn>
      </template>
    </CustomTable>
    <DynamicPageDeleteConfirmationDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-dynamic-page="selectedDynamicPage"
      @refetch="getDynamicPageList"
    />
  </PageWrapper>
</template>
