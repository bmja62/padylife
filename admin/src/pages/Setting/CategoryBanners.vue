<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload, IgetMainSlidersFilters} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import DeleteMainSliderDialog from "@/components/MainSlider/DeleteMainSliderDialog.vue";
import CreateMainSliderDialog from "@/components/MainSlider/CreateMainSliderDialog.vue";

// Interfaces
interface ItempSlider extends ICreateOrUpdateBrandPayload {
  id: string | number
}

// LifeCycles
onMounted(() => {
  getMainSliders()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const mainSliderList = ref<null | ItempSlider[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempSlider = ref<ItempSlider>()

const mainSliderFilters = ref<IgetMainSlidersFilters>({
  pageNumber: 1,
  count: 10,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'لینک', key: 'link'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: ICreateOrUpdateBrandPayload) {
  tempSlider.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: ICreateOrUpdateBrandPayload) {
  tempSlider.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  mainSliderFilters.value.pageNumber = +pageNumber
  await getMainSliders()
}

async function getMainSliders() {
  try {
    spinner.showSpinner()

    const response = await $api?.products.getAllMainSliders(mainSliderFilters.value)
    mainSliderList.value = response?.data.data.data as Array<ItempSlider>
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
      بنر های دسته بندی
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد بنر جدید
      </VBtn>
    </template>

    <CustomTable
      :items-list="mainSliderList"
      :count="mainSliderFilters.count"
      :page-number="mainSliderFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">

        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempSlider)"
        />
      </template>
    </CustomTable>


    <DeleteMainSliderDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-slider="tempSlider"
      @refetch="getMainSliders"
    />

    <CreateMainSliderDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getMainSliders"
    />
  </PageWrapper>
</template>
