<script setup lang="ts">

import type {IApiProvider} from "~/models/IApiProvider";
import {DynamicSettings, type IDynamicSetting} from "~/services/SiteDynamicSetting";

const {$api} = useNuxtApp<IApiProvider>()
const teamPerformance = ref(null)
const motivationSetting = ref<IDynamicSetting>(null)
const spinner = useSpinner()
const renderChart = ref(false)
const earthChartOptions = ref({
  series: [],
  chart: {
    fontFamily: "Peyda",
    height: 300,
    type: "radialBar",
  },
  stroke: {
    lineCap: "round",
  },
  plotOptions: {
    radialBar: {
      offsetY: 0,
      startAngle: 0,
      endAngle: 270,
      hollow: {
        max: 20, margin: 5,
        size: "60%",
        background: "transparent",
        image: "/common/planet-earth.webp",
        imageClipped: false,
        imageWidth: 200,
        imageHeight: 200,
      },
      track: {
        show: false,
      },
      dataLabels: {
        style: {
          colors: ["#000", "#E91E63", "#9C27B0"],
        },
        name: {
          show: false,
        },
        value: {
          show: false,
        },
      },
      barLabels: {
        enabled: true,
        useSeriesColors: false,
        offsetX: -20,
        fontSize: "12px",
        formatter: function (
            seriesName: string,
            opts: {
              w: { globals: { series: { [x: string]: string } } };
              seriesIndex: string | number;
            }
        ) {
          return seriesName + ":  " + opts.w.globals.series[opts.seriesIndex];
        },
      },
    },
  },
  colors: ["#FFE138", "#E91E63", "#9C27B0"],
  labels: [],
  responsive: [
    {
      breakpoint: 480,
      options: {
        legend: {
          show: false,
        },
      },
    },
  ],
})

async function getUserActivityForAllQuestions() {
  try {
    const response = await $api.report.getUserActivityForAllQuestions()
    if (response.data) {
      teamPerformance.value = response.data
      generateChartData()

    }
  } catch (e) {
    console.log(e)
  } finally {
    generateChartData()
    useSpinner().hideSpinner()
  }
}
function generateChartData(){
  earthChartOptions.value.labels = []
  earthChartOptions.value.labels.push(' کل مراحل', 'مراحل طی شده هم‌مسیرها', 'مراحل طی شده شما')
  // first value is 100 for chart preview , second is others completion ratio and third is my completion ratio
  earthChartOptions.value.series=[]
  earthChartOptions.value.series.push(100, Math.ceil(teamPerformance.value.averageCompletionPercentage), Math.ceil(teamPerformance.value.completionPercentage))
  renderChart.value = true
}

async function getSiteDynamicSetting() {
  try {
    spinner.renderSpinner()
    const response = await $api.dynamicSetting.getSiteDynamicSettingByKeyAndType({
      type: DynamicSettings.motivation,
      key: DynamicSettings.motivation
    })
    motivationSetting.value = response.data
    motivationSetting.value.jsonValue = JSON.parse(motivationSetting.value.jsonValue)
  } catch (error) {
    console.error(error.message);
  } finally {
    spinner.hideSpinner()
  }
}
getUserActivityForAllQuestions()
getSiteDynamicSetting()
</script>

<template>
  <ClientOnly>
    <div class="w-full flex flex-col  relative">
      <DashboardEarthChart v-if="renderChart" :options="earthChartOptions"></DashboardEarthChart>
      <div v-if="motivationSetting" class="flex gap-3 flex-col">
        <NuxtImg
            :src="motivationSetting.jsonValue.imageUrl ? motivationSetting.jsonValue.imageUrl : '/common/hasan-sister.png'"
            class="h-11 w-auto object-contain"
        />
        <p class="text-[#3E8F3E] text-xs mb-3 text-center">
          {{ motivationSetting.jsonValue.title }}
        </p>
      </div>
    </div>
  </ClientOnly>
</template>

<style scoped>

</style>