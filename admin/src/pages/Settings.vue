<script lang="ts" setup>
import { isAxiosError } from 'axios'
import { inject, onMounted, ref } from 'vue'
import { useSpinner } from '@/composables/spinner'
import { useAlerts } from '@/composables/alert'
import FroalaEditor from '@/components/Utilities/FroalaEditor.vue'
import type { IApiProvider } from '@/models/IApiProvider'
import type { IupdateDataPayload } from '@/services/DynamicPageService'

// LifeCycles
onMounted(() => {
  // getInsurancePaperList()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()
const settings = ref(null);


const dynamicPageDetails = ref<any>({
  option:1
})

// LifeCycles
onMounted(async () => {
  await getSettings()
})

// Functions
async function getSettings() {
  try {
    spinner.showSpinner()

    const response = await $api?.settings.getAllSettings(dynamicPageDetails.value)

    if (response?.data.isSuccess) {
      settings.value = response.data.data;
    }
    else { alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید') }
  }
  catch (error) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  }
  finally {
    spinner.hideSpinner()
  }
}

async function updateData() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.siteDynamicSettingCreate(
        {
          option:1,
          value:settings.value.value
        }
    )
    if (response?.data.isSuccess) {
      alert.success('برگه با موفقیت ویرایش شد')
      await getSettings()
    }
    else {
      alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    }
  }
  catch (error) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  }
  finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper v-if="settings">
    <template #title>
      تنظیمات
    </template>
    <VRow>
      <VCol cols="6">
        <VTextField
            v-model.trim="settings.value"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            color="success"
            density="compact"
            hide-details="auto"
            label="مقدار سود کارشناسان (تومان)"
            type="text"
            variant="outlined"
        />
      </VCol>
      <VCol
          cols="12"
          align="end"
          justify="end"
      >
        <VBtn @click="updateData">
          ویرایش
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
