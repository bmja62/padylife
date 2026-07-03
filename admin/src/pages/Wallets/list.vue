<script setup lang="ts">
import {inject, onMounted, ref} from 'vue'
import type {ITableHeaders} from '@/models/ITableHeader'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IGetWalletsParams, IWallet} from "@/services/WalletService";

// LifeCycles
onMounted(() => {
  getAllWallets()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const walletsList = ref<null | IWallet[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tableHeaders: ITableHeaders<IWallet> = [
  {title: 'شناسه', key: 'id'},
  {
    title: 'نام کاربر',
    key: 'user',
  },

  {
    title: 'موجودی',
    key: 'credit',
    value: (item: IWallet) => `${Intl.NumberFormat('fa-IR').format(item.credit)} تومان `
  },
  {title: 'عملیات', key: 'actions'},
]

const walletsListFilters = ref<IGetWalletsParams>({
  pageNumber: 1,
  count: 10,
  userFullName: '',
  roleName : '',
})

// Functions

async function getAllWallets() {
  try {
    spinner.showSpinner()
    const response = await $api?.wallet.getAllWallets(walletsListFilters.value)
    walletsList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  walletsListFilters.value.pageNumber = +pageNumber
  await getAllWallets()
}

async function resetFiltersAndgetAllWallets() {
  walletsListFilters.value.pageNumber = 1
  await getAllWallets()
}
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="resetFiltersAndgetAllWallets"
  >
    <template #title>
      لیست کیف پول‌ها
    </template>

    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="walletsListFilters.userFullName"
          label="نام کاربر"
          hide-details
          @keydown.enter="getAllWallets"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <RolePicker :return-object="false" v-model="walletsListFilters.roleName"></RolePicker>
      </VCol>
    </template>
    <CustomTable
      :items-list="walletsList"
      :count="walletsListFilters.count"
      :page-number="walletsListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #user="data">
        <span>{{data.item.user.fullName ? data.item.user.fullName : data.item.user.phoneNumber}}</span>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Wallets/${data.item.id}`"
        >
          <VIcon icon="mdi-edit"></VIcon>
        </VBtn>
      </template>
    </CustomTable>


  </PageWrapper>
</template>
