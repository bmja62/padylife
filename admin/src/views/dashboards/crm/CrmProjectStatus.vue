<script setup lang="ts">
import VueApexCharts from 'vue3-apexcharts'

const props = defineProps({
  title: {
    type: String as PropType<string>
  },
  chartOptions: {},
  data: {}
})


</script>

<template>
  <VCard :title="props.title">
    <VueApexCharts
      :options="chartOptions"
      :series="data"
      height="247"
    />
  </VCard>
</template>

<style lang="scss" scoped>

</style>
