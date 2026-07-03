<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import { inject } from 'vue'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import { useAlerts } from "@/composables/alert";
import CustomDatePicker from "@/components/Utilities/CustomDatePicker.vue";


// LifeCycles
onMounted(() => {
  getPlanDiscount()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const route = useRoute()
const planDiscount = ref<any>({})

async function getPlanDiscount() {
  try {
    spinner.showSpinner()

    const response = await $api?.plan.getPlanDiscount(route.params.id)
    planDiscount.value = response?.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function deletePriceDiscount() {
  planDiscount.value.discountPriceStartDate = null
  planDiscount.value.discountPriceEndDate = null
  planDiscount.value.discountPrice = null
  setPlanDiscount()
}

async function setPlanDiscount() {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.setPlanDiscount(planDiscount.value)
    if (response.data.isSuccess) {
      useAlerts().success('عملیات با موفقیت انجام شد')
      planDiscount.value = response?.data.data
      getPlanDiscount()
    } else {
      useAlerts().error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper>
    <template #title>
      تخفیف
    </template>
    <VRow>
      <VCol cols="6">
        <CustomDatePicker v-model="planDiscount.discountPriceStartDate" density="compact"
          input-id="discountPriceStartDate" label="تاریخ شروع تخفیف را انتخاب کنید" />
      </VCol>
      <VCol cols="6">
        <CustomDatePicker v-model="planDiscount.discountPriceEndDate" density="compact" input-id="discountPriceEndDate"
          label="تاریخ پایان تخفیف را انتخاب کنید" />
      </VCol>
      <VCol cols="12">
        <VTextField v-model="planDiscount.discountPrice" type="text"
          :rules="[(value) => !!value || 'این فیلد اجباری است']" required label="مبلغ تخفیف (تومان)" />
      </VCol>
      <VCol md="12" cols="12" class="d-flex justify-end gap-3">
        <VBtn @click="deletePriceDiscount" type="button" color="error">
          حذف تخفیف
        </VBtn>
        <VBtn @click="setPlanDiscount" type="button" color="green">
          ثبت
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
