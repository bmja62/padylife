<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IPlan, IPlansListFilters, planStatusesForShow} from "@/services/PlanService";
import {useAlerts} from "@/composables/alert";
import {useAuthStore} from "@/stores/auth";


// LifeCycles
onMounted(() => {
  getAllPlans()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingChangeStatusDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const plansList = ref<null | IPlan[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempPlan = ref<IPlan>()
const alert = useAlerts()
const authStore = useAuthStore()
const plansListFilters = ref<IPlansListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
  userId:authStore.getUser.id

})


const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'عنوان پلن', key: 'title'},
  {title: 'دسته بندی', key: 'planCategoryName'},
  {title: 'نمایش در ثبت نام', key: 'isSignUpPlan'},
  {title: 'وضعیت', key: 'status'},
  {title: 'عملیات', key: 'actions'},
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
  plansListFilters.value.pageNumber = +pageNumber
  await getAllPlans()
}

async function toggleSignUpPlan(plan: IPlan) {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.toggleSignUpPlan(plan.id)
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      getAllPlans()
    } else {
      alert.error(response.data.errorMessage)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
async function getAllPlans() {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.getAllPlans(plansListFilters.value)
    plansList.value = response?.data.data.data as Array<ItempPlan>
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
    @submitFilters="getAllPlans"
  >
    <template #title>
      لیست پلن ها
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="plansListFilters.search"
          label="عنوان پلن"
          hide-details
          @keydown.enter="getAllPlans"
        />
      </VCol>
    </template>

    <CustomTable
      :items-list="plansList"
      :count="plansListFilters.count"
      :page-number="plansListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >

      <template #isSignUpPlan="data">
        <CustomSwitch v-if="$can('manage','all')" :inert="data.item.isSignUpPlan" @click="toggleSignUpPlan(data.item)" v-model="data.item.isSignUpPlan" :has-tooltip="false"
                      :has-icon="false"></CustomSwitch>

      </template>
      <template #status="data">
        <VChip color="primary">{{ planStatusesForShow[data.item.status] }}</VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Plan/${data.item.id}`"
          icon="mdi-pencil"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ItempPlan)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-arrow-expand"
          @click="renderChangeStatusDialog(data.item as ItempPlan)"
        >

          <VIcon icon="mdi-arrow-expand"></VIcon>
          <VTooltip
            activator="parent"
          >
تغییر وضعیت
          </VTooltip>
        </VBtn>
        <VBtn
          color="transparent"
          elevation="0"
          :to="`/Plan/${data.item.id}/pricing`"
        >
          <VIcon icon="mdi-dollar"></VIcon>
          <VTooltip
            activator="parent"
          >
            قیمت گذاری
          </VTooltip>
        </VBtn>

      </template>
    </CustomTable>
    <DeletePlanDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-plan="tempPlan"
      @refetch="getAllPlans"
    />
    <ChangePlanStatusDialog
      v-model:dialogState="isRenderingChangeStatusDialog"
      :default-plan="tempPlan"
      @refetch="getAllPlans"
    />

  </PageWrapper>
</template>
