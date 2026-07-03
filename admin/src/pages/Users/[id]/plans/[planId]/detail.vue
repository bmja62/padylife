<script setup lang="ts">
import { inject } from 'vue'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import type { IWeeklyCommitmentReportRequest, IPlanAnswersRequestParams } from '@/services/PlanService'
import VueApexCharts from 'vue3-apexcharts'

const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const route = useRoute()
const selectedUserId = computed(() => +(route.params.id as string))

const renderCommitmentChart = ref(false)
const weeklyCommitmentSeries = ref([
    { name: 'سوالات پاسخ داده شده', data: [] as number[] },
    { name: 'تمرینات انجام شده', data: [] as number[] },
    { name: 'تمرینات درحال انجام', data: [] as number[] },
])
const weeklyCommitmentChartOptions = ref({
    series: [
        { name: 'سوالات پاسخ داده شده', data: [] as number[] },
        { name: 'تمرینات انجام شده', data: [] as number[] },
        { name: 'تمرینات درحال انجام', data: [] as number[] },
    ],
    chart: {
        height: 350,
        type: 'line',
        zoom: {
            enabled: false
        }
    },
    colors: ['#22c55e', '#14b8a6', '#ef4444'],
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'straight'
    },
    title: {
        text: 'Product Trends by Month',
        align: 'left'
    },
    grid: {
        row: {
            colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
            opacity: 1
        },
    },
    xaxis: {
        categories: [],
    }
})

async function fetchWeeklyCommitmentReport() {
    try {
        spinner.showSpinner()
        const payload: IWeeklyCommitmentReportRequest = {
            userId: +(route.params.id as string),
            weeks: 7,
        }
        const response = await $api?.plan.getWeeklyCommitmentReport(payload)
        const report: any = response?.data?.data
        weeklyCommitmentChartOptions.value.xaxis.categories = []
        weeklyCommitmentSeries.value[0].data = []
        weeklyCommitmentSeries.value[1].data = []
        weeklyCommitmentSeries.value[2].data = []

        if (report?.weeklyData?.length) {
            report.weeklyData.forEach((week: any) => {
                weeklyCommitmentChartOptions.value.xaxis.categories.push(week.weekLabel)
                weeklyCommitmentSeries.value[0].data.push(week.answersCount)
                weeklyCommitmentSeries.value[1].data.push(week.exercisesCompleted)
                weeklyCommitmentSeries.value[2].data.push(week.exercisesAssigned)
            })
            renderCommitmentChart.value = true
        } else {
            // Fallback: generate categories based on dummy series length
            // const maxLen = Math.max(
            //     ...(weeklyCommitmentSeries.value?.map(s => s.data.length) || [0])
            // )
            // if (maxLen > 0) {
            //     weeklyCommitmentChartOptions.value.xaxis.categories = Array.from({ length: maxLen }, (_, i) => `هفته ${i + 1}`)
            //     renderCommitmentChart.value = true
            // } else {
            //     renderCommitmentChart.value = false
            // }
        }


    } catch (error) {
        console.error(error)
    } finally {
        spinner.hideSpinner()
    }
}

onMounted(() => {
    fetchWeeklyCommitmentReport()
    fetchPlanAnswers()
    fetchPlanExcersises()
})

// --- Plan Answers Section ---
const answersFilters = ref<IPlanAnswersRequestParams>({
    planId: +(route.params.planId as string),
    onlyCompleted: false,
    search: '',
    pageNumber: 1,
    count: 10,
})
const answersItems = ref<any[] | null>(null)
const excersiesItems = ref<any[] | null>(null)

async function fetchPlanAnswers() {
    try {
        spinner.showSpinner()
        if (!$api) return
        const response: any = await $api.plan.getPlanAnswersRequest(answersFilters.value)
        const payload = response?.data?.data?.data
        let items: any[] = []
        if (Array.isArray(payload)) {
            items = payload
        } else if (payload) {
            items = [payload]
        }
        // Filter by selected user id
        answersItems.value = items.filter(i => i?.userId === selectedUserId.value)
    } catch (error) {
        console.error(error)
    } finally {
        spinner.hideSpinner()
    }
}
async function fetchPlanExcersises() {
    try {
        spinner.showSpinner()
        if (!$api) return
        const response: any = await $api.plan.getPlanExcersisesRequest(answersFilters.value)
        const raw = response?.data?.data?.data
        const items = Array.isArray(raw) ? raw : raw ? [raw] : []
        excersiesItems.value = items.filter((i: any) => i?.userId === selectedUserId.value)

        console.log(excersiesItems.value)
    } catch (error) {
        console.error(error)
    } finally {
        spinner.hideSpinner()
    }
}

function applyFilters() {
    fetchPlanAnswers()
    fetchPlanExcersises()
}
</script>

<template>
    <PageWrapper>
        <template #title>
            گزارش تعهد به برنامه
        </template>

        <VCard>
            <VCardText>
                <VueApexCharts v-if="weeklyCommitmentChartOptions.xaxis.categories.length"
                    :options="weeklyCommitmentChartOptions" :series="weeklyCommitmentSeries" height="500" />
                <div v-else>
                    داده ای برای نمایش وجود ندارد
                </div>
            </VCardText>
        </VCard>

        <VCard class="mt-6" variant="flat">
            <VCardTitle>
                <h2>

                    پاسخ های کاربر به سوالات
                </h2>
            </VCardTitle>
            <VCardText>
                <VRow>
                    <VCol cols="12" md="4">
                        <VTextField v-model="answersFilters.search" label="جستجو" hide-details
                            @keydown.enter="fetchPlanAnswers" />
                    </VCol>
                    <VCol cols="12" md="4">
                        <CustomSwitch v-model="answersFilters.onlyCompleted" label="فقط تکمیل شده"
                            id="customizer-menu-collapsed" :has-tooltip="false" class="ms-2"
                            @change="fetchPlanAnswers" />
                    </VCol>
                    <VCol cols="12" md="4" class="d-flex align-center">
                        <VBtn color="primary" @click="fetchPlanAnswers">اعمال فیلتر</VBtn>
                    </VCol>
                </VRow>

                <VList v-if="answersItems?.length">
                    <VListItem v-for="(item, idx) in answersItems" :key="idx" class="mb-4">
                        <div class="d-flex align-start justify-space-between w-100">
                            <div class="d-flex align-center gap-3">
                                <VAvatar size="36" color="primary" variant="tonal">
                                    <VIcon icon="mdi-account" size="22"></VIcon>
                                </VAvatar>
                                <div>
                                    <div class="text-subtitle-1 font-weight-bold">{{ item.userFullName }}</div>
                                    <div class="text-caption d-flex align-center gap-2">
                                        <VIcon size="16" icon="mdi-calendar-clock"></VIcon>
                                        <span><b>شروع:</b> {{ new Date(item.startDate).toLocaleDateString('fa-IR')
                                            }}</span>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <VChip :color="item.isCompleted ? 'success' : 'warning'" variant="flat"
                                    class="text-caption">
                                    <VIcon start :icon="item.isCompleted ? 'mdi-check-circle' : 'mdi-progress-clock'">
                                    </VIcon>
                                    {{ item.isCompleted ? 'تکمیل شده' : 'در حال انجام' }}
                                </VChip>
                            </div>
                        </div>
                        <VDivider class="my-3" />
                        <div>
                            <div v-for="(ans, aIdx) in item.answers" :key="aIdx" class="mb-3">
                                <div class="d-flex align-center gap-2">
                                    <VIcon icon="mdi-help-circle-outline" size="18"></VIcon>
                                    <span class="text-body-2"><b>سوال:</b>
                                        <div v-html="ans.questionText"></div>
                                    </span>
                                </div>
                                <div class="mt-1 d-flex align-center gap-2">
                                    <VIcon icon="mdi-checkbox-marked-circle-outline" size="18"></VIcon>
                                    <span class="text-caption">پاسخ:</span>
                                    <VChip color="info" variant="tonal" size="small">{{ ans.selectedOptionText }}
                                    </VChip>
                                </div>
                            </div>
                        </div>
                    </VListItem>
                </VList>
                <div v-else class="text-caption">نتیجه ای یافت نشد</div>
            </VCardText>
        </VCard>
        <VCard class="mt-6" variant="flat">
            <VCardTitle>
                <h2>تمرین‌ها و پاسخ‌ها</h2>
            </VCardTitle>
            <VCardText>
                <VList v-if="excersiesItems?.length">
                    <VListItem v-for="(item, idx) in excersiesItems" :key="idx" class="mb-4">
                        <div class="d-flex align-start justify-space-between w-100">
                            <div class="d-flex align-center gap-3">
                                <VAvatar size="36" color="primary" variant="tonal">
                                    <VIcon icon="mdi-account" size="22"></VIcon>
                                </VAvatar>
                                <div>
                                    <div class="text-subtitle-1 font-weight-bold">{{ item.userFullName }}</div>
                                    <div class="text-caption d-flex align-center gap-3 flex-wrap">
                                        <span class="d-flex align-center gap-1">
                                            <VIcon size="16" icon="mdi-calendar-start"></VIcon>
                                            <b>شروع:</b> {{ new Date(item.startDate).toLocaleDateString('fa-IR') }}
                                        </span>
                                        <span class="d-flex align-center gap-1">
                                            <VIcon size="16" icon="mdi-calendar-end"></VIcon>
                                            <b>پایان:</b> {{ new Date(item.endDate).toLocaleDateString('fa-IR') }}
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="d-flex align-center gap-2">
                                <VChip :color="item.isSignUpPlan ? 'info' : 'secondary'" variant="tonal" size="small">
                                    <VIcon start icon="mdi-account-plus"></VIcon>
                                    پلن ثبت نام
                                </VChip>
                                <VChip :color="item.isCompleted ? 'success' : 'warning'" variant="flat"
                                    class="text-caption">
                                    <VIcon start :icon="item.isCompleted ? 'mdi-check-circle' : 'mdi-progress-clock'">
                                    </VIcon>
                                    {{ item.isCompleted ? 'تکمیل شده' : 'در حال انجام' }}
                                </VChip>
                            </div>
                        </div>
                        <VDivider class="my-3" />

                        <div>
                            <VCard v-for="(ex, eIdx) in item.exerciseAnswers" :key="eIdx" class="mb-3"
                                variant="outlined">
                                <VCardText>
                                    <div class="d-flex align-start justify-space-between w-100">
                                        <div class="d-flex align-center gap-2">
                                            <VIcon icon="mdi-dumbbell" size="18"></VIcon>
                                            <span class="text-body-2"><b>تمرین:</b> {{ ex.exerciseName }}</span>
                                        </div>
                                        <div class="d-flex align-center gap-2">
                                            <VChip color="primary" variant="tonal" size="small">
                                                <VIcon start icon="mdi-stairs-up"></VIcon>
                                                {{ ex.stepName }}
                                            </VChip>
                                        </div>
                                    </div>

                                    <div class="mt-2 d-flex align-center gap-2">
                                        <VIcon icon="mdi-checkbox-marked-circle-outline" size="18"></VIcon>
                                        <span class="text-caption">گزینه انتخابی:</span>
                                        <VChip color="info" variant="tonal" size="small">{{ ex.selectedOptionText }}
                                        </VChip>
                                    </div>

                                    <div v-if="ex.textAnswer" class="mt-2">
                                        <span class="text-caption"><b>پاسخ متنی:</b></span>
                                        <div class="text-body-2">{{ ex.textAnswer }}</div>
                                    </div>

                                    <div v-if="ex.imageUrl" class="mt-2">
                                        <span class="text-caption"><b>تصویر:</b></span>
                                        <VImg :src="ex.imageUrl" max-height="160" class="mt-1 rounded" cover></VImg>
                                    </div>

                                    <div v-if="ex.selectedChoices?.length" class="mt-3">
                                        <div class="text-caption mb-2"><b>انتخاب‌ها:</b></div>
                                        <div class="d-flex flex-wrap gap-2">
                                            <VChip v-for="(ch, cIdx) in ex.selectedChoices" :key="cIdx"
                                                :color="ch.isCorrect ? 'success' : 'error'" variant="tonal"
                                                size="small">
                                                <VIcon start :icon="ch.isCorrect ? 'mdi-check' : 'mdi-close'" />
                                                {{ ch.choiceText }}
                                            </VChip>
                                        </div>
                                        <template v-for="(choice, cIdx) in ex.selectedChoices" :key="'fb-' + cIdx">
                                            <div v-if="choice.feedback" class="mt-1 d-flex align-center gap-2">
                                                <VIcon icon="mdi-information-outline" size="16"></VIcon>
                                                <span class="text-caption">بازخورد: {{ choice.feedback }}</span>
                                            </div>
                                        </template>
                                    </div>
                                </VCardText>
                            </VCard>
                        </div>
                    </VListItem>
                </VList>
                <div v-else class="text-caption">تمرینی یافت نشد</div>
            </VCardText>
        </VCard>
    </PageWrapper>
</template>
