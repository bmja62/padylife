<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IRelatedPlanDetail} from "~/services/PlanService";
import {BasketItemType} from "~/services/BasketService";
import {UtilsInputsBaseInput} from "#components";

const isRendering = defineModel()
const { $api }: IApiProvider = useNuxtApp()
const authStore = useAuthStore()
const router = useRouter()
const discountCode = ref('')

interface IProps {
  relatedPlans: IRelatedPlanDetail
}

const props: IProps = defineProps({
  relatedPlans: {
    type: Object as PropType<IRelatedPlanDetail>
  }
})

async function addItemToBasket(item) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.addBasketItem({
      itemType: BasketItemType.Plan,
      objectId: item.targetPlanId,
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
  <LazyUtilsDialogsBaseDialog dialog-id="relatedPlans" v-model="isRendering">
    <template #title>
      <span class="text-lg"> موفق شدی این پلن رو به پایان برسونی!
      </span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">
        <span class="text-base">با توجه به برنامه ای که ما واست تنظیم کردیم پلن زیر میتونه گزینه خوبی برات باشه</span>
        <div   v-for="item in relatedPlans?.nextPlans" class="w-full flex flex-col gap-3">
          <div class="w-full border border-gray-300 rounded-xl flex items-center justify-between gap-3 shadow p-3"
          >
            <span>{{ item.targetTitle }}</span>
            <span class="text-primary">{{ Intl.NumberFormat('fa-IR').format(item.finalPrice) }} تومان</span>
          </div>
          <div class="w-full">
            <UtilsInputsBaseInput bordered v-model="discountCode" name="userName" placeholder="کد تخفیف">
            </UtilsInputsBaseInput>
          </div>
          <button type="button" class="w-full btn btn-primary" @click="addItemToBasket(item)">خرید</button>
          <LazySharedPayByWallet :plan="{finalPrice:item.finalPrice,id:item.targetPlanId,itemType:BasketItemType.Plan}" v-model:discountCode="discountCode" ></LazySharedPayByWallet>

        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped></style>