<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IWalletDetail} from "~/services/WalletService";

const {$api}: IApiProvider = useNuxtApp()
const walletDetail = ref<IWalletDetail>(null)
const isRenderingChargeWalletDialog = ref(false)
const isRenderingExchangeDialog = ref(false)

async function getMyWallet() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.wallet.getMyWallet()
    walletDetail.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

getMyWallet()

</script>

<template>
  <div
      v-if="walletDetail"
      class="w-full  rounded-[16px] bg-white border border-gray-200 flex flex-row justify-between items-center p-2"
  >
    <div class="flex flex-col w-full gap-2">
      <div class="w-full flex items-center justify-between">
        <strong class="text-sm text-gray-600 ">کیف پول</strong>
        <NuxtImg
            src="/core/wallet.png"
            class="w-[30px] h-[30px] object-contain"
        />
      </div>
      <div class="w-full flex items-center justify-between">
        <span class="text-gray-700 text-sm">{{ Intl.NumberFormat('fa-IR').format(walletDetail.credit)  }} تومان</span>
        <div class="flex items-center gap-1">
          <button @click="isRenderingExchangeDialog = true" type="button"
                  class="btn btn-sm bg-white [&_*]:fill-primary border-primary !rounded-xl tooltip"
                  data-tip="تبدیل امتیازات">
            <Icon name="icon:refresh" size="10"
                  class=""></Icon>
          </button>
          <button @click="isRenderingChargeWalletDialog = true" type="button"
                class="btn btn-sm bg-primary text-white !rounded-xl">شارژ کیف پول
        </button>
        </div>
      </div>
    </div>
    <LazyUtilsDialogsChargeWalletDialog
        v-model="isRenderingChargeWalletDialog"></LazyUtilsDialogsChargeWalletDialog>
    <LazyDashboardWalletExchangePointsDialog
        @refetch="getMyWallet"
        v-model="isRenderingExchangeDialog"></LazyDashboardWalletExchangePointsDialog>
  </div>
</template>

<style scoped>

</style>