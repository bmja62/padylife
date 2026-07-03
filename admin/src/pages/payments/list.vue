<script lang="ts" setup>
import { inject, onMounted, ref } from 'vue'
import type { ITableHeaders } from '@/models/ITableHeader'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import type { IPaymentListFilters, IPaymentListItem } from '@/services/PaymentService'
import CustomDatePicker from '@/components/Utilities/CustomDatePicker.vue'
import {IOrdersListItem} from "@/services/OrderService";

// LifeCycles
onMounted(() => {
  getAllPayments()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const paymentsList = ref<null | IPaymentListItem[]>(null)
const totalCount = ref<null | string | number | undefined>(null)

const tableHeaders: ITableHeaders = [
  { title: 'شناسه سفارش', key: 'orderId' },
  { title: 'کاربر', key: 'user.fullName' },
  {
    title: 'مبلغ',
    key: 'amount',
    value: (item: IPaymentListItem) => `${Intl.NumberFormat('fa-IR').format(item.amount)} تومان`,
  },
  {
    title: 'وضعیت پرداخت',
    key: 'isPayed',
  },
  {
    title: 'تاریخ پرداخت',
    key: 'createdAt',
    value: (item: IOrdersListItem) => `  ${new Date(item.createdAt).toLocaleTimeString('fa-IR', {hour:'numeric',minute:'numeric'})}- ${new Date(item.createdAt).toLocaleDateString('fa-IR')} `,
  },
]

const paymentsFilters = ref<IPaymentListFilters>({
  PageNumber: 1,
  Count: 10,
  isPayed: null,
  userFullName: null,
  from: null,
  to: null,
  userId: null,
})

// Functions
function setSelectedFromDate(date: Date) {
  paymentsFilters.value.from = date
}

function setSelectedToDate(date: Date) {
  paymentsFilters.value.to = date
}

async function getAllPayments() {
  try {
    spinner.showSpinner()

    const response = await $api?.payment.getAllPayments(paymentsFilters.value)

    paymentsList.value = response?.data.data.data as Array<IPaymentListItem>
    totalCount.value = response?.data.data.totalCount as number
  }
  catch (error) {
    console.error(error)
  }
  finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  paymentsFilters.value.PageNumber = +pageNumber
  await getAllPayments()
}

async function resetFiltersAndGetAllPayments() {
  paymentsFilters.value.PageNumber = 1
  await getAllPayments()
}

</script>

<template>
  <PageWrapper
    has-filters
    @submitFilters="resetFiltersAndGetAllPayments"
  >
    <template #title>
      لیست پرداخت ها
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="paymentsFilters.userFullName"
          hide-details
          label="نام کاربر"
        />
      </VCol>

      <VCol
        cols="12"
        md="3"
      >
        <CustomDatePicker
          density="compact"
          input-id="fromDate"
          v-model="paymentsFilters.from"
          label="تاریخ شروع را انتخاب کنید"
          @getselectedDate="setSelectedFromDate"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <CustomDatePicker
          density="compact"
          v-model="paymentsFilters.to"
          input-id="toDate"
          label="تاریخ پایان را انتخاب کنید"
          @getselectedDate="setSelectedToDate"
        />
      </VCol>
    </template>
    <CustomTable
      :count="paymentsFilters.Count"
      :items-list="paymentsList"
      :page-number="paymentsFilters.PageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #isPayed="data">
        <VChip :color="data.item.isPayed ? 'green' : 'red'">
          {{
            data.item.isPayed ? 'پرداخت شده' : 'پرداخت نشده'
          }}
        </VChip>
      </template>
    </CustomTable>
  </PageWrapper>
</template>
