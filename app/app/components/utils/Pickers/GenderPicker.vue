<script setup lang="ts">
const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})
const selectedOption = defineModel()
const options = ref([
  {
    label: 'مرد',
    value: "Male"
  },
  {
    label: 'زن',
    value: "Female"
  },
])

</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length"  :required="props.required"
                              placeholder="انتخاب جنسیت"
                              v-model="selectedOption"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>