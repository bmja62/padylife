<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import type {VDataTable} from 'vuetify/components'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'



// LifeCycles
onMounted(() => {
  getFirstViews()
})

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const phoneNumbersList = ref<null>(null)
const totalCount = ref<null | string | number | undefined>(null)

const phoneNumbersListFilters = ref<IgetFirstViewsFilters>({
  pageNumber: 1,
  count: 10,
  searchByTitle: null,
})

const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'شماره تلفن', key: 'phoneNumber'},
]


async function getFirstViews() {
  try {
    spinner.showSpinner()

    const response = await $api?.firstViews.getAllFirstViewDiscounts(phoneNumbersListFilters.value)
    phoneNumbersList.value = response?.data.data as Array<ItempMember>
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
      شماره تلفن هایی که شامل تخفیف اول شده اند
    </template>
    <CustomTable
      :items-list="phoneNumbersList"
      :count="phoneNumbersListFilters.count"
      :page-number="phoneNumbersListFilters.pageNumber"
      :table-headers="tableHeaders"
      :total-count="totalCount"
    >
<!--      <template #actions="data">-->

<!--        <VBtn-->
<!--          color="transparent"-->
<!--          elevation="0"-->
<!--          icon="mdi-delete"-->
<!--          @click="renderDeleteDialog(data.item as ItempMember)"-->
<!--        />-->
<!--      </template>-->

    </CustomTable>
  </PageWrapper>
</template>
