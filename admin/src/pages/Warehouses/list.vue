<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IProductCategory} from "@/services/wareHouses";
import {IGetWarehouseParams, IWarehouse} from "@/services/WarehouseService";
import UpdateWarehouseDialog from "@/components/Warehouses/UpdateWarehouseDialog.vue";
import DeleteWarehouseDialog from "@/components/Warehouses/DeleteWarehouseDialog.vue";
import CreateWarehouseDialog from "@/components/Warehouses/CreateWarehouseDialog.vue";

// LifeCycles
onMounted(() => {
  getWarehouses()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const warehousesList = ref<IWarehouse[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedWarehouse = ref<IWarehouse>()

const wareHousesFilters = ref<IGetWarehouseParams>({
  pageNumber: 1,
  count: 10,
  search: '',
  isActive: true
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام انبار', key: 'name'},
  {title: 'کد انبار', key: 'code'},
  {title: 'وضعیت', key: 'isActive'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IProductCategory) {
  selectedWarehouse.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: IProductCategory) {
  selectedWarehouse.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  wareHousesFilters.value.pageNumber = +pageNumber
  await getWarehouses()
}

async function getWarehouses() {
  try {
    spinner.showSpinner()
    const response = await $api?.warehouse.getWarehouses(wareHousesFilters.value)
    warehousesList.value = response?.data.data.data as Array<IProductCategory>
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
    @submit-filters="getWarehouses"
  >
    <template #title>
      لیست انبارها
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد انبار جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="wareHousesFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getWarehouses"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <EntityIsActivePicker v-model="wareHousesFilters.isActive"></EntityIsActivePicker>
      </VCol>
    </template>
    <CustomTable
      :items-list="warehousesList"
      :count="wareHousesFilters.count"
      :page-number="wareHousesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >

      <template #isActive="data">
        <VChip color="success" v-if="data.item.isActive">فعال</VChip>
        <VChip color="error" v-else>غیرفعال</VChip>
      </template>
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
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-eye"
          :to="`/Warehouses/${data.item.id}`"
        />
      </template>
    </CustomTable>

    <UpdateWarehouseDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selected-item="selectedWarehouse"
      @refetch="getWarehouses"
    />

    <DeleteWarehouseDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedWarehouse"
      @refetch="getWarehouses"
    />

    <CreateWarehouseDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getWarehouses"
    />
  </PageWrapper>
</template>
