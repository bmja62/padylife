<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {InternalPointsActionType, IUserPoints} from "@/services/PointsService";
import {IUser} from "@/services/UserService";
import IncreaseOrDecreasePointsDialog from "@/components/Users/IncreaseOrDecreasePointsDialog.vue";

// LifeCycles
onMounted(() => {
  getUserPoints()
  getUserDetail()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const userPoints = ref<IUserPoints[]>([])
const userDetails = ref<IUser>(null)
const totalCount = ref<null | string | number | undefined>(null)
const route = useRoute()
const isRenderingPointsDialog = ref(false)
const selectedActionType = ref<InternalPointsActionType>(null)

const tableHeaders: VDataTable['headers'] = [
  {title: 'امتیاز در دسترس', key: 'availablePoints'},
  {title: 'امتیاز کسب شده', key: 'earnedPoints'},
  {title: 'امتیاز مصرف شده', key: 'consumedPoints'},
  {title: 'معادل تومانی', key: 'moneyValue', value: (item) => Intl.NumberFormat('fa-IR').format(item.moneyValue)},
  {
    title: 'نرخ تبدیل',
    key: 'pointsToMoneyRatio',
    value: (item) => Intl.NumberFormat('fa-IR').format(item.pointsToMoneyRatio)
  },
]

// Functions
async function getUserPoints() {
  try {
    spinner.showSpinner()
    const response = await $api?.points.getUserPoints(route.params.id)
    userPoints.value = []
    userPoints.value.push(response.data.data)
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getUserDetail() {
  try {
    spinner.showSpinner()
    const response = await $api?.users.getUserById(route.params.id)
    userDetails.value = response.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function openIncreaseOrDecrease(actionType: InternalPointsActionType) {
  isRenderingPointsDialog.value = true
  selectedActionType.value = actionType
}

</script>

<template>
  <PageWrapper
  >
    <template #title>
      امتیازات {{ userDetails?.fullName ? userDetails?.fullName : userDetails?.phoneNumber }}
    </template>
    <template #append>
      <div class="d-flex gap-2">

        <VBtn
          color="error"
          @click="openIncreaseOrDecrease(2)"
        >
          کسر امتیاز
        </VBtn>
        <VBtn
          color="success"
          variant="flat"
          @click="openIncreaseOrDecrease(1)"
        >
          افزایش امتیاز
        </VBtn>
      </div>
    </template>
    <CustomTable
      :items-list="userPoints"
      :count="1"
      :page-number="1"
      :table-headers="tableHeaders"
      :total-count="1"
    >

    </CustomTable>
    <IncreaseOrDecreasePointsDialog
      v-model:dialogState="isRenderingPointsDialog"
      :actionType="selectedActionType"
      @refetch="getUserPoints"
    ></IncreaseOrDecreasePointsDialog>
  </PageWrapper>
</template>
