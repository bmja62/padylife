<script setup lang="ts">
import type {IPlanTeamChart} from "~/services/ReportService";

interface IProps {
  icon: string,
  title: string,
  subTitle: string,
  dataPoints: IPlanTeamChart[],
}

const isOpen = ref(false);


const props = withDefaults(defineProps<IProps>(), {
  icon: "sleep",
  title: "خواب",
})

const options = ref({
  series: [
    {
      name: 'تعداد  پاسخ ها',
      data: []
    },
    {
      name: 'امتیاز کسب شده',
      data: []
    }
  ],
  colors: ["#63ABFD", '#FFE138'],
  chart: {
    fontFamily: "Peyda",
    height: 250,
    type: 'bar',
    toolbar: {
      show: false,
    },
    zoom: {
      enabled: false,
    },
  },
  legend: {
    show: true,
    horizontalAlign: "right",
    markers: {
      shape: "square",
    },
  },
  plotOptions: {
    bar: {
      borderRadius: 4,
      columnWidth: '25%',
      distributed: false,
    }
  },

  dataLabels: {
    enabled: false,
  },
  xaxis: {
    categories: [].reverse(),
    axisTicks: {
      show: false,
    },
    labels: {
      style: {
        fontSize: '14px',
        fontWeight: "bold"
      }
    }
  },
  yaxis: {
    tickAmount: 12,
    max: 24,
    show: true,
  },
  grid: {
    show: false
  }
})
onMounted(() => {
  if (props?.dataPoints?.length) {
    props.dataPoints.forEach((dataPoint) => {
      options.value.xaxis.categories.push(new Date(dataPoint.periodLabel).toLocaleDateString('fa-IR', {weekday: "long"}))
      options.value.series[0].data.push(dataPoint.answerCount)
      options.value.series[1].data.push(dataPoint.earnedPoints)
    })
  }
})
</script>

<template>
  <LazyUtilsDropDown class="!p-0" v-model:is-open="isOpen" @click="isOpen = !isOpen">
    <template #title>
      <div class="flex justify-between bg-white p-4 rounded-[8px]">
        <div class="flex flex-row items-center gap-3">
          <nuxtImg :src="icon" class="w-[30px] h-[30px] "/>
          <div class="text-[#0F1419] text-sm font-bold line-clamp-1 tooltip" :data-tip="title">{{ title }}</div>

        </div>

        <div class="flex flex-row items-center">
          <Icon name="icon:chevron-right" size="15" class="transform  transition-all duration-700"
                :class="[isOpen ? '-rotate-90' : 'rotate-90']"/>
        </div>
      </div>
    </template>
    <template #content>
      <ChartsBarChart :options="options" class="bg-white"/>
    </template>
  </LazyUtilsDropDown>
</template>

<style scoped>

</style>