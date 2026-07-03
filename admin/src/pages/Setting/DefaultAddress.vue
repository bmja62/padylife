<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";

// Interfaces

// LifeCycles
onMounted(() => {
  getDefaultAddress()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const defaultAddress = ref<null>(null)
const alert = useAlerts()

// Functions

function setCityState(cityState) {
  defaultAddress.value.stateId = cityState.stateId
  defaultAddress.value.cityId = cityState.cityId
}

async function setDefaultAddress() {
  try {
    defaultAddress.value.latitude = null
    defaultAddress.value.longitude = null
    spinner.showSpinner()
    const response = await $api?.settings.setDefaultAddress(defaultAddress.value)
    getDefaultAddress()
    alert.success('ویرایش شد')
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getDefaultAddress() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.getDefaultAddress()
    defaultAddress.value = response?.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
  >
    <template #title>
      ویرایش آدرس پیش فرض
    </template>

    <VRow v-if="defaultAddress" class="my-2">
      <VCol cols="12" md="6">
        <VTextField
          v-model="defaultAddress.addressName"
          variant="outlined"
          density="compact"
          label="نام آدرس"
          color="success"
          dir="auto"
          required
          hide-details
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          density="compact"
          dir="auto"
          v-model="defaultAddress.companyName"
          label="نام شرکت"
          color="success"
          hide-details
        />
      </VCol>
<!--      <VCol cols="3" md="12">-->
<!--        <CityStatePicker @setCityStateId="setCityState" :default-city-id="defaultAddress.cityId"-->
<!--                         :default-state-id="defaultAddress.stateId"></CityStatePicker>-->
<!--      </VCol>-->
      <VCol cols="12" md="6">
        <VTextField
          v-model="defaultAddress.postalCode"
          variant="outlined"
          density="compact"
          label="کد پستی"
          color="success"
          dir="auto"
          required
          hide-details
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          density="compact"
          dir="auto"
          v-model="defaultAddress.phoneNumber"
          label="شماره تماس"
          color="success"
          hide-details
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextField
          density="compact"
          dir="auto"
          v-model="defaultAddress.placeText"
          label="آدرس"
          color="success"
          hide-details
        />
      </VCol>
      <VCol cols="3" md="3">
        <VBtn
          color="success"
          @click="setDefaultAddress"
        >
          بروزرسانی
        </VBtn>
      </VCol>
    </VRow>


  </PageWrapper>
</template>
