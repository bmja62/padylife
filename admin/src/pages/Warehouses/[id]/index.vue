<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute, productAttributeTypesShow} from "@/services/ProductAttributes";
import {IWarehouseDetail, IZone} from "@/services/WarehouseService";

// LifeCycles
onMounted(() => {
  getWarehouseDetail()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const warehouseDetail = ref<IWarehouseDetail>(null)
const route = useRoute()
const selectedZone = ref<IZone>(null)


const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام منطقه', key: 'name'},
  {title: 'کد منطقه', key: 'code'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}


function renderDeleteDialog(item: IAttribute) {
  selectedZone.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  attributesFilters.value.pageNumber = +pageNumber
  await getWarehouseDetail()
}

async function getWarehouseDetail() {
  try {
    spinner.showSpinner()
    const response = await $api?.warehouse.getWarehouseDetail(route.params.id)
    warehouseDetail.value = response?.data.data

  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
    v-if="warehouseDetail"
  >
    <template #title>
      جزئیات {{ warehouseDetail.name }}
    </template>
    <VRow>
      <VCol cols="12" class="d-flex align-content-center justify-space-between">

        <h2>مناطق انبار</h2>
        <VBtn
          color="success"
          @click="renderCreateDialog"
        >
          ایجاد منطقه جدید
        </VBtn>
      </VCol>
      <VCol cols="12">
        <CustomTable
          :items-list="warehouseDetail.zones"
          :count="100"
          :page-number="1"
          :table-headers="tableHeaders"
          :total-count="100"
        >
          <template #actions="data">
            <VBtn
              color="transparent"
              elevation="0"
              icon="mdi-delete"
              @click="renderDeleteDialog(data.item)"
            />
          </template>
        </CustomTable>

      </VCol>
    </VRow>


    <DeleteZoneDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedZone"
      @refetch="getWarehouseDetail"
    />

    <CreateZoneDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getWarehouseDetail"
    />
  </PageWrapper>
</template>
