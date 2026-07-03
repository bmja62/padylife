<script setup lang="ts">
import type {ValidationRule} from '@/models/IValidationRule'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const $api = inject<IApiProvider>('$api')

// Interfaces
interface IEmits {
  (e: 'getSelectedType', value: any | null): void
}

interface IProps {
  label?: string
  disabled?: boolean
  required?: boolean
  color?: string
  rules?: ValidationRule[]
  validationOrder?: number,
  returnObject: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  label: 'انتخاب نوع کد تخفیف *',
  disabled: false,
  required: false,
  color: 'success',
  returnObject: true
})

// Emits
const emits = defineEmits<IEmits>()

// Variables
const tempSelectedType = defineModel()

const DiscountTypes = [
  {
    name: 'درصدی',
    value: 1
  },
  {
    name: 'عددی',
    value: 2
  }
]
// Watchers
watch(
  () => tempSelectedType.value,
  value => {
    emits('getSelectedType', value)
  },
)

// functions

</script>

<template>
  <CustomPicker
    v-model="tempSelectedType"
    rounded="lg"
    :required="props.required"
    :rules="props.required ? [(value) => !!value || 'فیلد اجباری است']:[]"
    :validation-order="props.validationOrder"
    :disabled="props.disabled"
    :color="props.color"
    :label="props.label"
    :items="DiscountTypes"
    :return-object="props.returnObject"
    :item-title="(item: any) => item.name"
    :item-value="(item: any) => item.value"
    :search-callback="() => null"
  />
</template>
