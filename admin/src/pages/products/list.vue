<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute} from "@/services/ProductAttributes";
import {IProduct, IProductGetParams} from "@/services/ProductService";

// LifeCycles
onMounted(() => {
  getAllProducts()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingDeleteDialog = ref<boolean>(false)
const productsList = ref<IAttribute[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedProduct = ref<IProduct>(null)
const productsFilters = ref<IProductGetParams>({
  pageNumber: 1,
  count: 10,
  searchTerm: '',
  categoryId: null
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام محصول', key: 'name'},
  {title: 'نام دسته بندی', key: 'categoryName'},
  {title: 'نوع محصول', key: 'type'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderDeleteDialog(item: IAttribute) {
  selectedProduct.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  productsFilters.value.pageNumber = +pageNumber
  await getAllProducts()
}

async function getAllProducts() {
  try {
    spinner.showSpinner()
    const response = await $api?.product.getAllProduct(productsFilters.value)
    productsList.value = response?.data.data.data as Array<IAttribute>
    totalCount.value = response?.data.data.totalCount as number

  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function submitFilters() {
  productsFilters.value.pageNumber = 1
  getAllProducts()
}
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="submitFilters"
  >
    <template #title>
      لیست محصولات
    </template>
    <template #append>
      <VBtn
        color="success"
        to="/products/create"
      >
        ایجاد محصول جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="productsFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getAllProducts"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <ProductCategoryPicker :required="false" v-model="productsFilters.categoryId"></ProductCategoryPicker>
      </VCol>
    </template>
    <CustomTable
      :items-list="productsList"
      :count="productsFilters.count"
      :page-number="productsFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          :to="`/products/${data.item.id}`"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as IAttribute)"
        />
      </template>
      <template #type="data">
        <VChip color="primary">{{data.item.type==='Simple' ? 'ساده':'پیشرفته'}}</VChip>
      </template>
    </CustomTable>


    <DeleteProductDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedProduct"
      @refetch="getAllProducts"
    />

  </PageWrapper>
</template>
