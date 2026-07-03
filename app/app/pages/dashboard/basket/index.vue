<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import {type IBasket, type IBasketItemDetail} from "~/services/BasketService";

definePageMeta({
  layout: "dashboard",
  auth: true

});
useHead({
  title: 'سبد خرید'
})
const {$api} = useNuxtApp<IApiProvider>()
const router = useRouter()
const basketDetail = ref<IBasket>(null)
const basketItems = ref<IBasketItemDetail[]>([])
async function getBasketDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.getUserBasket()
    basketDetail.value = response.data
    if (basketDetail.value.items) {
      getBasketItemDetails()
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

function generateBasketItemsPayload() {
  let res = []
  basketDetail.value.items.forEach((item) => {
    res.push({
      objectId: item.objectId,
      itemType: item.itemType,
      quantity: item.quantity
    })
  })
  return res
}

async function getBasketItemDetails() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.getUserBasketItems({
      items: generateBasketItemsPayload()
    })
    basketItems.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function basketToOrder() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.basketToOrder()
    if (response.isSuccess) {
      router.push('/dashboard/user/my-orders')
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
getBasketDetail()


</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>سبد خرید</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-between items-start  gap-y-4"
      >
        <div class="w-full flex flex-col gap-y-4">
          <template v-if="basketItems.length">
            <LazyDashboardBasketItemCard
                @refetch="getBasketDetail"
                v-for="item in basketItems"
                :key="item.id"
                :basket-item="item"
            >
            </LazyDashboardBasketItemCard>
            <!--            <div class="w-full flex items-center justify-center">-->
            <!--              <UtilsCustomPagination-->
            <!--                  :page-number="notificationsListFilters.pageNumber"-->
            <!--                  :count="notificationsListFilters.count"-->
            <!--                  :total-count="totalCount"-->
            <!--                  @change-page="changePage"-->
            <!--              />-->
            <!--            </div>-->
          </template>
          <div v-else class="flex flex-col items-center justify-center gap-3">
            <Icon name="icon:basket" size="60"/>
            <span>سبد خرید شما خالی است !</span>
            <nuxt-link to="/shop" class="w-full btn bg-primary rounded-full">
              رفتن به فروشگاه
            </nuxt-link>
          </div>
        </div>
        <div v-if="basketDetail?.items?.length"
             class="w-full sticky flex flex-col gap-3 bg-white bottom-8 rounded-lg shadow p-3">
          <div class="w-full flex items-center justify-between">
            <span>قیمت کالاها ({{ basketDetail.items.length }})</span>
            <span>{{ Intl.NumberFormat('fa-IR').format(basketDetail.productTotalPrice) }} تومان</span>
          </div>
          <div class="w-full flex items-center justify-between">
            <span>جمع سبد خرید</span>
            <span>{{ Intl.NumberFormat('fa-IR').format(basketDetail.finalPrice) }} تومان</span>
          </div>
          <button
              @click="basketToOrder"
              type="button" class="w-full btn btn-primary !rounded-full ">
            تایید و تکمیل سفارش
          </button>
        </div>
      </div>
    </UtilsWrappersPageWrapper>

  </div>
</template>

<style scoped></style>
