<script setup lang="ts">
import type { ITableHeaders } from '@/models/ITableHeader'
import { inject } from 'vue'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import { PlanStatus, type IPlan, type IUserPlansListFilters } from '@/services/PlanService'

onMounted(() => {
    fetchUserPlans()
})

const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const route = useRoute()

const userPlans = ref<IPlan[] | null>([])
const totalCount = ref<number | string | null>(null)

const filters = ref<IUserPlansListFilters>({
    pageNumber: 1,
    count: 10,
    search: '',
    userId: +(route.params.id as string),
})

const tableHeaders: ITableHeaders = [
    { title: 'شناسه', key: 'id' },
    { title: 'عنوان پلن', key: 'title' },
    { title: 'دسته بندی', key: 'planCategoryName' },
    { title: 'وضعیت', key: 'status' },
    { title: 'جزئیات', key: 'actions' },
]

async function changePage(pageNumber: number | string) {
    filters.value.pageNumber = +pageNumber
    await fetchUserPlans()
}

async function fetchUserPlans() {
    try {
        spinner.showSpinner()
        const response = await $api?.plan.getUserPlans(filters.value)
        userPlans.value = response?.data.data.data as unknown as IPlan[]
        totalCount.value = response?.data.data.totalCount as number
    } catch (error) {
        console.error(error)
    } finally {
        spinner.hideSpinner()
    }
}
</script>

<template>
    <PageWrapper has-filters @submit-filters="fetchUserPlans">
        <template #title>
            پلن های کاربر
        </template>

        <template #filters>
            <VCol cols="12" md="3">
                <VTextField v-model="filters.search" label="عنوان پلن" hide-details @keydown.enter="fetchUserPlans" />
            </VCol>
        </template>

        <CustomTable :items-list="userPlans" :count="filters.count" :page-number="filters.pageNumber"
            :table-headers="tableHeaders" :total-count="totalCount" @change-page="changePage">
            <template #status="data">
                <VChip v-if="data.item.status === 'Active'" color="success">فعال</VChip>   
                <VChip v-else color="error">غیرفعال</VChip>   
            </template>
            <template #actions="data">
                <VBtn color="transparent" elevation="0" :to="`/Users/${filters.userId}/plans/${data.item.id}/detail`">
                    <VIcon icon="mdi-chart-line"></VIcon>
                    <VTooltip activator="parent">گزارش تعهد به برنامه</VTooltip>
                </VBtn>
            </template>
        </CustomTable>
    </PageWrapper>
</template>
