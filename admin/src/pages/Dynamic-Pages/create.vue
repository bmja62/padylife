<script lang="ts" setup>
import {isAxiosError} from 'axios'
import {inject, ref} from 'vue'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import FroalaEditor from '@/components/Utilities/FroalaEditor.vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateDynamicPagePayload} from '@/services/DynamicPageService'

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const router = useRouter()

const dynamicPagePayload = ref<ICreateDynamicPagePayload>({
  title: '',
  content: '',
  seoDescription: '',
  seoTitle: '',
  seoUrl: '',
  scriptContent: '',
  metaContent: ''
})
const dynamicPagesValidation = ['title', 'seoUrl', 'content', 'seoTitle']

// Functions
async function createNewDynamicPage() {
  let isValid = true
  dynamicPagesValidation.forEach((validation) => {
    if (!dynamicPagePayload.value[validation]) {
      isValid = false
    }
  })
  if (isValid) {

    try {
      spinner.showSpinner()

      const response = await $api?.dynamicPages.createANewDynamicPage(dynamicPagePayload.value)

      if (response?.data.isSuccess) {
        alert.success('برگه با موفقیت ایجاد شد')
        router.push('/dynamic-pages/list')
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
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل کنید')

  }
}
</script>

<template>
  <PageWrapper>
    <template #title>
      ایجاد صفحه‌ی جدید
    </template>
    <VRow>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPagePayload.title"
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
          v-model.trim="dynamicPagePayload.seoUrl"
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
          v-model.trim="dynamicPagePayload.seoTitle"
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
          v-model.trim="dynamicPagePayload.seoDescription"
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
          v-model="dynamicPagePayload.content"
          editor-placeholder="محتوای برگه"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPagePayload.scriptContent"
          color="success"
          density="compact"
          hide-details="auto"
          label="script content"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="dynamicPagePayload.metaContent"
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
        <VBtn @click="createNewDynamicPage">
          ایجاد برگه
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
