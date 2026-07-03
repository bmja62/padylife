<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import {persianStatusMap, statusMap} from "~/services/OrderService";

definePageMeta({
  layout: "dashboard",
  auth: true

});
useHead({
  title: 'جزئیات سفارش'
})
const {$api} = useNuxtApp<IApiProvider>()
const orderDetail = ref(null)
const route = useRoute()

async function getOrderById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.order.getOrderById(route.params.orderId)
    orderDetail.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}


const statusInfo = computed(
    () => persianStatusMap[orderDetail.value.status]
)

const formattedDate = computed(() =>
    new Date(orderDetail.value.createdAt).toLocaleDateString('fa-IR', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
)



const formattedTotal = computed(() => formatPrice(orderDetail.value.totalAmount))

function formatPrice(value) {
  return Number(value).toLocaleString('fa-IR') + ' تومان'
}

getOrderById()


</script>

<template>
  <div v-if="orderDetail" class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>جزئیات سفارش</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-between items-start  gap-y-4"
      >
        <!-- ===== ORDER-DETAIL SECTION (paste inside WRITE HERE) ===== -->
        <section class="w-full h-full overflow-y-auto   pb-28 space-y-6">

          <!-- ▸ Summary card -->
          <div class="w-full bg-white rounded-2xl shadow-sm p-4 space-y-2">
            <div class="flex justify-between items-center text-sm">
              <span class="font-semibold text-gray-800">سفارش #{{ orderDetail.id }}</span>

              <!-- status badge -->
              <span :class="['text-xs px-2 py-1 rounded-full font-medium', statusInfo.color]">
        {{ statusInfo.label }}
      </span>
            </div>

            <div class="text-gray-500 text-sm">
              📅 {{ formattedDate }}
            </div>

            <div class="text-gray-700 text-sm">
              💳 وضعیت پرداخت: {{ orderDetail.paymentStatus }}
            </div>

            <div class="text-gray-900 font-bold text-base">
              💰 مبلغ کل: {{ formattedTotal }}
            </div>
          </div>

          <!-- ▸ Items card -->
          <div class="w-full bg-white rounded-2xl shadow-sm p-4">
            <h3 class="mb-3 font-semibold text-gray-800">اقلام سفارش</h3>

            <ul class="divide-y divide-gray-100">
              <li
                  v-for="item in orderDetail.items"
                  :key="item.objectId"
                  class="flex justify-between py-3"
              >
                <div class="flex flex-col">
                  <span class="text-gray-800">{{ item.title }}</span>
                  <span class="text-xs text-gray-500">×{{ item.quantity }}</span>
                </div>

                <span class="font-semibold text-gray-700">
          {{ formatPrice(item.unitPrice * item.quantity) }}
        </span>
              </li>
            </ul>
          </div>

          <!-- ▸ Buyer info card -->
          <div class="w-full bg-white rounded-2xl shadow-sm p-4 space-y-1">
            <h3 class="mb-2 font-semibold text-gray-800">اطلاعات خریدار</h3>
            <p class="text-sm text-gray-700">{{ orderDetail.userInfo.fullName }}</p>
            <p class="text-xs text-gray-500">نام کاربری: {{ orderDetail.userInfo.userName }}</p>
            <p class="text-xs text-gray-500">شماره تماس: {{ orderDetail.userInfo.phoneNumber }}</p>
            <p class="text-xs text-gray-500">
              ایمیل: {{ orderDetail.userInfo.email || 'ثبت نشده' }}
            </p>
          </div>

          <!-- ▸ Address card -->
          <div class="w-full bg-white rounded-2xl shadow-sm p-4">
            <h3 class="mb-2 font-semibold text-gray-800">آدرس تحویل</h3>
            <p class="text-sm text-gray-700">
              {{ orderDetail.address || 'ثبت نشده' }}
            </p>
          </div>

        </section>

      </div>
    </UtilsWrappersPageWrapper>

  </div>

</template>

<style scoped>

</style>