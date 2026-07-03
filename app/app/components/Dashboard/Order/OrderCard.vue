<template>
  <div class="bg-white rounded-2xl p-4 shadow-md mb-6 space-y-3 border border-gray-100">
    <!-- Header: Order ID + Status -->
    <div class="flex justify-between items-center text-sm">
      <span class="font-semibold text-gray-800">سفارش #{{ props.order.orderId }}</span>
      <span :class="['text-xs px-2 py-1 rounded-full font-medium', statusColor]">
        {{ statusLabel }}
      </span>
    </div>

    <!-- Order Date -->
    <div class="text-gray-500 text-sm">
      📅 تاریخ سفارش: {{ formattedDate }}
    </div>

    <!-- Address -->
    <div class="text-gray-600 text-sm">
      📍 آدرس: {{ props.order.address || 'ثبت نشده' }}
    </div>

    <!-- Total Price -->
    <div class="text-gray-700 font-bold text-base">
      💳 مبلغ: {{ formattedPrice }}
    </div>
    <nuxt-link :to="`/dashboard/user/my-orders/${props.order.orderId}`"
               class="w-full btn btn-primary !rounded-full ">
      مشاهده جزئیات
    </nuxt-link>
    <button type="button"
            @click="downloadInvoice"
            class="w-full btn bg-white border border-primary !text-primary !rounded-full ">
      دانلود فاکتور
    </button>
  </div>
</template>

<script setup lang="ts">
import {computed} from 'vue'
import {type IOrder, statusMap} from "~/services/OrderService";
import type {IApiProvider} from "~/models/IApiProvider";

const {$api} = useNuxtApp<IApiProvider>()

interface IProps {
  order: IOrder
}

const props: IProps = defineProps({
  order: {
    type: Object as PropType<IOrder>,
    required: true
  }
})


const statusLabel = computed(() => statusMap[props.order.status]?.label || 'نامشخص')
const statusColor = computed(() => statusMap[props.order.status]?.color || 'bg-gray-100 text-gray-500')

const formattedDate = computed(() => {
  const date = new Date(props.order.orderDate)
  return date.toLocaleDateString('fa-IR', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

const formattedPrice = computed(() =>
    Number(props.order.totalPrice).toLocaleString('fa-IR') + ' تومان'
)


async function downloadInvoice() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.order.getOrderInvoiceById(props.order.orderId)
    let url = URL.createObjectURL(response)
    let anchorTag = document.createElement('a')
    anchorTag.href = url
    anchorTag.download = `فاکتور فروش- ${props.order.orderId}`
    anchorTag.click()
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>
