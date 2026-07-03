<script setup lang="ts">
import {BasketItemType} from "~/services/BasketService";
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPlan} from "~/services/PlanService";

const isRendering = defineModel()
const {$api} = useNuxtApp<IApiProvider>()

interface IProps {
  plan: IPlan;
}

const props = defineProps<IProps>();
const router = useRouter()
const authStore = useAuthStore()
const discountCode = ref('')
async function addItemToBasket() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.addBasketItem({
      itemType: BasketItemType.Plan,
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
      createPaymentLink(response.data.orderId)
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function createPaymentLink(orderId) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.createPaymentLink(orderId)
    if (response.isSuccess) {
      if (response.data.link) {
      window.location.href = response.data.link
      } else {
        useAlerts().success('خرید شما با موفقیت انجام شد')
        isRendering.value = false
        router.push('/dashboard')
      }
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span v-if="props.plan">خرید {{ props.plan.title }}</span>
    </template>
    <template #default>
      <div v-if="props.plan" class="w-full flex flex-col gap-2">
        <div class="flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">قیمت</span>
          <div class="flex flex-col gap">
          <span
              :class="{'!text-gray-400 line-through':props.plan.discountPrice}">{{
              Intl.NumberFormat('fa-IR').format(props.plan.price)
            }} تومان</span>
            <span
                v-if="props.plan.discountPrice">{{
                Intl.NumberFormat('fa-IR').format(props.plan.finalPrice)
              }} تومان</span>
          </div>
        </div>
        <UtilsInputsBaseInput
        bordered
            v-model="discountCode"
            name="userName"
            placeholder="کد تخفیف"
        ></UtilsInputsBaseInput>
        <div>
          <button @click="addItemToBasket" type="button" class="btn w-full bg-primary text-white">
            {{ props.plan.price ? 'ادامه به درگاه پرداخت' : 'افزودن پلن' }}
          </button>
          <LazySharedPayByWallet :plan="{...plan,itemType:BasketItemType.Plan}" v-model:discountCode="discountCode" ></LazySharedPayByWallet>
        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>