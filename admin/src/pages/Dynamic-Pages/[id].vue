<script lang="ts" setup>
import {isAxiosError} from 'axios'
import {inject, onMounted, ref} from 'vue'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import FroalaEditor from '@/components/Utilities/FroalaEditor.vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {IUpdateDynamicPagePayload} from '@/services/DynamicPageService'

// LifeCycles
onMounted(() => {
  // getInsurancePaperList()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()
const defaultTitle = ref<string | null>(null)

const dynamicPageDetails = ref<IUpdateDynamicPagePayload>({
  title: '',
  content: '',
  seoDescription: '',
  seoTitle: '',
  slug: '',
  id: 0,
})

// LifeCycles
onMounted(async () => {
  await getASingleDynamicPage()
})

// Functions
async function getASingleDynamicPage() {
  try {
    spinner.showSpinner()

    const response = await $api?.dynamicPages.getASingleDynamicPage(route.params.id as string)

    dynamicPageDetails.value = response.data
    defaultTitle.value = JSON.parse(JSON.stringify(response.data.title))
  } catch (error) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function updateDynamicPage() {
  try {
    spinner.showSpinner()

    const response = await $api?.dynamicPages.updateADynamicPage(dynamicPageDetails.value)

    if (response?.data.isSuccess) {
      alert.success('برگه با موفقیت ویرایش شد')
      await getASingleDynamicPage()
    } else {
      alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    }
  } catch (error) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper>
    <template #title>
      ویرایش برگه {{ defaultTitle }}
    </template>
    <VRow>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPageDetails.title"
          :rules="[(value) => !!value || 'فیلد عنوان صفحه اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="عنوان صفحه"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPageDetails.seoUrl"
          :rules="[(value) => !!value || 'فیلد آدرس صفحه اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="آدرس صفحه"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPageDetails.seoTitle"
          :rules="[(value) => !!value || 'فیلد عنوان سئو اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="عنوان سئو"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPageDetails.seoDescription"
          :rules="[(value) => !!value || 'فیلد توضیحات سئو اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="توضیحات سئو"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12">
        <FroalaEditor
          v-model="dynamicPageDetails.content"
          editor-placeholder="محتوای برگه"
        />
      </VCol>
      <VCol cols="6">
        <VTextarea
          v-model.trim="dynamicPageDetails.scriptContent"
          color="success"
          density="compact"
          hide-details="auto"
          label="script content"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextarea
          v-model.trim="dynamicPageDetails.metaContent"
          color="success"
          density="compact"
          hide-details="auto"
          label="meta content"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol
        cols="12"
        align="end"
        justify="end"
      >
        <VBtn @click="updateDynamicPage">
          ویرایش برگه
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
