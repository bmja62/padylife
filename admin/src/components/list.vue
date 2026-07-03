<script lang="ts" setup>
import { inject, onMounted, ref } from 'vue'
import type { VDataTable } from 'vuetify/components'
import { useSpinner } from '@/composables/spinner'
import type { IApiProvider } from '@/models/IApiProvider'
import type { ICreateOrUpdatePropertyPayload, IGetProductPropertyFilters } from '@/services/ProductPropertyService'

// LifeCycles
onMounted(() => {
  getProperties()
})

// Interfaces
interface ITempProperty extends ICreateOrUpdatePropertyPayload {
  id: string | number
}

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const propertiesList = ref<null | ITempProperty[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const tempProperty = ref<ITempProperty>()
const router = useRouter()

const propertyFilters = ref<IGetProductPropertyFilters>({
  pageNumber: 1,
  count: 10,
  name: null,
})

const tableHeaders: VDataTable['headers'] = [
  {
    title: 'شناسه',
    key: 'id',
  },
  {
    title: 'نام',
    key: 'name',
  },
  {
    title: 'دسته بندی شاخص است؟',
    key: 'indicator',
  },
  {
    title: 'عملیات',
    key: 'actions',
  },
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: ITempProperty) {
  tempProperty.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: ITempProperty) {
  tempProperty.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function getProperties() {
  try {
    spinner.showSpinner()

    const response = await $api?.properties.getProperties(propertyFilters.value)

    propertiesList.value = response?.data.data.data as Array<ITempProperty>
    totalCount.value = response?.data.data.totalCount as number
  }
  catch (error) {
    console.error(error)
  }
  finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  propertyFilters.value.pageNumber = +pageNumber
  await getProperties()
}
</script>

<template>
  <PageWrapper has-filters>
    <template #title>
      لیست ویژگی ها
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد ویژگی جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="propertyFilters.name"
          hide-details
          label="نام ویژگی"
          @keydown.enter="getProperties"
        />
      </VCol>
    </template>
    <CustomTable
      :count="propertyFilters.count"
      :items-list="propertiesList"
      :page-number="propertyFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #indicator="data">
        <VChip :color="data.item.indicator ? 'success' : 'error'">
          {{ data.item.indicator ? "بله" : "خیر" }}
        </VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item as ITempProperty)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as ITempProperty)"
        />
        <VTooltip text="Tooltip">
          <template #activator="{ props }">
            <VBtn
              class="rounded-circle"
              color="transparent"
              elevation="0"
              icon="mdi-arrange-bring-to-front"
              @click="router.push(`/property-values/${data.item.id}`)"
            >
              <VIcon icon="mdi-arrange-bring-to-front" />
              <VTooltip
                activator="parent"
                location="top"
              >
                <span>مشاهده مقادیر این ویژگی</span>
              </VTooltip>
            </VBtn>
          </template>
        </VTooltip>
      </template>
    </CustomTable>

    <CreatePropertyDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getProperties"
    />

    <UpdatePropertyDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :default-property="tempProperty"
      @refetch="getProperties"
    />

    <DeletePropertyDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :default-property="tempProperty"
      @refetch="getProperties"
    />
  </PageWrapper>
</template>
