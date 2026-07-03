<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";

// Interfaces
interface ItempFee extends ICreateOrUpdateBrandPayload {
  id: string | number
}

// LifeCycles
onMounted(() => {
  getDefaultBanner()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const defaultBanner = ref<null>(null)

// Functions


async function setDefaultFirstBanner() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.setDefaultFirstBanner(defaultBanner.value)
    useAlerts().success('بروزرسانی شد')
    getDefaultBanner()
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getDefaultBanner() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.getDefaultFirstBanner()
    defaultBanner.value = response?.data
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
      ویرایش بنر ویژه سایت
    </template>

    <VRow v-if="defaultBanner" class="my-2">
      <VCol cols="12" md="6">
        <VTextField
          v-model="defaultBanner.link"
          variant="outlined"
          density="compact"
          label="لینک"
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
          v-model="defaultBanner.content"
          label="متن"
          color="success"
          hide-details
        />
      </VCol>
      <VCol cols="12" md="3">
        <VBtn
          color="success"
          @click="setDefaultFirstBanner"
        >
          بروزرسانی
        </VBtn>
      </VCol>
    </VRow>


  </PageWrapper>
</template>
