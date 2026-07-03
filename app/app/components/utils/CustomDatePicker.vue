<script setup lang="ts">
import DatePicker from 'vue3-persian-datetime-picker'
import {ErrorMessage, useField} from "vee-validate";

const selectedDate = defineModel<Date | string>()

enum datePickerTypes {
  date = 1,
  time = 2,
  datetime = 3,
}

const props = defineProps({
  required: {
    type: Boolean,
    default: true
  },
  label: {
    type: String
  },
  id: {
    type: String
  },
  name: {
    type: String,
    default:'test'
  },
  type: {
    type: String as PropType<typeof datePickerTypes>,
    default: "date",
    required: false,
  },
  placeholder: {
    type: String as PropType<string>,
    default: "انتخاب تاریخ",
  },
  simple: {
    type: Boolean as PropType<boolean>,
    default: false
  }
})
const {value} = useField(props.name, undefined, {
  initialValue: selectedDate.value,
  validateOnValueUpdate: false
});

</script>

<template>
  <div class="bg-white rounded-2xl">
    <span dir="rtl" class="xl:block hidden  mb-2">{{ label }} <span v-if="props.required" class="text-red-500">*</span></span>
    <div class=" flex items-center border  border-gray-300 rounded-2xl ">
      <input

          :id="`${props.id}`"
          :placeholder="props.placeholder"
          class="w-full custom-input  placeholder:text-right rounded-2xl leading-10 ring-0 text-[14px] pr-2 placeholder:text-[14px] outline-0 px-2"
      />
      <Icon v-if="selectedDate" name="icon:close" class="ml-2 [&_*]:stroke-red-500" @click="selectedDate = null" size="14" />
      <Icon name="icon:calendar" class="ml-2" size="20" />
    </div>
    <DatePicker
        :simple="props.simple"
        :initial-value="selectedDate"
        format="YYYY/MM/DD"
        displayFormat="jYYYY/jMM/jDD"
        :custom-input="`#${props.id}`"
        :type="props.type"
        v-model="selectedDate"
        append-to="body"
    />

    <ErrorMessage v-slot="{ message }" :name="props.name" as="span" class="text-sm !text-red-500">
      {{ message }}
    </ErrorMessage>
  </div>
</template>

<style scoped>

</style>