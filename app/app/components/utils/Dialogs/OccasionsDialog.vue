<script setup lang="ts">
import type {ICalendarDay} from "~/services/CalendarService";

const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'continue'): void
}>()

interface IProps {
  selectedDay: ICalendarDay
}

const props: IProps = defineProps({
  selectedDay: {
    type: Object as PropType<ICalendarDay>
  }
})
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="welcomeBack" v-model="isRendering">
    <template #title>
      <span>مناسبت‌های این روز</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">
      <span v-for="occasion in props.selectedDay.occasions">
        {{ occasion }}
      </span>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>