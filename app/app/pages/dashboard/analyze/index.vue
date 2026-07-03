<script setup lang="ts">
import type {
  ICoursesProgressReport,
  IFeelingReport,
  IPlanTeamChartsReport,
  IWeeklyCommitment,
  IWeeklyCommitmentReport,
  IWeeklyReportResult
} from "~/services/ReportService";
import {dailyFeelingsColorMap} from "~/models/Enums/DailyFeelings";

definePageMeta({
  layout: "dashboard",
  auth: true
});
useHead({
  title:'آنالیز وضعیت'
})
const authStore = useAuthStore()
const {$api} = useNuxtApp()
const weeklyFeelingReport = ref<IWeeklyReportResult<IFeelingReport[]>>(null)
const weeklyCommitmentReport = ref<IWeeklyCommitmentReport>(null)
const coursesProgressReport = ref<ICoursesProgressReport>(null)
const planTeamCharts = ref<IPlanTeamChartsReport[]>(null)
const renderCommitmentChart = ref(false)
const weeklyFeelingChartOptions = ref({
  series: [
    {
      data: [],
    },
  ],
  chart: {
    fontFamily: "Peyda",
    fontSize: "10px",
    height: 250,
    type: "bar",
    toolbar: {
      show: false,
    },
  },
  colors: [],
  plotOptions: {
    bar: {
      borderRadius: 15,
      borderRadiusApplication: "end",
      columnWidth: "70%",
      distributed: true,
      dataLabels: {
        position: "top",
      },
    },
  },
  dataLabels: {
    offsetY: 2,
    enabled: false,
    style: {
      fontSize: "20px",
    },

  },
  legend: {
    show: false,
  },
  xaxis: {
    categories: [].reverse(),
    labels: {
      style: {
        fontSize: "13px",
      },
    },
  },
  yaxis: {
    max: 40,
    show: false,
  },
  grid: {
    show: false,
  },
})
const weeklyCommitmentChartOptions = ref({
  series: [
    {
      name: "سوالات پاسخ داده شده",
      data: [],
    },
    {
      name: "تمرینات انجام شده",
      data: [],
    },
    {
      name: "تمرینات درحال انجام",
      data: [],
    },
  ],
  colors: ["#00ABFB", "#FFE138", "#9F41DC"],
  chart: {
    fontFamily: "Peyda",
    height: 200,
    type: "line",
    toolbar: {
      show: false,
    },
    zoom: {
      enabled: false,
    },
  },
  dataLabels: {
    enabled: false,
  },
  stroke: {
    curve: "smooth",
    width: 2,
  },
  xaxis: {
    categories: [].reverse(),
    axisTicks: {
      show: false,
    },
    labels: {
      style: {
        fontSize: "14px",
        fontWeight: "bold",
      },
    },
  },
  yaxis: {
    tickAmount: 0,
    max: 20,
    show: false,
  },
  grid: {
    show: false, // Hide the grid completel
    xaxis: {
      lines: {
        show: false, // Hide x-axis lines
      },
    },
    yaxis: {
      lines: {
        show: false, // Hide y-axis lines
      },
    },
  },
  legend: {
    show: true,
    horizontalAlign: "right",
    markers: {
      shape: "square",
    },
  }
})

async function getWeeklyFeelingsReport() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getWeeklyFeelingReport({
      userId: authStore.getUser.id,
      weeks: 7
    })
    weeklyFeelingReport.value = response.data
    await generateWeeklyFeelingChart()
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function generateWeeklyFeelingChart() {
  if (weeklyFeelingReport?.value?.weeklyData?.length) {
    weeklyFeelingReport?.value?.weeklyData.forEach((week: IFeelingReport) => {
      weeklyFeelingChartOptions.value.xaxis.categories.push(week.weekLabel)
      weeklyFeelingChartOptions.value.series[0].data.push(week.feelingEntries)
      if (week.averageFeeling) {
        weeklyFeelingChartOptions.value.colors.push(dailyFeelingsColorMap[week.averageFeeling])

      }
    })
  }
}

async function getWeeklyCommitmentReport() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getWeeklyCommitmentReport({
      userId: authStore.getUser.id,
      weeks: 7
    })
    weeklyCommitmentReport.value = response.data
    await generateWeeklyCommitmentChart()
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function generateWeeklyCommitmentChart() {

  weeklyCommitmentReport.value.weeklyData.forEach((week: IWeeklyCommitment) => {
    weeklyCommitmentChartOptions.value.xaxis.categories.push(week.weekLabel)
    weeklyCommitmentChartOptions.value.series[0].data.push(week.answersCount)
    weeklyCommitmentChartOptions.value.series[1].data.push(week.exercisesCompleted)
    weeklyCommitmentChartOptions.value.series[2].data.push(week.exercisesAssigned)
  })
  renderCommitmentChart.value = true

}

async function getCourseProgressReport() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getCourseProgressReport(authStore.getUser.id)
    coursesProgressReport.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getPlanTeamChart() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.report.getPlanTeamChart({
      topPlans: 10,
      periods: 7,
      grouping: 'Daily',
    })
    planTeamCharts.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
getWeeklyFeelingsReport()
getWeeklyCommitmentReport()
getCourseProgressReport()
getPlanTeamChart()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper>
      <template #header>
        <BaseNotificationHeader>
          <template #title> آنالیز وضعیت </template>
        </BaseNotificationHeader>
      </template>
      <DashboardAnalyzeChartsContainer>
        <template #title> درصد پیشرفت برنامه‌ها </template>
        <template #chart>
          <div v-if="coursesProgressReport?.plansProgress?.length" class="grid grid-cols-2">
            <div class="col-span-1 flex flex-col gap-1 items-center justify-center"
                 v-for="course in coursesProgressReport?.plansProgress">
              <ChartsRadialChart :color="dailyFeelingsColorMap[Math.ceil(Math.random()*4)]"
                                 :percent="course.progressPercentage"/>
              <p class="text-[12px] text-center -mt-2">{{ course.planTitle }}</p>
            </div>
          </div>
        </template>
      </DashboardAnalyzeChartsContainer>
      <DashboardAnalyzeChartsContainer>
        <template #title> گزارش هفتگی تعهد به برنامه </template>
        <template #chart>
          <ChartsLineChart v-if="renderCommitmentChart" :options="weeklyCommitmentChartOptions" class="w-full"/>
        </template>
      </DashboardAnalyzeChartsContainer>
      <DashboardAnalyzeChartsContainer>
        <template #title> گزارش هفتگی احساسات </template>
        <template #chart>
          <ChartsBarChart v-if="weeklyFeelingReport" :options="weeklyFeelingChartOptions"
                          class="w-full"/>
        </template>
      </DashboardAnalyzeChartsContainer>
      <DashboardAnalyzeChartsContainer :isLastComponent="true" class="">
        <template #title> گزارش هفتگی پلن‌ها</template>
        <template #chart>

          <DashboardAnalyzeProgramReportsProgramDropDown
              v-for="(planChart,idx) in planTeamCharts"
              :key="idx"
              :dataPoints="planChart.dataPoints"
              :icon="planChart.planImageUrl?planChart.planImageUrl : '/common/no-image.png'"
              :title="planChart.planName"
              class="mb-3"
          />
        </template>
      </DashboardAnalyzeChartsContainer>
    </UtilsWrappersPageWrapper>
  </div>
</template>
