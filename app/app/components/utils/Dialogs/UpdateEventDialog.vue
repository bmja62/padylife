<script setup lang="ts">
import {type ICalendarDay, type ICalendarDayEvent} from "~/services/CalendarService";
import * as Yup from "yup";
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
    type: Object as PropType<ICalendarDay>
  }
})
const {$api} = useNuxtApp<IApiProvider>()
const eventSchema = Yup.object({
  title: Yup.string().required("عنوان اجباری است"),
});

async function updateEvent() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.calendar.updateEvent(props.selectedEvent)
    if (response.isSuccess) {
      isRendering.value = false
      useAlerts().success('رویداد با موفقیت بروزرسانی شد')
      emits('refetch')
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
  <LazyUtilsDialogsBaseDialog dialog-id="updateEvent" v-model="isRendering">
    <template #title>
      <span>ویرایش رویداد</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">
        <UtilsFormWrapper :schema="eventSchema" @submit="updateEvent">
          <div class="w-full flex flex-col gap-y-5 ">
            <UtilsInputsBaseInput
                v-model="props.selectedEvent.title"
                name="title"
                bordered
                placeholder="عنوان رویداد"
            ></UtilsInputsBaseInput>
            <LazyUtilsPickersEventTypePicker v-model="props.selectedEvent.type"></LazyUtilsPickersEventTypePicker>
            <textarea
                v-model="props.selectedEvent.description"
                class="textarea textarea-bordered rounded-2xl w-full focus:outline-none"
                placeholder="یادداشت"
            ></textarea>
            <button type="submit" class="btn bg-primary text-white w-full">
              بروزرسانی
            </button>
          </div>
        </UtilsFormWrapper>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>