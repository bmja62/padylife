<script setup lang="ts">
const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  },
  clearable: {
    type: Boolean as PropType<boolean>,
    default: false
  },
})
const selectedOption = defineModel()
const options = ref([
  {
    label: 'شخصی',
    value: "Personal"
  },
  {
    label: 'کاری',
    value: "Work"
  },
  {
    label: 'قرار ملاقات',
    value: "Meeting"
  },
  {
    label: 'یادآوری',
    value: "Reminder"
  },
  {
    label: 'دیگر',
    value: "Other"
  },
])

</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length" :required="props.required"
                              placeholder="انتخاب نوع رویداد"
                              :clearable="props.clearable"
                              v-model="selectedOption"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>