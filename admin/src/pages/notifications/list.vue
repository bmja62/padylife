<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute} from "@/services/ProductAttributes";
import {IGetNotificationParams, INotification, notificationTypeMap} from "@/services/NotificationsService";
import ViewNotificationDialog from "@/components/Notifications/ViewNotificationDialog.vue";

// LifeCycles
onMounted(() => {
  getAllNotifications()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingUpdateDialog = ref<boolean>(false)
const notificationsList = ref<INotification[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedNotification = ref<INotification>()

const notificationsListFilters = ref<IGetNotificationParams>({
  pageNumber: 1,
  count: 10,
  search: '',
  userId: null
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان', key: 'subject'},
  {
    title: 'تاریخ ارسال',
    key: 'createdAt',
    value: (item: INotification) => `${new Date(item.createdAt).toLocaleDateString('fa-IR')}-${new Date(item.createdAt).toLocaleTimeString('fa-IR')}`
  },
  {title: 'خوانده شده', key: 'isRead'},
  {title: 'نوع اعلان', key: 'notificationTypes'},
  {title: 'عملیات', key: 'actions'},
]

// Functions

function renderUpdateDialog(item: IAttribute) {
  selectedNotification.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}


async function changePage(pageNumber: number | string) {
  notificationsListFilters.value.pageNumber = +pageNumber
  await getAllNotifications()
}
function applyFilters(){
  notificationsListFilters.value.pageNumber = 1
  getAllNotifications()
}

async function getAllNotifications() {
  try {
    spinner.showSpinner()
    const response = await $api?.notification.getAllNotifications(notificationsListFilters.value)
    notificationsList.value = response?.data.data.data.data
    totalCount.value = response?.data.data.data.totalCount as number

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
    @submit-filters="applyFilters"
  >
    <template #title>
      اعلانات ارسال شده به کاربران
    </template>

    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="notificationsListFilters.search"
          label="جستجو... "
          hide-details
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <UserPicker v-model="notificationsListFilters.userId" :return-object="false"></UserPicker>

      </VCol>
    </template>
    <CustomTable
      :items-list="notificationsList"
      :count="notificationsListFilters.count"
      :page-number="notificationsListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #isRead="data">
        <VChip color="success" v-if="data.item.isRead">خوانده شده</VChip>
        <VChip color="error" v-else>خوانده نشده</VChip>
      </template>
      <template #notificationTypes="data">
        <VChip color="primary">{{ notificationTypeMap[data.item.notificationTypes] }}</VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-eye"
          @click="renderUpdateDialog(data.item as IAttribute)"
        />

      </template>
    </CustomTable>

    <ViewNotificationDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selected-item="selectedNotification"
      @refetch="getAllNotifications"
    />
  </PageWrapper>
</template>
