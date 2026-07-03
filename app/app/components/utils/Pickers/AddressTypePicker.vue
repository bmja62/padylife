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
    label: 'خانه',
    value: "Home"
  },
  {
    label: 'محل کار',
    value: "Work"
  },
  {
    label: 'دیگر',
    value: "Other"
  },
])

</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length"  :required="props.required"
                              placeholder="انتخاب نام آدرس"
                              v-model="selectedOption"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>