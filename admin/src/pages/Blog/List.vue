<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import DeleteBlogDialog from "@/components/Blog/DeleteBlogDialog.vue";
import {IBlogListFilters} from "@/services/BlogService";

// Interfaces
interface ItempBlog extends ICreateOrUpdateBrandPayload {
  id: string | number
}

// LifeCycles
onMounted(() => {
  getAllBlogs()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const blogsList = ref<null | ItempBlog[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempBlog = ref<ItempBlog>()

const blogsListFilters = ref<IBlogListFilters>({
  pageNumber: 1,
  count: 10,
  searchByTitle: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام', key: 'title'},
  {title: 'تاریخ ثبت', key: 'createdAt', value:(item)=> new Date(item.createdAt).toLocaleDateString('fa-IR')},
  {title: 'وضعیت', key: 'status'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: ICreateOrUpdateBrandPayload) {
  tempBlog.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: ICreateOrUpdateBrandPayload) {
  tempBlog.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  blogsListFilters.value.pageNumber = +pageNumber
  await getAllBlogs()
}

async function getAllBlogs() {
  try {
    spinner.showSpinner()
    const response = await $api?.blog.getAllBlog(blogsListFilters.value)
    blogsList.value = response?.data.data.data as Array<ItempBlog>
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
    @submitFilters="getAllBlogs"
  >
    <template #title>
      لیست اخبار محیط زیست
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="blogsListFilters.searchByTitle"
          label="نام خبر"
          hide-details
          @keydown.enter="getAllBlogs"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="blogsList"
      :count="blogsListFilters.count"
      :page-number="blogsListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #status="data">
        <VChip
          v-if="data.item.status === 'Publish'"
          label
          color="success"
        >
          منتشر شده
        </VChip>
        <VChip
          v-else
          label
          color="secondary"
        >
          پیش نویس
        </VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Blog/Edit/${data.item.id}`"
          icon="mdi-pencil"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempBlog)"
        />
      </template>
    </CustomTable>
    <DeleteBlogDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-blog="tempBlog"
      @refetch="getAllBlogs"
    />
  </PageWrapper>
</template>
