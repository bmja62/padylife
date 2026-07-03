<template>
  <div>
    <PageWrapper>
      <template #title>
        جزئیات کیف پول
      </template>
      <template #append>
        <v-btn class="mx-1" color="green" @click="setTransactionAction(2)">
          افزایش موجودی
        </v-btn>
        <v-btn class="mx-1" color="red" @click="setTransactionAction(1)">
          کاهش موجودی
        </v-btn>
      </template>
      <v-row>
        <v-col cols="3">
          <v-card
            class="mx-auto"
            max-width="344"
            prepend-icon="mdi-account"
            title="نام کاربر"
          >
            <v-card-text>
              {{ walletDetail?.user?.fullName ? walletDetail?.user?.fullName : walletDetail?.user?.phoneNumber }}
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="3">
          <v-card
            class="mx-auto"
            max-width="344"
            title="موجودی"
          >
            <template v-slot:prepend>
              <v-icon color="primary" icon="mdi-money"></v-icon>
            </template>
            <v-card-text>{{ Intl.NumberFormat('fa-IR').format(walletDetail?.credit) }} تومان</v-card-text>
          </v-card>
        </v-col>

      </v-row>

    </PageWrapper>
    <PageWrapper>
      <template #title>
        تراکنش های کیف پول
      </template>
      <CustomTable
        :count="walletTransactionFilters.count"
        :items-list="transactionsList"
        :page-number="walletTransactionFilters.pageNumber"
        :table-headers="tableHeaders"
        :total-count="totalCount"
        @change-page="changePage"
      >
        <template #operation="data">
          <VChip
            v-if="data.item.operation==='Deposit'"
            color="green"
            label
          >
            واریز
          </VChip>
          <VChip
            v-if="data.item.operation==='Withdraw'"
            color="red"
            label
          >
            برداشت
          </VChip>
        </template>
        <template #view="data">
          <VIcon
            v-if="data.item.description"
            class="cursor-pointer"
            color="blue"
            icon="tabler-eye"
            @click="renderDetailModal(data.item)"
          />

        </template>
      </CustomTable>


    </PageWrapper>
    <CreateTransactionDialog
      v-model:dialogState="isRenderingCreateDialog"
      :transactionAction="transactionAction"
      @refetch="refetchAll"
    ></CreateTransactionDialog>
    <TransactionDetailDialog
      v-model:dialogState="isRenderingViewModal"
      :selectedTransaction="selectedTransaction"
    ></TransactionDetailDialog>
  </div>

</template>

<script lang="ts" setup>
import {ITableHeaders} from "@/models/ITableHeader";
import CreateTransactionDialog from "@/components/Transactions/CreateTransactionDialog.vue";
import {IApiProvider} from "@/models/IApiProvider";
import {useSpinner} from "@/composables/spinner";
import TransactionDetailDialog from "@/components/Transactions/TransactionDetailDialog.vue";
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {IWallet, IWalletTransaction} from "@/services/WalletService";

const route = useRoute();
const walletTransactionFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
})
const transactionAction = ref<number>(1)
const transactionsList = ref<IWalletTransaction[]>([])
const selectedTransaction = ref<IWalletTransaction>(null)
const walletDetail = ref<IWallet>(null)
const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingViewModal = ref<boolean>(false)
const totalCount = ref<null | string | number | undefined>(null)
const tableHeaders: ITableHeaders = [
  {title: 'شناسه', key: 'id'},
  {title: 'توسط', key: 'createdByUser.fullName'},
  {
    title: 'مقدار',
    key: 'amount',
    value: (item: IWalletTransactionsListItem) => `${Intl.NumberFormat('fa-IR').format(item.amount)} تومان`
  },
  {
    title: 'نوع عملیات',
    key: 'operation',
  },
  {
    title: 'مشاهده',
    key: 'view',
  },
]

onMounted(async () => {
  await Promise.all([
    getWalletDetail(),
    getAllWalletTransactions()
  ])
})


async function changePage(pageNumber: number | string) {
  walletTransactionFilters.value.pageNumber = +pageNumber
  await getAllWalletTransactions()
}

async function getWalletDetail() {
  try {
    spinner.showSpinner()

    const response = await $api?.wallet.getWalletById(+route.params.id)
    walletDetail.value = response?.data.data as IWallet
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getAllWalletTransactions() {
  try {
    spinner.showSpinner()
    const response = await $api?.wallet.getWalletTransactionsById(walletTransactionFilters.value, route.params.id)
    transactionsList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function setTransactionAction(action: number) {
  transactionAction.value = action
  isRenderingCreateDialog.value = true

}

function refetchAll() {
  getAllWalletTransactions()
  getWalletDetail()
}

function renderDetailModal(walletTransaction: IWalletTransaction) {
  selectedTransaction.value = walletTransaction
  isRenderingViewModal.value = true
}
</script>

<style scoped>

</style>
