<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";
import {DynamicSettings, IDynamicSetting} from "@/services/SettingsService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

// Interfaces

// LifeCycles
onMounted(() => {
  getBabakSetting()
  getMotivationSetting()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const imageUrl = ref(null)
const babakSetting = ref<IDynamicSetting>(null)
const motivationSetting = ref<IDynamicSetting>(null)
const alert = useAlerts()

// Functions


async function getBabakSetting() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.getSiteDynamicSettingByKeyAndType({
      key: DynamicSettings.babak,
      type: DynamicSettings.babak
    })
    babakSetting.value = response.data.data
    babakSetting.value.jsonValue = JSON.parse(babakSetting.value.jsonValue)
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}


async function setBabakSetting() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.setSiteDynamicSetting({
      id: babakSetting.value.id,
      jsonValue: JSON.stringify(babakSetting.value.jsonValue)
    })
    if (response.data.isSuccess) {
    alert.success('ویرایش شد')
      getBabakSetting()
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}


async function getMotivationSetting() {
  try {
    spinner.showSpinner()
    const response = await $api?.settings.getSiteDynamicSettingByKeyAndType({
      key: DynamicSettings.motivation,
      type: DynamicSettings.motivation
    })
    motivationSetting.value = response.data.data
    motivationSetting.value.jsonValue = JSON.parse(motivationSetting.value.jsonValue)
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}


async function setMotivationSetting() {
  try {
    spinner.showSpinner()
    imageUrl.value.getFiles()
    const response = await $api?.settings.setSiteDynamicSetting({
      id: motivationSetting.value.id,
      jsonValue: JSON.stringify(motivationSetting.value.jsonValue)
    })
    if (response.data.isSuccess) {
      alert.success('ویرایش شد')
      getMotivationSetting()
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function setFile(medias) {
  if (medias.length) {
    motivationSetting.value.jsonValue.imageUrl = medias[0]
  }
}





</script>

<template>
  <PageWrapper
  >
    <template #title>
      تنظیمات دیگر
    </template>
    <VRow v-if="babakSetting" class="my-2">
      <VCol cols="12" md="12">
        <strong>ویرایش متن بابک</strong>
      </VCol>
      <VCol v-if="babakSetting.jsonValue" cols="12" md="6">
        <VTextField
          v-model="babakSetting.jsonValue.title"
          variant="outlined"
          density="compact"
          label="متن نمایشی بابک"
          color="success"
          dir="auto"
          hide-details
        />
      </VCol>
      <VCol cols="12" md="3">
        <VBtn
          @click="setBabakSetting"
          color="success"
        >
          بروزرسانی
        </VBtn>
      </VCol>
    </VRow>
    <VRow v-if="motivationSetting" class="my-2">
      <VCol cols="12" md="12">
        <strong>ویرایش پیام انگیزشی صفحه اول</strong>
      </VCol>
      <VCol v-if="motivationSetting.jsonValue" cols="12" md="6">
        <Uploader ref="imageUrl"
                  @getFiles="setFile" label="فایل مربوطه"
                  :file-type="UploaderTypes.Motivation"></Uploader>
      </VCol>
      <VCol v-if="motivationSetting.jsonValue" cols="12" md="6">
        <VTextField
          v-model="motivationSetting.jsonValue.title"
          variant="outlined"
          density="compact"
          label="متن نمایشی "
          color="success"
          dir="auto"
          hide-details
        />
      </VCol>
      <VCol cols="12" md="3">
        <VBtn
          @click="setMotivationSetting"
          color="success"
        >
          بروزرسانی
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
