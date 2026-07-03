<script setup lang="ts">
import {BasketItemType} from "~/services/BasketService";
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPrice} from "~/services/PlanService";

const isRendering = defineModel()
const {$api} = useNuxtApp<IApiProvider>()

interface IProps {
  planId: string | number,
  userId: string | number,
}

const props: IProps = defineProps({
  planId: {
    type: Number as PropType<number>
  },
  userId: {
    type: Number as PropType<number>
  },
})

watch(() => props, async () => {
  getExpertPlanPrice()
}, {deep: true})

const discountCode = ref('')
const router = useRouter()
const expertPlanPriceInfo = ref<IPrice>(null)
async function getExpertPlanPrice() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanPriceForUI({
      expertId: props.userId,
      planId: props.planId,
      isActive: true
    })
    expertPlanPriceInfo.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

const authStore = useAuthStore()

async function addItemToBasket() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.addBasketItem({
      itemType: BasketItemType.ExpertPlanPrice,
      objectId: expertPlanPriceInfo.value.id,
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
      <span>درخواست همراهی از متخصص</span>
    </template>
    <template #default>
      <div v-if="expertPlanPriceInfo" class="w-full flex flex-col gap-2">
        <div class="flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">نام متخصص</span>
          <strong>{{ expertPlanPriceInfo.expertFullName }}</strong>
        </div>
        <div class="flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">پلن</span>
          <strong>{{ expertPlanPriceInfo.planTitle }}</strong>
        </div>
        <div class="flex items-center border-b pb-2 justify-between">
          <span class="text-gray-400">قیمت</span>
          <strong class="text-primary">{{ Intl.NumberFormat('fa-IR').format(expertPlanPriceInfo.price) }} تومان</strong>
        </div>
        <div class="w-full border-b pb-2">
          <UtilsInputsBaseInput
          bordered
              v-model="discountCode"
              name="userName"
              placeholder="کد تخفیف"
          ></UtilsInputsBaseInput>
        </div>
        <div>
          <button @click="addItemToBasket" type="button" class="btn w-full bg-primary text-white">
            ادامه به درگاه پرداخت
          </button>
          <LazySharedPayByWallet :plan="{finalPrice:expertPlanPriceInfo.price,id:expertPlanPriceInfo.id,itemType:BasketItemType.ExpertPlanPrice}"
                                 v-model:discountCode="discountCode"></LazySharedPayByWallet>

        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>