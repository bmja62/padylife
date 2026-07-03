<script setup lang="ts">
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IGetUserFilters} from '@/services/UserService'
import {useSpinner} from '@/composables/spinner'
import {IChat} from "@/services/ChatService";

// LifeCycles
onMounted(() => {
  getAllUserChats()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const chatsList = ref<null | IChat[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tableHeaders: ITableHeaders = [
  {title: 'شناسه', key: 'chat.chatId'},
  {
    title: 'تعداد پیام ها',
    key: 'messageCount',
  },

  {
    title: 'ارسال کننده',
    key: 'chat.userFullNames[0].userFullName',
  },
  {
    title: 'گیرنده',
    key: 'chat.userFullNames[1].userFullName',
  },
  {
    title: 'تاریخ آخرین پیام',
    key: 'lastMessageTime',
    value: (item) => item.lastMessageTime ? new Date(item.lastMessageTime).toLocaleDateString('fa-IR') : '-'
  },
  {title: 'عملیات', key: 'actions'},
]

const chatsFilters = ref<IGetUserFilters>({
  pageNumber: 1,
  count: 10,
  search: '',
  roleName: '',
})

// Functions

async function getAllUserChats() {
  try {
    spinner.showSpinner()
    const response = await $api?.chat.getAllUserChats(chatsFilters.value)
    chatsList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  chatsFilters.value.pageNumber = +pageNumber
  await getAllUserChats()
}

async function resetFiltersAndgetAllUserChats() {
  chatsFilters.value.pageNumber = 1
  await getAllUserChats()
}
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="resetFiltersAndgetAllUserChats"
  >
    <template #title>
      لیست مکالمات کاربران
    </template>
    <!--    <template #append>-->
    <!--      <VBtn-->
    <!--        color="success"-->
    <!--        @click="renderCreateDialog"-->
    <!--      >-->
    <!--        ایجاد کاربر جدید-->
    <!--      </VBtn>-->
    <!--    </template>-->
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="chatsFilters.search"
          label="نام کاربر"
          hide-details
          @keydown.enter="getAllUserChats"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="chatsList"
      :count="chatsFilters.count"
      :page-number="chatsFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Users/chats/${data.item.chat.chatId}`"
        >
          <VIcon icon="mdi-edit"></VIcon>
          <VTooltip
            activator="parent"
          >
            مشاهده مکالمه
          </VTooltip>
        </VBtn>
      </template>
    </CustomTable>
  </PageWrapper>
</template>
