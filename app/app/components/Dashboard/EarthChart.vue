<script setup lang="ts">
import ApexCharts from "apexcharts";

const chartContainer = useTemplateRef("chartContainer");

const apexChartInstance = ref<ApexCharts>();
interface IProps {
  options:object
}
const props:IProps = defineProps({
  options:{
    type: Object as PropType<object>,
  }
})
onMounted(() => {
  apexChartInstance.value = new ApexCharts(chartContainer.value, props.options);
  apexChartInstance.value.render();
});

// const apexChartInstance = new ApexCharts(chartContainer.value);
</script>

<template>
  <div ref="chartContainer" class="w-full h-auto"></div>
</template>
