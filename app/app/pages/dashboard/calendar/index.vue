<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {ICalendar, ICalendarDay, ICalendarDayEvent, IGetCalendarParams} from "~/services/CalendarService";

definePageMeta({
  auth: true,
  layout: "dashboard",
})

useHead({
  title: 'تقویم'
})
const {$api} = useNuxtApp<IApiProvider>()
const authStore = useAuthStore()
const calendar = ref<ICalendar>(null)
const weekdays = ref([
  {
    title: 'ش',
    symbol: 'sat'
  },
  {
    title: 'ی',
    symbol: 'sun'
  },
  {
    title: 'د',
    symbol: 'mon'
  },
  {
    title: 'س',
    symbol: 'thu'
  },
  {
    title: 'چ',
    symbol: 'wed'
  },
  {
    title: 'پ',
    symbol: 'thur'
  },
  {
    title: 'ج',
    symbol: 'fri'
  },
])
const selectedDay = ref<ICalendarDay>(null)
const isRenderingOccasionsDialog = ref(false)
const isRenderingCreateEventDialog = ref(false)
const selectedDayEvents = ref<ICalendarDayEvent[]>([])
const calendarFilters = ref<IGetCalendarParams>({
  shamsiMonth: useTimeUtils().getCurrentMonth(),
  shamsiYear: useTimeUtils().getCurrentYear(),
})

async function getCalendar() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.calendar.getCalendar(calendarFilters.value)
    calendar.value = response.data
    selectedDay.value = calendar.value.days.filter(e => useTimeUtils().isToday(e.date))[0]
    getDayEvents()
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getDayEvents() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.calendar.getEventsByDay(selectedDay.value.date)
    selectedDayEvents.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

function previousMonth() {
  if (calendarFilters.value.shamsiMonth > 1) {
    calendarFilters.value.shamsiMonth--
  } else if (calendarFilters.value.shamsiMonth === 1) {
    calendarFilters.value.shamsiMonth = 12
    calendarFilters.value.shamsiYear--
  }
  getCalendar()
}

function nextMonth() {
  if (calendarFilters.value.shamsiMonth < 12) {
    calendarFilters.value.shamsiMonth++
  } else if (calendarFilters.value.shamsiMonth === 12) {
    calendarFilters.value.shamsiMonth = 1
    calendarFilters.value.shamsiYear++
  }
  getCalendar()
}

function setSelectedDay(day) {
  selectedDay.value = day
  getDayEvents()
}

async function getBoth() {
  await getCalendar()
  await getCalendar()
}

await getCalendar()

</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>تقویم</template>
        </BaseNotificationHeader>
      </template>
      <div
          v-if="calendar"
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start  "
      >
        <div class="w-full flex flex-col gap-y-4 py-5">
          <div class="bg-orange-50 rounded-2xl p-4 shadow-md space-y-4">
            <!-- Header: Mood Type & Date -->
            <div class="w-full flex justify-around relative bottom-7">
              <div
                  class=" w-3 h-8 bg-gray-400 border border-gray-400 rounded-lg"></div>
              <div class="mt-6 flex items-center  gap-5">
                <div @click="previousMonth"
                     class="cursor-pointer border rounded-full border-primary w-7 h-7 flex items-center justify-center">
                  <LazyIconsChevronLeftIcon
                      class="!fill-primary !w-5 !h-4 transform rotate-180"></LazyIconsChevronLeftIcon>
                </div>
                <span class="font-bold text-primary">{{
                    useTimeUtils().getMonthName(calendarFilters.shamsiMonth)
                  }} {{ calendarFilters.shamsiYear }} </span>
                <div @click="nextMonth"
                     class="cursor-pointer border rounded-full border-primary w-7 h-7 flex items-center justify-center">
                  <LazyIconsChevronLeftIcon class="!fill-primary !w-4 !h-4"></LazyIconsChevronLeftIcon>
                </div>
              </div>
              <div
                  class=" w-3 h-8 bg-gray-400 border border-gray-400 rounded-lg"></div>
            </div>
            <div class="!mt-0 grid grid-cols-7 gap-2">
              <div class="w-full col-span-7 bg-primary p-3 rounded-xl flex items-center justify-between">
                <span v-for="weekDay in weekdays" class="text-center text-white">{{ weekDay.title }}</span>
              </div>
              <LazyCalendarCell v-for="day in calendar.days"
                                v-model="selectedDay"
                                @click="setSelectedDay(day)" :key="day.dayId" :day="day"></LazyCalendarCell>
            </div>
          </div>
          <div v-if="selectedDay" class="bg-orange-50 rounded-2xl relative p-4 shadow-md mb-6 space-y-4">
            <div class="w-full flex items-center justify-between">
            <span class="font-bold">{{
                new Date(selectedDay.date).toLocaleDateString('fa-IR', {
                  day: 'numeric',
                  month: 'long',
                  year: 'numeric'
                })
              }}</span>
              <div v-if="selectedDay.occasions.length" @click="isRenderingOccasionsDialog = true"
                   class="tooltip tooltip-right"
                   data-tip="مشاهده رویداد‌های این روز">
                <LazyIconsInfoIcon class="fill-primary w-5 h-5"></LazyIconsInfoIcon>
              </div>
            </div>
            <div v-if="!selectedDayEvents.length" class="w-full flex flex-col items-center justify-center gap-4">
              <span class="text-gray-500 font-bold">هیچ رویدادی برای امروز ثبت نشده است</span>
              <LazyIconsCalendarCloseIcon class="!w-10 !h-10 fill-gray-500"></LazyIconsCalendarCloseIcon>
            </div>
            <div v-else class="w-full flex flex-col gap-4">
              <LazyCalendarEventCard @refetch="getBoth" v-for="event in selectedDayEvents"
                                     :event="event"></LazyCalendarEventCard>
            </div>

            <button @click="isRenderingCreateEventDialog = true" type="button"
                    class="sticky bottom-0 btn bg-primary text-white w-full">
              ثبت رویداد جدید
            </button>
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsOccasionsDialog v-model="isRenderingOccasionsDialog"
                                     :selected-day="selectedDay"></LazyUtilsDialogsOccasionsDialog>
    <LazyUtilsDialogsCreateEventDialog @refetch="getBoth" :selected-day="selectedDay"
                                       v-model="isRenderingCreateEventDialog"></LazyUtilsDialogsCreateEventDialog>

  </div>
</template>

<style scoped></style>
