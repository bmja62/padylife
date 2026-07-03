<script setup lang="ts">
import type {IWalletDetail} from "~/services/WalletService";
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPlan} from "~/services/PlanService";

const walletDetail = ref<IWalletDetail>(null)
const {$api}: IApiProvider = useNuxtApp()
const plan = defineModel('plan')
const discountCode = defineModel('discountCode')
const authStore = useAuthStore()
const router = useRouter()

interface IProps {
  plan: IPlan;
}

const props = defineProps<IProps>();

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


async function addItemToBasket() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.addBasketItem({
      itemType: props.plan.itemType,
      objectId: props.plan.id,
      quantity: 1
    }, authStore.getUser.id)
    if (response.isSuccess) {
      await basketToOrder()
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function basketToOrder() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.basketToOrder(discountCode.value)
    if (response.isSuccess) {
      payByWallet(response.data.orderId)
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function payByWallet(orderId) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.payByWallet(orderId)
    if (response.isSuccess) {
      useAlerts().success('پرداخت شما از کیف پول انجام شد')
      router.push('/dashboard')
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

await getMyWallet()

</script>

<template>
  <div class="w-full flex flex-col mt-3 gap-3">
    <div class="w-full flex items-center justify-between">
      <span>موجودی کیف پول: {{ Intl.NumberFormat('fa-IR').format(walletDetail.credit) }} تومان</span>
      <NuxtImg
          src="/core/wallet.png"
          class="w-[30px] h-[30px] object-contain"
      />
    </div>
    <button :disabled="walletDetail?.credit<props.plan.finalPrice" @click="addItemToBasket"
            type="button"
            class="btn w-full bg-primary text-white">
      پرداخت با کیف پول
    </button>
  </div>
</template>

<style scoped>

</style>