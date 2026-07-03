<script lang="ts" setup>
import {inject, onMounted, ref} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";

// LifeCycles
onMounted(() => {
  getOrderById()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const route = useRoute()
const orderData = ref(null)
const alert = useAlerts()
// Functions
const getPaymentGateway = computed(() => {
  let result = '-'
  if (orderData.value.paymentGateWay) {
    +orderData.value.paymentGateWay === 1 ? result = 'ملت' : result = 'پارسیان'
  }
  return result
})

async function updateOrderSerialDescription() {
  try {
    spinner.showSpinner()

    const res = await $api?.order.updateOrderSerialDescriptionByAdmin({
      orderId: parseInt(route.params.id),
      orderSerialDescription: orderData.value.orderSerialDescription
    })
    if (res.data.isSuccess) {
      alert.success('سریال سفارش با موفقیت بروزرسانی شد')
      getOrderById()
    } else {
      alert.error(res.data.errorMessage)
    }
  } catch (e) {
    console.log(e)
  } finally {
    spinner.hideSpinner()

  }
}

async function updateOrderDescription() {
  try {
    spinner.showSpinner()

    const res = await $api?.orders.updateOrderDescriptionByAdmin({
      orderId: parseInt(route.params.id),
      orderSerialDescription: orderData.value.description
    })
    if (res.data.isSuccess) {
      alert.success('سریال سفارش با موفقیت بروزرسانی شد')
      getOrderById()
    } else {
      alert.error(res.data.errorMessage)
    }
  } catch (e) {
    console.log(e)
  } finally {
    spinner.hideSpinner()

  }
}

async function getOrderById() {
  try {
    spinner.showSpinner()

    const response = await $api?.order.getOrderById(route.params.id)

    orderData.value = response?.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}


</script>

<template>
  <PageWrapper
    v-if="orderData"
  >
    <template #title>
      جزئیات سفارش
    </template>
    <v-container>
      <v-row>
        <v-col cols="12">
          <v-row>
            <v-col class="d-none d-lg-flex" cols="2"></v-col>
            <v-col cols="6" lg="2" class="my-3 my-lg-0">
              <v-row justify="center" align="center" class="text-center">
                <v-col class="d-flex flex-column">
                  <strong class="text-muted font-md">شماره سفارش</strong>
                  <strong>{{ orderData.id }}</strong>
                </v-col>
              </v-row>
            </v-col>
            <v-col cols="6" lg="2" class="my-3 my-lg-0">
              <v-row justify="center" align="center" class="text-center">
                <v-col class="d-flex flex-column">
                  <strong class="text-muted font-md">وضعیت</strong>
                  <span class="text-center">{{ orderData.status }}</span>
                </v-col>
              </v-row>
            </v-col>
            <v-col cols="6" lg="2" class="my-3 my-lg-0">
              <v-row justify="center" align="center" class="text-center">
                <v-col class="d-flex flex-column">
                  <strong class="text-muted font-md">تاریخ</strong>
                  <strong v-if="orderData.createdAt">{{
                      new Date(orderData.createdAt).toLocaleDateString('fa-IR')
                    }}</strong>
                </v-col>
              </v-row>
            </v-col>
            <v-col cols="6" lg="2" class="my-3 my-lg-0">
              <v-row justify="center" align="center" class="text-center">
                <v-col class="d-flex flex-column">
                  <strong class="text-muted font-md">مجموع</strong>
                  <strong>{{ Intl.NumberFormat('fa-IR').format(orderData.totalAmount) }}
                    <small class="Toman">تومان</small>
                  </strong>
                </v-col>
              </v-row>
            </v-col>
            <v-col class="d-none d-lg-flex" cols="2"></v-col>
          </v-row>
        </v-col>

        <v-col cols="12">
          <h2 class="my-3"><strong>اطلاعات سفارش</strong></h2>
          <v-row class="my-4 px-xl-4 px-1 ">
            <v-col cols="12" class="my-4">
              <h3><strong>محصولات</strong></h3>
            </v-col>
            <v-divider></v-divider>

            <v-col cols="12" v-if="orderData && orderData.items">
              <v-card v-for="(item, idx) in orderData.items" :key="idx" flat
                      class="SingleOrderProduct  pb-3">
                <v-row align="center" justify="space-between">
                  <v-col>
                    <div>{{ item.quantity }} x
                      <span
                        class="StandardLinkTag text-lightPink">{{ item.title }}
                      </span>
                    </div>

                    <!--                 -->
                    <!--                    <div v-for="el in item.options" :key="el.optionValue" class="d-flex align-items-center gap-2">-->
                    <!--                      <small v-if="!el.optionValue.includes('#')" class="text-secondary">{{ el.optionValue }}</small>-->
                    <!--                      <v-avatar style="width: 20px;height: 20px" v-else-->
                    <!--                                :style="{ backgroundColor: el.optionValue }"></v-avatar>-->
                    <!--                    </div>-->
                    <!--                    -->
                  </v-col>

                  <v-col class="d-flex align-items-center gap-2">
                    <small v-if="item">{{ Intl.NumberFormat('fa-IR').format(item.unitPrice) }} <small class="Toman">تومان</small></small>
                  </v-col>
                </v-row>

              </v-card>
            </v-col>

            <!-- Additional Content - Summarized for Brevity -->

          </v-row>
          <v-row class="my-3">
            <v-col cols="12">
              <v-card flat class="my-4">
                <!-- Shipping Method -->
                <!--                <v-row class="my-1" v-if="orderData.shipping" justify="space-between" align="center">-->
                <!--                  <v-col cols="6" class="text-right">-->
                <!--                    <strong>روش ارسال:</strong>-->
                <!--                  </v-col>-->
                <!--                  <v-col cols="6" class="text-left text-muted">-->
                <!--                    {{ orderData.shipping.shippingTypeName }}-->
                <!--                  </v-col>-->
                <!--                </v-row>-->
                <v-divider/>
                <!-- Payment Method -->
                <v-divider/>
                <!-- Total Price Without Discount -->
                <v-row class="my-1" justify="space-between" align="center">
                  <v-col cols="6" class="text-right">
                    <strong>مجموع بدون تخفیف:</strong>
                  </v-col>
                  <v-col cols="6" class="text-left">
                    {{ Intl.NumberFormat('fa-IR').format(orderData.totalAmount) }} تومان
                  </v-col>
                </v-row>
              </v-card>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>

  </PageWrapper>
</template>
