<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IExchangePointsPayload} from "~/services/UsersService";

const isRendering = defineModel()
const {$api}: IApiProvider = useNuxtApp()
const authStore = useAuthStore()
const exchangePayload = ref<IExchangePointsPayload>({
  userId: null,
  pointsToConvert: null,
  description: ''
})
const emits = defineEmits<{
  (e:'refetch'):void
}>()
async function exchangePoints() {
  try {
    useSpinner().renderSpinner()
    exchangePayload.value.userId = authStore.getUser.id
    exchangePayload.value.pointsToConvert = authStore?.getUserPoints?.availablePoints
    const response = await $api.users.exchangePoints(exchangePayload.value)
    if(response.isSuccess){
      useAlerts().success('تبدیل امتیازات با موفقیت انجام شد')
      authStore.fetchUserPoints()
      emits('refetch')
    }else {
      useAlerts().error(response.message)
    }
      isRendering.value = false
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>تبدیل امتیازات</span>
    </template>
    <template #default>

      <div class="w-full flex flex-col gap-3">
        <div class="w-full flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">امتیاز شما</span>
          <span class="font-bold">{{ authStore?.getUserPoints?.availablePoints }}</span>
        </div>
        <div class="w-full flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">نرخ تبدیل</span>
          <div class="flex flex-col items-center justify-center ">
            <span class="font-bold">{{ authStore.getUserPoints.pointsToMoneyRatio }} امتیاز</span>
            <span class="font-bold">=</span>
            <span class="font-bold">1000 تومان</span>

          </div>
        </div>
        <div class="w-full flex items-center  justify-between">
          <span class="text-gray-400">دریافتی شما</span>
          <span class="font-bold">{{
              Intl.NumberFormat('fa-IR').format(authStore.getUserPoints.moneyValue)
            }} تومان</span>
        </div>
        <button type="button"
                @click="exchangePoints"
                :disabled="authStore.getUserPoints.availablePoints==0"
                class="btn  bg-primary text-white !rounded-xl">تبدیل امتیازات
        </button>
      </div>

    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>