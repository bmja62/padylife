<script setup lang="ts">
import ApexCharts from "apexcharts";

const chartContainer = useTemplateRef("chartContainer");

const apexChartInstance = ref<ApexCharts>();
interface IProps {
  options?: object;
}
const props = withDefaults(defineProps<IProps>(),{
  options: {
    series: [
      {
        name: "میانگین",
        data: [90, 29, 44, 20, 65, 79, 87],
      },
      {
        name: "روان",
        data: [20, 29, 37, 36, 44, 45, 58],
      },
      {
        name: "ورزش",
        data: [10, 26, 55, 35, 55, 8, 100],
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
      categories: [
        "شنبه",
        "یکشنبه",
        "دوشنبه",
        "سه شنبه",
        "چهارشنبه",
        "پنجشنبه",
        "جمعه",
      ].reverse(),
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
      max: 100,
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
    },
  }
})


onMounted(() => {
  apexChartInstance.value = new ApexCharts(chartContainer.value, props.options);
  apexChartInstance.value.render();

});
</script>

<template>
  <div ref="chartContainer"></div>
</template>

<style scoped></style>
