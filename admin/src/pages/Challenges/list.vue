<script setup lang="ts">
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IAttribute} from "@/services/ProductAttributes";
import {challengeTypesShow, IChallenge, IGetChallangeFilters} from "@/services/ChallengeService";
import {useAuthStore} from "@/stores/auth";

// LifeCycles
onMounted(() => {
  getAllChallenges()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const isRenderingDeleteDialog = ref<boolean>(false)
const challengesList = ref<IChallenge[]>(null)
const totalCount = ref<null | string | number | undefined>(null)
const selectedChallenge = ref<IChallenge>()
const authStore = useAuthStore()
const challengesFilters = ref<IGetChallangeFilters>({
  pageNumber: 1,
  count: 10,
  search: '',
  type: null,
  allUsers:authStore.getUser.roles.filter(e=> e.name === 'Admin').length ? true : false
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'نام چالش', key: 'title'},
  {title: 'نوع چالش', key: 'type', value: (item: IChallenge) => challengeTypesShow[item.type]},
  {title: 'عملیات', key: 'actions'},
]

// Functions

function renderDeleteDialog(item: IAttribute) {
  selectedChallenge.value = JSON.parse(JSON.stringify(item))
  isRenderingDeleteDialog.value = true
}

async function changePage(pageNumber: number | string) {
  challengesFilters.value.pageNumber = +pageNumber
  await getAllChallenges()
}

async function getAllChallenges() {
  try {
    spinner.showSpinner()
    const response = await $api?.challenge.getAllChallengesByFilter(challengesFilters.value)
    challengesList.value = response?.data.data.data as Array<IAttribute>
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
    @submit-filters="getAllChallenges"
  >
    <template #title>
      لیست چالش‌ها
    </template>

    <template #filters>
      <VCol
        cols="12"
        md="3"
      >
        <VTextField
          v-model="challengesFilters.search"
          label="جستجو... "
          hide-details
          @keydown.enter="getAllChallenges"
        />
      </VCol>
      <VCol
        cols="12"
        md="3"
      >
        <ChallengeTypePicker required v-model="challengesFilters.type"></ChallengeTypePicker>
      </VCol>
    </template>
    <CustomTable
      :items-list="challengesList"
      :count="challengesFilters.count"
      :page-number="challengesFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
      @change-page="changePage"
    >
      <template #actions="data">
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-pencil"
          :to="`/Challenges/${data.item.id}`"
        />
        <VBtn
          color="transparent"
          elevation="0"
          icon="mdi-delete"
          @click="renderDeleteDialog(data.item as IAttribute)"
        />
      </template>
    </CustomTable>


    <DeleteChallengeDialog
      v-model:dialogState="isRenderingDeleteDialog"
      :selected-item="selectedChallenge"
      @refetch="getAllChallenges"
    />

  </PageWrapper>
</template>
