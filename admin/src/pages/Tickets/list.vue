<script setup lang="ts">
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ITicketListItem, ITicketsFilter} from '@/services/TicketService'
import {useSpinner} from '@/composables/spinner'
import {TicketTypesPersian} from '@/models/Enums/TicketTypes'

// LifeCycles
onMounted(() => {
  getTicketsList()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const ticketsList = ref<null | ITicketListItem[]>(null)
const totalCount = ref<null | string | number | undefined>(null)

const tableHeaders: ITableHeaders = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان تیکت', key: 'title'},
  {title: 'واحد پشتیبانی', key: 'type', value: (item) => `${TicketTypesPersian[item.ticketType]}`},
  {
    title: 'کاربر',
    key: 'userInfo',
    value: (item) => item.userInfo ? `${item.userInfo.firstName} ${item.userInfo.lastName}` : '-'
  },
  {
    title: 'تاریخ ایجاد',
    key: 'createDate',
    value: (item: ITicketListItem) => new Date(item.createdAt).toLocaleDateString('fa-IR')
  },
  {title: 'وضعیت تیکت', key: 'isRead'},
  {title: 'عملیات', key: 'actions'},
]

const ticketsFilter = ref<ITicketsFilter>({
  pageNumber: 1,
  count: 10,
  status: null,
  type: null,
})

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

async function getTicketsList() {
  try {
    spinner.showSpinner()

    const response = await $api?.tickets.getTicketsList(ticketsFilter.value)
    ticketsList.value = response?.data.data.data as Array<ITicketListItem>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  ticketsFilter.value.pageNumber = +pageNumber
  await getTicketsList()
}
</script>

<template>
  <PageWrapper
    has-filters
    @submitFilters="getTicketsList"
  >
    <template #filters>
      <VCol cols="3">
        <TicketStatusPicker clearable v-model="ticketsFilter.status"></TicketStatusPicker>
      </VCol>
      <VCol cols="3">
        <TicketTypePicker clearable v-model="ticketsFilter.type"></TicketTypePicker>
      </VCol>
    </template>
    <template #title>
      لیست تیکت ها
    </template>
    <CustomTable
      :items-list="ticketsList"
      :count="ticketsFilter.count"
      :page-number="ticketsFilter.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #isRead="data">
        <VChip
          v-if="data.item.isRead"
          tonal
          color="success"
        >
          خوانده شده
        </VChip>
        <VChip
          v-else
          tonal
          color="error"
        >
          خوانده نشده
        </VChip>
      </template>

      <template #actions="data">
        <VBtn
          :to="`/tickets/${data.item.id}`"
          color="transparent"
          elevation="0"
          icon="mdi-open-in-new"
        />
      </template>
    </CustomTable>
  </PageWrapper>
</template>
