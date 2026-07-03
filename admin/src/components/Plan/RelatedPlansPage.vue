<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IRelatedPlan, IRelatedPlanDetail} from "@/services/RelatedPlans";
import {useAlerts} from "@/composables/alert";


// LifeCycles
onMounted(() => {
  getPlanRelatedPlans()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const relatedPlan = ref<IRelatedPlanDetail>(null)
const selectedItem = ref<IRelatedPlanDetail>(null)
const route = useRoute()
const nextPlanPayload = ref<IRelatedPlan>({
  sourcePlanId: route.params.id,
  targetPlanId: null,
  order: null
})
const tableHeaders: VDataTable['headers'] = [
  {title: 'اولویت', key: 'order'},
  {title: 'عنوان پلن', key: 'targetTitle'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderUpdateDialog(item) {
  selectedItem.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item) {
  selectedItem.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function getPlanRelatedPlans() {
  try {
    spinner.showSpinner()

    const response = await $api?.relatedPlans.getPlanRelatedPlans(route.params.id)
    relatedPlan.value = response?.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function createNextPlan() {
  if(nextPlanPayload.value.targetPlanId){

  try {
    spinner.showSpinner()
    nextPlanPayload.value.order = relatedPlan.value.nextPlans.length ? relatedPlan.value.nextPlans[0].order : 1
    const response = await $api?.relatedPlans.createPlanRelation(nextPlanPayload.value)
    if (response.data.isSuccess) {
      useAlerts().success('عملیات با موفقیت انجام شد')
      getPlanRelatedPlans()
      nextPlanPayload.value = {
        sourcePlanId: route.params.id,
        targetPlanId: null,
        order: null
      }
    } else {
      useAlerts().error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
  }else{
    useAlerts().error('لطفا پلن بعدی را انتخاب کنید')
  }
}
</script>

<template>
  <PageWrapper
  >
    <template #title>
      اتصال پلن بعدی
    </template>
    <VRow>
      <VCol cols="12">
        <VRow>
          <VCol cols="3">
            <PlanPicker v-model="nextPlanPayload.targetPlanId"></PlanPicker>
          </VCol>
          <VCol cols="3">
            <VBtn color="primary" @click="createNextPlan">ثبت</VBtn>
          </VCol>
        </VRow>
      </VCol>
      <VCol cols="12">
        <CustomTable
          :items-list="relatedPlan?.nextPlans"
          :count="1"
          :page-number="1"
          :table-headers="tableHeaders"
          :total-count="1"
          @change-page="changePage"
        >
          <template #actions="data">
<!--            <VBtn-->
<!--              color="transparent"-->
<!--              elevation="0"-->
<!--              icon="mdi-pencil"-->
<!--              @click="renderUpdateDialog(data.item)"-->
<!--            />-->
            <VBtn
              color="transparent"
              elevation="0"
              icon="mdi-delete"
              @click="renderDeleteDialog(data.item)"
            />
          </template>
        </CustomTable>
      </VCol>
    </VRow>


    <!--    <UpdatePlanCategoryDialog-->
    <!--      v-model:dialogState="isRenderingUpdateDialog"-->
    <!--      :default-category="tempCategory"-->
    <!--      @refetch="getPlanRelatedPlans"-->
    <!--    />-->

    <DeleteRelatedPlanDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedItem"
      @refetch="getPlanRelatedPlans"
    />
  </PageWrapper>
</template>
