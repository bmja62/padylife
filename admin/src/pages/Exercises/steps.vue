<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type { VDataTable } from 'vuetify/components'
import { inject } from 'vue'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import { IStepsListFilters, ITempStep } from "@/services/StepsService";
import CreateStepDialog from "@/components/Steps/CreateStepDialog.vue";
import DeleteStepDialog from "@/components/Steps/DeleteStepDialog.vue";
import UpdateStepDialog from "@/components/Steps/UpdateStepDialog.vue";
import {useAuthStore} from "@/stores/auth";


// LifeCycles
onMounted(() => {
  getAllSteps()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const stepsList = ref<null | ITempStep[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempStep = ref<ITempStep>()
const authStore = useAuthStore()
const stepsListFilters = ref<IStepsListFilters>({
  pageNumber: 1,
  count: 10,
  search: null,
  allUsers:authStore.getUser.roles.filter(e=> e.name === 'Admin').length ? true : false
})

const tableHeaders: VDataTable['headers'] = [
  { title: 'شناسه', key: 'id' },
  { title: 'نام', key: 'name' },
  { title: 'عملیات', key: 'actions' },
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item) {
  tempStep.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item) {
  tempStep.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  stepsListFilters.value.pageNumber = +pageNumber
  await getAllSteps()
}

async function getAllSteps() {
  try {
    spinner.showSpinner()

    const response = await $api?.steps.getAllStepsByFilter(stepsListFilters.value)
    stepsList.value = response?.data.data.data as Array<ItempStep>
    totalCount.value = response?.data.data.totalCount as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function resetFiltersAndGet() {
  stepsListFilters.value.pageNumber = 1
  getAllSteps()
}
</script>

<template>
  <PageWrapper has-filters @submit-filters="resetFiltersAndGet">
    <template #title>
      لیست مراحل تمرینات
    </template>
    <template #append>
      <VBtn color="success" @click="renderCreateDialog">
        ایجاد مرحله جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol cols="12" md="3">
        <VTextField v-model="stepsListFilters.search" label="جستجو... " hide-details
          @keydown.enter="resetFiltersAndGet" />
      </VCol>
    </template>
    <CustomTable :items-list="stepsList" :count="stepsListFilters.count" :page-number="stepsListFilters.pageNumber"
      :table-headers="tableHeaders" :total-count="totalCount" @change-page="changePage">
      <template #actions="data">
        <VBtn color="transparent" elevation="0" icon="mdi-pencil" @click="renderUpdateDialog(data.item as ItempStep)" />
        <VBtn color="transparent" elevation="0" icon="mdi-delete" @click="renderDeleteDialog(data.item as ItempStep)" />
        <VBtn color="transparent" elevation="0" :to="`/Exercises/step-options/${data.item.id}`">
          <VIcon icon="mdi-crop-free"></VIcon>
          <VTooltip activator="parent" location="top">
            <span>پیکربندی</span>
          </VTooltip>
        </VBtn>
      </template>
    </CustomTable>

    <UpdateStepDialog v-model:dialogState="isRenderingUpdateDialog" :default-step="tempStep" @refetch="getAllSteps" />

    <DeleteStepDialog v-model:dialogState="isRenderingDeleteDialog" :default-step="tempStep" @refetch="getAllSteps" />

    <CreateStepDialog v-model:dialogState="isRenderingCreateDialog" @refetch="getAllSteps" />
  </PageWrapper>
</template>
