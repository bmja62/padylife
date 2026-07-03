<script setup lang="ts">
import type {ICalendarDay} from "~/services/CalendarService";

interface IProps {
  day: ICalendarDay
}
const selectedDay = defineModel()

const props: IProps = defineProps({
  day: {
    type: Object as PropType<ICalendarDay>
  }
})

const computedBg = computed(() => {
  if (props.day.events.length) {
    if (props.day.events.length < 3) {
      return 'bg-green-500'
    } else if (props.day.events.length < 5) {
      return 'bg-yellow-500'
    } else {
      return 'bg-red-500'
    }

  }
})
</script>

<template>
  <div
       :class="{'border !border-primary rounded-xl':useTimeUtils().isToday(props.day.date),'border border-primary/50 rounded-xl':selectedDay?.dayId === props.day.dayId}"
       class="col-span-1 py-[6px] cursor-pointer flex items-center justify-center relative">
    <div :class="computedBg" v-if="props.day.events.length"
         class="w-6 h-6 rounded-full flex items-center  border border-gray-300 justify-center absolute -top-2 -right-2">
      {{ props.day.events.length }}
    </div>
                <span class="text-base font-bold"
                      :class="[{'!text-gray-400':!props.day.isInMonth},{'!text-primary':props.day.isHoliday}]">{{
                    props.day.shamsiDay
                  }}</span>
  </div>
</template>

<style scoped>

</style>