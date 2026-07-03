<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IBlogCategory} from "@/services/BlogService";
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
// LifeCycles
onMounted(() => {
  getAllBlogCategories()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const blogCategoryList = ref<null | IBlogCategory[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempCategory = ref<IBlogCategory>()

const blogCategoryFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام', key: 'title'},
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

function renderDeleteDialog(item: IBlogCategory) {
  tempCategory.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  blogCategoryFilters.value.pageNumber = +pageNumber
  await getAllBlogCategories()
}

async function getAllBlogCategories() {
  try {
    spinner.showSpinner()

    const response = await $api?.blog.getAllBlogCategories(blogCategoryFilters.value)
    blogCategoryList.value = response?.data.data.data as Array<IBlogCategory>
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
      دسته بندی های اخبار محیط زیست
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
      :items-list="blogCategoryList"
      :count="blogCategoryFilters.count"
      :page-number="blogCategoryFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item)"
        />
      </template>
    </CustomTable>

    <UpdateBlogCategoryDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :default-category="tempCategory"
      @refetch="getAllBlogCategories"
    />

    <DeleteBlogCategoryDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-category="tempCategory"
      @refetch="getAllBlogCategories"
    />

    <CreateBlogCategoryDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getAllBlogCategories"
    />
  </PageWrapper>
</template>
