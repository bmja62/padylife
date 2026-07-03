<script setup lang="ts">
import ApexCharts from "apexcharts";

const chartContainer = useTemplateRef("chartContainer");

const apexChartInstance = ref<ApexCharts>();

interface IProps {
  options?: object;
}

const props = withDefaults(defineProps<IProps>(), {
  options: {
    series: [
      {
        data: [21, 22, 10, 40, 16, 21, 13],
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
    colors: ["#FFD900", "#70B2FF", "#FF2D2D", "#1DF576", "#FF9500"],
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
      style: {
        fontSize: "20px",
      },
      formatter: function (val, opt) {
        // add some logic to dynamically choose emoji in the future
        return "🙂";
      },
    },
    legend: {
      show: false,
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
  },
});

onMounted(() => {
  apexChartInstance.value = new ApexCharts(chartContainer.value, props.options);
  apexChartInstance.value.render();
});
</script>

<template>
  <div ref="chartContainer" class=""></div>
</template>

<style scoped></style>
