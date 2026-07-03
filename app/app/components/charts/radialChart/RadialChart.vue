<script setup lang="ts">
import ApexCharts from "apexcharts";

interface IProps {
  percent?: number;
  color?: string;
}

const props = withDefaults(defineProps<IProps>(), {
  percent: 60,
  color:'#01CED1'
});

const chartContainer = useTemplateRef("chartContainer");

const apexChartInstance = ref<ApexCharts>();

onMounted(() => {
  apexChartInstance.value = new ApexCharts(chartContainer.value, {
    series: [props.percent],
    colors: [props.color],
    chart: {
      fontFamily: "Peyda",
      height: 90,
      width: 90,
      type: "radialBar",
    },
    plotOptions: {
      radialBar: {
        offsetY: -10,
        startAngle: 0,
        endAngle: 360,
        hollow: {
          margin: 15,
          size: "45%",
        },
        track: {
          background: "#ffff",
          startAngle: 216,
          endAngle: 360,
        },
        dataLabels: {
          name: {
            show: false,
          },
          value: {
            offsetY: 5,
            fontSize: "12px",
            show: true,
          },
        },
      },
    },
    stroke: {
      lineCap: "round",
    },
    labels: [""],
  });
  apexChartInstance.value.render();
});
</script>

<template>
  <div ref="chartContainer" class=""></div>
</template>

<style scoped></style>
