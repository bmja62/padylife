<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IGetOrdersParams, IOrder} from "~/services/OrderService";

definePageMeta({
  layout: "dashboard",
  auth: true

});
useHead({
  title: 'سفارشات من'
})
const {$api} = useNuxtApp<IApiProvider>()
const router = useRouter()
const ordersList = ref<IOrder[]>([])
const totalCount = ref(null)
const authStore = useAuthStore()
const ordersListFilters = ref<IGetOrdersParams>({
  page: 1,
  pageSize: 10,
  userId: authStore.getUser.id,
  status: null,
  orderId: null
})

async function getAllOrders() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.order.getAllOrders(ordersListFilters.value)
    ordersList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}


function changePage(page) {
  ordersListFilters.value.page = page
  getAllOrders()
}

getAllOrders()


</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>سفارشات من</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-between items-start  gap-y-4"
      >
        <div class="w-full flex flex-col gap-y-2">
          <template v-if="ordersList.length">
            <LazyDashboardOrderCard
                v-for="item in ordersList"
                :key="item.id"
                :order="item"
            >
            </LazyDashboardOrderCard>
            <div class="w-full flex items-center justify-center">
              <UtilsCustomPagination
                  :page-number="ordersListFilters.page"
                  :count="ordersListFilters.pageSize"
                  :total-count="totalCount"
                  @change-page="changePage"
              />
            </div>
          </template>
          <div v-else class="flex flex-col items-center justify-center gap-3">
            <Icon name="icon:basket" size="60"/>
            <span>موردی یافت نشد</span>

          </div>
        </div>

      </div>
    </UtilsWrappersPageWrapper>

  </div>
</template>

<style scoped></style>
