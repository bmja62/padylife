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
    label: 'مجرد',
    value: "Single"
  },
  {
    label: 'متاهل',
    value: "Married"
  },
])

</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length"  :required="props.required"
                              placeholder="انتخاب وضعیت تاهل"
                              v-model="selectedOption"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>