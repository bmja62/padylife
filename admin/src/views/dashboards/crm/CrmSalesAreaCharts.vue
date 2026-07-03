<script setup lang="ts">
import VueApexCharts from 'vue3-apexcharts'
import {useTheme} from 'vuetify'
import LongTimePicker from "@/components/Analytics/LongTimePicker.vue";

const vuetifyTheme = useTheme()
const props = defineProps({
  count: {
    type: Number as PropType<number>
  }
})
const currentTheme = vuetifyTheme.current.value.colors

const series = [
  {
    name: '',
    data: [200, 55, 400, 250],
  },
]

const chartOptions = {
  chart: {
    type: 'area',
    parentHeightOffset: 0,
    toolbar: {
      show: false,
    },
    sparkline: {
      enabled: true,
    },
  },
  markers: {
    colors: 'transparent',
    strokeColors: 'transparent',
  },
  grid: {
    show: false,
  },
  colors: [currentTheme.success],
  fill: {
    type: 'gradient',
    gradient: {
      shadeIntensity: 0.9,
      opacityFrom: 0.5,
      opacityTo: 0.07,
      stops: [0, 80, 100],
    },
  },
  dataLabels: {
    enabled: false,
  },
  stroke: {
    width: 2,
    curve: 'smooth',
  },
  xaxis: {
    show: true,
    lines: {
      show: false,
    },
    labels: {
      show: false,
    },
    stroke: {
      width: 0,
    },
    axisBorder: {
      show: false,
    },
  },
  yaxis: {
    stroke: {
      width: 0,
    },
    show: false,
  },
  tooltip: {
    enabled: false,
  },
}
const selectedStatTime = defineModel()
</script>

<template>
  <VCard>
    <VCardText class="pb-2">
      <h5 class="text-h5">
        مبلغ سفارشات
      </h5>
      <p class="my-2">
        <LongTimePicker v-model="selectedStatTime"></LongTimePicker>
      </p>
    </VCardText>

    <VueApexCharts
      :options="chartOptions"
      :series="series"
      :height="72"
    />

    <VCardText class="pt-0">
      <div class="d-flex align-center justify-space-between mt-3">
        <h4 class="text-h4">
          {{ Intl.NumberFormat('fa-IR').format(count) }} تومان
        </h4>

      </div>
    </VCardText>
  </VCard>
</template>
