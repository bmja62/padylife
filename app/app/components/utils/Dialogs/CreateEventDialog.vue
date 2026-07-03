<script setup lang="ts">
import {type ICalendarDay, type ICreateEventPayload} from "~/services/CalendarService";
import * as Yup from "yup";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'refetch'): void
}>()


interface IProps {
  selectedDay: ICalendarDay
}

const props: IProps = defineProps({
  selectedDay: {
    type: Object as PropType<ICalendarDay>
  }
})
const {$api} = useNuxtApp<IApiProvider>()
const eventPayload = ref<ICreateEventPayload>({
  date: null,
  title: '',
  description: '',
  type: 'Personal'
})
const eventSchema = Yup.object({
  title: Yup.string().required("عنوان اجباری است"),
});

async function createEvent() {
  try {
    useSpinner().renderSpinner()
    eventPayload.value.date =props.selectedDay.date
    const response = await $api.calendar.createEvent(eventPayload.value)
    if (response.isSuccess) {
      eventPayload.value = {
        date: null,
        title: '',
        description: '',
        type: 'Personal'
      }
      isRendering.value = false
      useAlerts().success('رویداد با موفقیت ثبت شد')
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
  <LazyUtilsDialogsBaseDialog dialog-id="createEvent" v-model="isRendering">
    <template #title>
      <span>ایجاد رویداد جدید</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-2">
        <UtilsFormWrapper :schema="eventSchema" @submit="createEvent">
          <div class="w-full flex flex-col gap-y-5 ">
            <UtilsInputsBaseInput
                v-model="eventPayload.title"
                name="title"
                bordered
                placeholder="عنوان رویداد"
            ></UtilsInputsBaseInput>
            <LazyUtilsPickersEventTypePicker v-model="eventPayload.type"></LazyUtilsPickersEventTypePicker>
            <textarea
                v-model="eventPayload.description"
                class="textarea textarea-bordered rounded-2xl w-full focus:outline-none"
                placeholder="یادداشت"
            ></textarea>
            <button type="submit" class="btn bg-primary text-white w-full">
              ثبت
            </button>
          </div>
        </UtilsFormWrapper>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>