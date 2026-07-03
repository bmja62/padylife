

<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IDiscount} from "@/services/DiscountService";
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import UpdateCouponDialog from "@/components/Coupons/UpdateCouponDialog.vue";
import DeleteCouponDialog from "@/components/Coupons/DeleteCouponDialog.vue";
import CreateCouponDialog from "@/components/Coupons/CreateCouponDialog.vue";

// LifeCycles
onMounted(() => {
  getAllCoupons()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const discountsList = ref<null | IDiscount[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedItem = ref<IDiscount>(null)

const discountsFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'کد ', key: 'code',},
  {
    title: 'مقدار ',
    key: 'discount',
    value: (val:IDiscount) => val.discountPercentage? `% ${val.discountPercentage}` : `${Intl.NumberFormat('fa-IR').format(val.discountAmount)} تومان`
  },
  {title: 'تاریخ پایان', key: 'expireDate', value: (val) => new Date(val.endDate).toLocaleDateString('fa-IR')},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item) {
  selectedItem.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item) {
  selectedItem.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  discountsFilters.value.pageNumber = +pageNumber
  await getAllCoupons()
}

async function getAllCoupons() {
  try {
    spinner.showSpinner()

    const response = await $api?.discounts.getAllCoupons(discountsFilters.value)
    discountsList.value = response?.data.data.data
    totalCount.value = response?.data.data.totalCount
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
    @submitFilters="getAllCoupons"
  >
    <template #title>
      لیست کدهای تخفیف
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="discountsFilters.search"
          label="جستجو... "
          hide-details
        />
      </VCol>
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد کد تخفیف جدید
      </VBtn>
    </template>

    <CustomTable
      :items-list="discountsList"
      :count="discountsFilters.count"
      :page-number="discountsFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
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
      </template>
    </CustomTable>

    <UpdateCouponDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selectedItem="selectedItem"
      @refetch="getAllCoupons"
    />

    <DeleteCouponDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selectedItem="selectedItem"
      @refetch="getAllCoupons"
    />

    <CreateCouponDialog
      @refetch="getAllCoupons"

      v-model:dialogState="isRenderingCreateDialog"
    />
  </PageWrapper>
</template>
