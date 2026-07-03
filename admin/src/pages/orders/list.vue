<script lang="ts" setup>
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IOrdersListItem, orderStatusBgMap, orderStatusPersianMap,} from '@/services/OrderService'
import ChangeOrderStatusDialog from "@/components/Order/ChangeOrderStatusDialog.vue";
import OrderStatusPicker from "@/components/Order/OrderStatusPicker.vue";

// LifeCycles
onMounted(() => {
  getAllOrders()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const ordersList = ref<null | IOrdersListItem[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const isRenderingChangeStatusDialog = ref(false)
const tableHeaders: ITableHeaders = [
  {title: 'شناسه سفارش', key: 'orderId'},
  {title: 'کاربر', key: 'userInfo.userName'},
  // {title: 'شماره تلفن', key: 'userInfo.phoneNumber'},
  {
    title: 'تاریخ ثبت',
    key: 'orderDate',
    value: (item: IOrdersListItem) => `${new Date(item.orderDate).toLocaleTimeString('fa-IR', {hour:'numeric',minute:'numeric'})}  - ${new Date(item.orderDate).toLocaleDateString('fa-IR')}`,
  },
  {
    title: 'وضعیت سفارش',
    key: 'status',
  },

  {
    title: 'مبلغ',
    key: 'finalPrice',
    value: (item: IOrdersListItem) => `${Intl.NumberFormat('fa-IR').format(item.totalPrice)} تومان`,
  },
  {
    title: 'مشاهده',
    key: 'view',
  },
]
const tempOrder = ref(null)
const ordersListFilters = ref({
  page: 1,
  pageSize: 10,
  orderId: '',
  status: null
})

// Functions
async function getAllOrders() {
  try {
    spinner.showSpinner()

    const response = await $api?.order.getAllOrders(ordersListFilters.value)
    ordersList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  ordersListFilters.value.page = +pageNumber
  await getAllOrders()
}

function renderChangeStatusDialog(item: ICreateOrUpdateBrandPayload) {
  tempOrder.value = JSON.parse(JSON.stringify(item))
  isRenderingChangeStatusDialog.value = true
}

async function resetFiltersAndgetAllOrders() {
  ordersListFilters.value.pageNumber = 1
  await getAllOrders()
}


</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="resetFiltersAndgetAllOrders"
  >
    <template #title>
      لیست سفارشات
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="ordersListFilters.orderId"
          hide-details
          label="شناسه سفارش"
          @keydown.enter="getAllOrders"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <OrderStatusPicker v-model="ordersListFilters.status"></OrderStatusPicker>
      </VCol>
    </template>
    <CustomTable
      :count="ordersListFilters.pageSize"
      :items-list="ordersList"
      :page-number="ordersListFilters.page"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #status="data">
        <VChip :color="orderStatusBgMap[data.item.status]" style="cursor: pointer"
               @click="renderChangeStatusDialog(data.item)"
               append-icon="tabler-chevron-down">
          {{ orderStatusPersianMap[data.item.status] }}
        </VChip>
      </template>
      <template #view="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/orders/${data.item.orderId}`"
          class=""
          icon="mdi-open-in-new"
        />
      </template>
    </CustomTable>
    <ChangeOrderStatusDialog
      :default-order="tempOrder"
      @refetch="getAllOrders"
      v-model:dialogState="isRenderingChangeStatusDialog"
    />
  </PageWrapper>

</template>
