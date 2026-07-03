<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {IProductCategory} from "@/services/ProductCategories";

// LifeCycles
onMounted(() => {
  getProductCategories()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const productCategoriesList = ref<IProductCategory[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedProductCategory = ref<IProductCategory>()

const productCategoriesFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام دسته بندی', key: 'name'},
  {title: 'نام دسته بندی مادر', key: 'parentName'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IProductCategory) {
  selectedProductCategory.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: IProductCategory) {
  selectedProductCategory.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  productCategoriesFilters.value.pageNumber = +pageNumber
  await getProductCategories()
}

async function getProductCategories() {
  try {
    spinner.showSpinner()
    const response = await $api?.productCategories.getAllProductCategories(productCategoriesFilters.value)
    productCategoriesList.value = response?.data.data.data as Array<IProductCategory>
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
    @submit-filters="getProductCategories"
  >
    <template #title>
      دسته بندی محصولات
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد دسته بندی جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="productCategoriesFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getProductCategories"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="productCategoriesList"
      :count="productCategoriesFilters.count"
      :page-number="productCategoriesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item as IProductCategory)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as IProductCategory)"
        />
      </template>
    </CustomTable>

    <UpdateProductCategoryDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selected-item="selectedProductCategory"
      @refetch="getProductCategories"
    />

    <DeleteProductCategoryDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedProductCategory"
      @refetch="getProductCategories"
    />

    <CreateProductCategoryDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getProductCategories"
    />
  </PageWrapper>
</template>
