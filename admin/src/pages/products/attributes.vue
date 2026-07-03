<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute, productAttributeTypesShow} from "@/services/ProductAttributes";
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";

// LifeCycles
onMounted(() => {
  getProductAttributes()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const attributesList = ref<IAttribute[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedAttribute = ref<IAttribute>()

const attributesFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام ویژگی', key: 'name'},
  {title: 'نوع ویژگی', key: 'type', value: (item:IAttribute) =>productAttributeTypesShow[item.type]},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IAttribute) {
  selectedAttribute.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: IAttribute) {
  selectedAttribute.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  attributesFilters.value.pageNumber = +pageNumber
  await getProductAttributes()
}

async function getProductAttributes() {
  try {
    spinner.showSpinner()
    const response = await $api?.productAttributes.getAllProductAttributes(attributesFilters.value)
    attributesList.value = response?.data.data.data as Array<IAttribute>
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
    @submit-filters="getProductAttributes"
  >
    <template #title>
      ویژگی های محصولات
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد ویژگی جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="attributesFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getProductAttributes"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="attributesList"
      :count="attributesFilters.count"
      :page-number="attributesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item as IAttribute)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as IAttribute)"
        />
      </template>
    </CustomTable>

    <UpdateAttributeDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selected-item="selectedAttribute"
      @refetch="getProductAttributes"
    />

    <DeleteAttributeDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedAttribute"
      @refetch="getProductAttributes"
    />

    <CreateAttributeDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getProductAttributes"
    />
  </PageWrapper>
</template>
