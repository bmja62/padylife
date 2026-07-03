<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IPlan, IPlanPricesListFilters, IPrice, planStatusesForShow} from "@/services/PlanService";
import {useAlerts} from "@/composables/alert";
import type {IUser} from "@/services/UserService";


// LifeCycles
onMounted(() => {
  getAllPlanPrices()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingChangeStatusDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const pricesList = ref<null | IPrice[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const alert = useAlerts()
const planPricesListFilters = ref<IPlanPricesListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
  planId: null
})


const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه پلن', key: 'planId'},
  {title: 'عنوان پلن', key: 'planTitle'},
  {title: 'قیمت', key: 'price', value: (item) => `${Intl.NumberFormat('fa-IR').format(item.price)} تومان`},
  {title: 'نام متخصص', key: 'expertFullName'},
  {title: 'وضعیت متخصص', key: 'isActive'},
]

function renderDeleteDialog(item) {
  tempPlan.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

function renderChangeStatusDialog(item) {
  tempPlan.value = JSON.parse(JSON.stringify(item))
  isRenderingChangeStatusDialog.value = true
}

async function changePage(pageNumber: number | string) {
  planPricesListFilters.value.pageNumber = +pageNumber
  await getAllPlanPrices()
}

async function toggleSignUpPlan(plan: IPlan) {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.toggleSignUpPlan(plan.id)
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      getAllPlanPrices()
    } else {
      alert.error(response.data.errorMessage)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getAllPlanPrices() {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.getPlanPrices(planPricesListFilters.value)
    pricesList.value = response?.data.data.data
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
    @submitFilters="getAllPlanPrices"
  >
    <template #title>
      لیست قیمت‌های ثبت شده
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="planPricesListFilters.search"
          label="عنوان پلن"
          hide-details
          @keydown.enter="getAllPlanPrices"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <PlanPicker v-model="planPricesListFilters.planId"></PlanPicker>
      </VCol>
    </template>

    <CustomTable
      :items-list="pricesList"
      :count="planPricesListFilters.count"
      :page-number="planPricesListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #isActive="data">
        <VChip color="success" v-if="data.item.isActive">فعال</VChip>
        <VChip color="error" v-else>غیرفعال</VChip>
      </template>

      <template #status="data">
        <VChip color="primary">{{ planStatusesForShow[data.item.status] }}</VChip>
      </template>
    </CustomTable>
    <DeletePlanDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-plan="tempPlan"
      @refetch="getAllPlanPrices"
    />
    <ChangePlanStatusDialog
      v-model:dialogState="isRenderingChangeStatusDialog"
      :default-plan="tempPlan"
      @refetch="getAllPlanPrices"
    />

  </PageWrapper>
</template>
