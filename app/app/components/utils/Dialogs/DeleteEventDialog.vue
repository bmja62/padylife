<script setup lang="ts">
import {type ICalendarDayEvent} from "~/services/CalendarService";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'refetch'): void
}>()


interface IProps {
  selectedEvent: ICalendarDayEvent
}

const props: IProps = defineProps({
  selectedEvent: {
    type: Object as PropType<ICalendarDayEvent>
  }
})
const {$api} = useNuxtApp<IApiProvider>()

async function deleteEvent() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.calendar.deleteEvent(props.selectedEvent.id)
    if (response.isSuccess) {
      useAlerts().success('رویداد با موفقیت حذف شد')
      emits('refetch')
      isRendering.value = false
    } else {
      alert.error(
          response?.message || "مشکلی پیش آمد، لطفا دوباره امتحان کنید"
      );
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="createEvent" v-model="isRendering">
    <template #title>
      <span>حذف رویداد</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2 p-3">
        <span>آیا از حذف این رویداد اطمینان دارید؟</span>
        <div class="w-full flex items-center gap-2">
          <button type="button" @click="isRendering = false" class="btn bg-gray-400 w-1/2 text-white">
            بستن
          </button>
          <button type="button" @click="deleteEvent" class="btn bg-primary w-1/2 text-white">
            تایید
          </button>
        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>