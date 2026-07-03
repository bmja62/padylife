<script setup lang="ts">
import type {ICalendarDayEvent} from "~/services/CalendarService";

interface IProps {
  event: ICalendarDayEvent
}

const emits = defineEmits<{
  (e: 'refetch'): void
}>()
const isRenderingDeleteEventDialog = ref(false)
const isRenderingUpdateEventDialog = ref(false)
const showMore = ref(false)
const props: IProps = defineProps({
  event: {
    type: Object as PropType<ICalendarDayEvent>
  }
})

</script>

<template>
  <div
      class="w-full p-3 bg-white text-black shadow rounded-2xl flex flex-col gap-2" style="overflow-wrap: anywhere">

    <div class="w-full flex   items-center justify-between">
      <span>{{ props.event.title }}</span>
      <div class="flex items-center gap-2">
        <div class="cursor-pointer" @click="isRenderingUpdateEventDialog = true">
          <LazyIconsEditIcon class="fill-blue-500 mt-1"></LazyIconsEditIcon>
        </div>
        <div class="cursor-pointer" @click="isRenderingDeleteEventDialog = true">
          <LazyIconsTrashIcon class="fill-primary"></LazyIconsTrashIcon>
        </div>
        <div class="cursor-pointer" @click="showMore =! showMore">
          <LazyIconsChevronLeftIcon :class="{'!rotate-90':showMore}" class="fill-gray-400 transition-all transform -rotate-90"></LazyIconsChevronLeftIcon>
        </div>
      </div>
    </div>
    <div v-if="showMore" class="transition-all">
      <p class="text-gray-700">{{ props.event.description }}</p>
    </div>
    <LazyUtilsDialogsDeleteEventDialog @refetch="emits('refetch')" :selected-event="props.event"
                                       v-model="isRenderingDeleteEventDialog"></LazyUtilsDialogsDeleteEventDialog>
    <LazyUtilsDialogsUpdateEventDialog @refetch="emits('refetch')" :selected-event="props.event"
                                       v-model="isRenderingUpdateEventDialog"></LazyUtilsDialogsUpdateEventDialog>
  </div>
</template>

<style scoped>

</style>