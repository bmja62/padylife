<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IGlobalGridRequest} from "@/models/IGlobalGridRequest";
import {IEntity} from "@/services/CityService";
import CreateCityDialog from "@/components/Cities/CreateCityDialog.vue";
import DeleteCityDialog from "@/components/Cities/DeleteCityDialog.vue";
import UpdateCityDialog from "@/components/Cities/UpdateCityDialog.vue";

// LifeCycles
onMounted(() => {
  getEntity()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingCreateDialog = ref<boolean>(false)
const isRenderingUpdateDialog = ref<boolean>(false)
const isRenderingDeleteDialog = ref<boolean>(false)
const entitiesList = ref<IEntity[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedEntity = ref<IEntity>()

const entityFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام انگلیسی شهر', key: 'cityName'},
  {title: 'نام فارسی شهر', key: 'cityNameFa'},
  {title: 'وضعیت', key: 'isActive'},
  {title: 'عملیات', key: 'actions'},
]

// Functions
function renderCreateDialog() {
  isRenderingCreateDialog.value = true
}

function renderUpdateDialog(item: IEntity) {
  selectedEntity.value = JSON.parse(JSON.stringify(item))
  isRenderingUpdateDialog.value = true
}

function renderDeleteDialog(item: IEntity) {
  selectedEntity.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  entityFilters.value.pageNumber = +pageNumber
  await getEntity()
}

async function getEntity() {
  try {
    spinner.showSpinner()
    const response = await $api?.cities.getAll(entityFilters.value)
    entitiesList.value = response?.data.data.data as Array<IEntity>
    totalCount.value = response?.data.data.totalCount as number
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
    @submit-filters="getEntity"
  >
    <template #title>
      لیست شهر‌ها
    </template>
    <template #append>
      <VBtn
        color="success"
        @click="renderCreateDialog"
      >
        ایجاد شهر جدید
      </VBtn>
    </template>
    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="entityFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getEntity"
        />
      </VCol>
    </template>
    <CustomTable
      :items-list="entitiesList"
      :count="entityFilters.count"
      :page-number="entityFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #isActive="data">
        <VChip
          v-if="data.item.isActive"
          tonal
          color="success"
        >
          فعال
        </VChip>
        <VChip
          v-else
          tonal
          color="error"
        >
          غیر فعال
        </VChip>
      </template>
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          @click="renderUpdateDialog(data.item as IEntity)"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as IEntity)"
        />
      </template>
    </CustomTable>

    <UpdateCityDialog
      v-model:dialogState="isRenderingUpdateDialog"
      :selected-item="selectedEntity"
      @refetch="getEntity"
    />

    <DeleteCityDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedEntity"
      @refetch="getEntity"
    />

    <CreateCityDialog
      v-model:dialogState="isRenderingCreateDialog"
      @refetch="getEntity"
    />
  </PageWrapper>
</template>
