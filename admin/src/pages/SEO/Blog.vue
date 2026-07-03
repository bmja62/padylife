<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload, IGetseoDataFilters} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from "@/composables/alert";

// Interfaces
interface ItempSeoData extends ICreateOrUpdateBrandPayload {
  id: string | number
}

// LifeCycles
onMounted(() => {
  getSeoData()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingUpdateSection = ref<boolean>(false)
const seoDataList = ref<null | ItempSeoData[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempSeoData = ref<ItempSeoData>()
const alert = useAlerts()
const seoDataFilters = ref<IGetseoDataFilters>({
  pageNumber: 1,
  count: 10,
  searchByTitle: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'blogId'},
  {title: 'نام', key: 'title'},
  {title: 'عملیات', key: 'actions'},
]


function renderUpdateDialog(item: ICreateOrUpdateBrandPayload) {
  tempSeoData.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateSection.value = true
}


async function changePage(pageNumber: number | string) {
  seoDataFilters.value.pageNumber = +pageNumber
  await getSeoData()
}

async function getSeoData() {
  try {
    spinner.showSpinner()
    const response = await $api?.blog.getAllBlog(seoDataFilters.value)
    seoDataList.value = response?.data.data as Array<ItempSeoData>
    totalCount.value = response?.data.total as number
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function updateSeoData() {
  try {
    spinner.showSpinner()
    const response = await $api?.blog.updateBlogSEO(tempSeoData.value)
    alert.success('بروزرسانی با موفقیت انجام شد')
    getSeoData()
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <PageWrapper
    has-filters
    @submit-filters="getSeoData"
  >
    <template #title>
      بروزرسانی سئوی اخبار
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="seoDataFilters.searchByTitle"
          label="جستجو..."
          hide-details
          @keydown.enter="getSeoData"
        />
      </VCol>
    </template>
    <VRow>
      <VCol cols="12">
        <CustomTable
          :items-list="seoDataList"
          :count="seoDataFilters.count"
          :page-number="seoDataFilters.pageNumber"
          :table-headers="tableHeaders"
          :total-count="totalCount"
          @change-page="changePage"
        >
          <template #actions="data">
            <VBtn
              color="transparent"
              elevation="0"
              icon="mdi-pencil"
              @click="renderUpdateDialog(data.item as ItempSeoData)"
            />
          </template>
        </CustomTable>
      </VCol>
    </VRow>
    <VRow v-if="isRenderingUpdateSection">
      <VCol cols="6">
        <VTextField
          v-model.trim="tempSeoData.seourl"
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
          v-model.trim="tempSeoData.seoTitle"
          :rules="[(value) => !!value || 'فیلد عنوان سئو اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="عنوان سئو"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12">
        <VTextField
          v-model.trim="tempSeoData.seoDescription"
          color="success"
          density="compact"
          hide-details="auto"
          label="توضیحات سئو"
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol cols="6">
        <VTextField
          v-model.trim="tempSeoData.scriptContent"
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
          v-model.trim="tempSeoData.metaContent"
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
        <VBtn @click="updateSeoData">
          بروزرسانی
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
