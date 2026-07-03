<script setup lang="ts">
import { ref } from "vue";
import { useSpinner } from "@/composables/spinner";
import type { IApiProvider } from "@/models/IApiProvider";
import LongDatePicker from "@/components/Order/LongDatePicker.vue";
import VueApexCharts from "vue3-apexcharts";
import { IDashboardPeriodicStatistics } from "@/services/DietService";
import { IVisitEntity } from "@/services/VisitService";
import CrmSalesAreaCharts from "@/views/dashboards/crm/CrmSalesAreaCharts.vue";
import CrmSessionsBarWithGapCharts from "@/views/dashboards/crm/CrmSessionsBarWithGapCharts.vue";
import { useUtils } from "@/composables/useUtils";

const spinner = useSpinner()
const statistics = ref<IDashboardPeriodicStatistics>(null)
const visitsData = ref(null)
const $api = inject<IApiProvider>('$api')
const mockedFilters = ref({
  name: '30 روز پیش',
  fromDate: useUtils().adjustDate(new Date(Date.now()), 30, 'subtract'),
  toDate: new Date(Date.now()).toISOString(),
  days: 30
})
const generalFilters = ref({
  fromDate: useUtils().adjustDate(new Date(Date.now()), 30, 'subtract'),
  toDate: new Date(Date.now()).toISOString(),
  days: 30,
})
const dashboardStatisticsDemoCards = ref([
  {
    icon: 'tabler-user',
    color: 'info',
    title: 'کاربران فعال',
    key: 'totalActiveUsers',
    subTitle: '',
    stat: '',
  },
  {
    icon: 'tabler-user-check',
    color: 'info',
    title: 'متخصصان',
    key: 'totalExperts',
    subTitle: '',
    stat: '',
  },

])
const visitsStatisticsDemoCards = ref([
  {
    icon: 'tabler-chart-arrows-vertical',
    color: 'info',
    title: 'میانگین بازدید روزانه',
    key: 'avgVisitsPerDay',
    subTitle: '',
    stat: '',
  },
  {
    icon: 'tabler-hand-finger',
    color: 'info',
    title: 'بازدیدکنندگان',
    key: 'uniqueVisits',
    subTitle: '',
    stat: '',
  },
  {
    icon: 'tabler-click',
    color: 'info',
    title: 'بازدیدها',
    key: 'totalVisits',
    subTitle: '',
    stat: '',
  },
  {
    icon: 'tabler-tag-starred',
    color: 'info',
    title: 'پربازدید ترین صفحه',
    key: 'mostPopularPage',
    subTitle: '',
    stat: '',
  },

])
const dashboardSummary = ref(null)
const selectedStatTime = ref('daily')
const selectedOrderStatTime = ref('daily')
const areaChartOptions = ref({
  series: [{
    name: 'کل بازدید',
    data: []
  }, {
    name: 'بازدید‌های یکتا',
    data: []
  }],
  chart: {
    height: 350,
    type: 'area'
  },
  colors: ['#22C55E', '#FF5B60'],
  dataLabels: {
    enabled: false
  },
  stroke: {
    curve: 'smooth'
  },
  xaxis: {
    type: 'datetime',
    tickAmount: 30,
    categories: [],
    labels: {
      formatter(val) {
        return new Date(val).toLocaleDateString('fa-IR')
      }
    }
  },
  tooltip: {
    x: {
      format: 'dd/MM/yy HH:mm'
    },
  },
})
const areaChartSeries = ref([
  {
    name: 'کل بازدید',
    data: []
  }, {
    name: 'بازدید‌های یکتا',
    data: []
  }
])

const computedSalesValue = computed(() => {
  if (dashboardSummary.value) {
    return dashboardSummary?.value[`${selectedStatTime.value}Sales`]
  }
})
const computedOrdersCount = computed(() => {
  if (dashboardSummary.value) {
    console.log(dashboardSummary?.value[`${selectedOrderStatTime.value}Orders`])
    return dashboardSummary?.value[`${selectedOrderStatTime.value}Orders`]
  }
})

async function getVisitsSummary() {
  try {
    spinner.showSpinner()
    const response = await $api?.visit.getVisitsSummary(generalFilters.value)
    statistics.value = response?.data.data
    visitsStatisticsDemoCards.value.forEach((item) => {
      item.stat = statistics.value[item.key]
    })
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getDashboardSummary() {
  try {
    spinner.showSpinner()
    const response = await $api?.visit.getDashboardSummary(generalFilters.value)
    dashboardSummary.value = response?.data.data
    dashboardStatisticsDemoCards.value.forEach((item) => {
      item.stat = dashboardSummary.value[item.key]
    })
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function getVisitsChart() {
  try {
    spinner.showSpinner()
    const response = await $api?.visit.getVisitsChart(generalFilters.value)
    visitsData.value = response?.data.data
    generateChartPayload()
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function generateChartPayload() {
  if (visitsData.value) {
    areaChartSeries.value = [
      {
        name: 'کل بازدید',
        data: []
      }, {
        name: 'بازدید‌های یکتا',
        data: []
      },
    ]
    areaChartOptions.value.xaxis.categories = []
    visitsData.value.forEach((visit: IVisitEntity) => {
      areaChartSeries.value[0].data.push(visit.visits)
      areaChartSeries.value[1].data.push(visit.uniqueVisits)
      areaChartOptions.value.xaxis.categories.push(visit.date)
    })
    areaChartOptions.value.xaxis.tickAmount = visitsData.value.length
  }
}

async function setFilters() {
  generalFilters.value.fromDate = new Date(mockedFilters.value.fromDate).toISOString().split('T')[0]
  generalFilters.value.toDate = new Date(mockedFilters.value.toDate).toISOString().split('T')[0]
  generalFilters.value.days = mockedFilters.value.days
  await fetchPage()
}

async function fetchPage() {
  await Promise.all([
    getVisitsSummary(),
    getVisitsChart(),
    getDashboardSummary()
  ])
}

onMounted(async () => {
  await fetchPage()
})
</script>


<template>
  <PageWrapper has-filters @submitFilters="setFilters">
    <template #title>
      داشبورد
    </template>
    <template #filters>
      <VCol cols="12" md="3">
        <LongDatePicker :required="false" return-object v-model="mockedFilters" label="انتخاب بازه گزارش">
        </LongDatePicker>
      </VCol>

    </template>
    <VRow class="match-height">
      <VCol cols="12">
        <h2>آمار کلی سیستم</h2>
      </VCol>
      <VCol cols="12" md="4" sm="6" lg="3">
        <CrmSalesAreaCharts v-model="selectedStatTime" :count="computedSalesValue" />
      </VCol>
      <VCol cols="12" md="4" sm="6" lg="3">
        <CrmSessionsBarWithGapCharts v-model="selectedOrderStatTime" :amount="computedOrdersCount" />
      </VCol>
      <VCol v-for="demo in dashboardStatisticsDemoCards" :key="demo.title" cols="12" sm="6" md="4" lg="3">
        <VCard>
          <VCardTitle>
            <h5 class="text-h5 mt-4">
              {{ demo.title }}
            </h5>
          </VCardTitle>
          <VCardText>
            <VIcon :icon="demo.icon" size="30"></VIcon>
            <div class="d-flex align-center justify-space-between mt-16">
              <h4 dir="ltr" class="text-h4 mt-5">
                {{ demo.stat }}
              </h4>
            </div>
          </VCardText>
        </VCard>
      </VCol>
      <VCol cols="12">
        <h2>گزارش بازدید صفحات</h2>
      </VCol>
      <VCol cols="12">
        <VueApexCharts v-if="areaChartOptions.xaxis.categories.length" :key="areaChartOptions.xaxis.categories"
          :options="areaChartOptions" :series="areaChartSeries" :height="600" />
        <!--        <EmptyChartView v-else icon="tabler-chart-cohort"></EmptyChartView>-->

      </VCol>
      <VCol v-for="demo in visitsStatisticsDemoCards" :key="demo.title" cols="12" sm="6" md="4" lg="3">
        <VCard>
          <VCardTitle>
            <h5 class="text-h5 mt-4">
              {{ demo.title }}
            </h5>
          </VCardTitle>
          <VCardText>
            <VIcon :icon="demo.icon" size="30"></VIcon>
            <div style="overflow-wrap: anywhere;" class="d-flex align-center justify-space-between mt-16">
              <h4 dir="ltr" class="text-h4 mt-5" v-if="demo.stat">
                {{ typeof demo.stat === 'object' ? demo.stat.pageUrl : demo.stat }}
              </h4>
            </div>
          </VCardText>
        </VCard>
      </VCol>

    </VRow>
  </PageWrapper>

</template>
