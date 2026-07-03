<script setup lang="ts">
import DatePicker from 'vue3-persian-datetime-picker'

// Interfaces
interface IProps {
  inputId: string
  label: string
}
interface IEmit {
  (e: 'getDateRange', value: [] | Date[]): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  inputId: 'rangeDateSelector',
  label: 'بازه‌ی تاریخ را انتخاب کنید',
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const dateRange = ref<Date[] | []>([])

// Watchers
watch(() => dateRange.value, () => {
  emit('getDateRange', dateRange.value)
})

// Computed
const formattedDate = computed({
  get() {
    if (dateRange.value && dateRange.value[0] && dateRange.value[1])
      return `${new Date(dateRange.value[0]).toLocaleDateString('fa-IR')} - ${new Date(dateRange.value[1]).toLocaleDateString('fa-IR')}`
  },

  set() {
    // Note: we don't need to set value in here.
  },
})

// Functions
function clearDateFilter() {
  dateRange.value = []
}
</script>

<template>
  <div>
    <DatePicker
      v-model="dateRange"
      range
      clearable
      :custom-input="`#${inputId}`"
      display-format="jYYYY/jMM/jDD"
      input-format="YYYY/MM/DD"
      format="YYYY-MM-DD"
    />
    <VTextField
      :id="props.inputId"
      v-model="formattedDate"
      :label="props.label"
      :append-inner-icon="dateRange.length > 0 ? 'tabler:x' : 'tabler:calendar-clock'"
      variant="outlined"
      @click:append-inner="clearDateFilter"
    />
  </div>
</template>
