<script setup lang="ts">
import type {ValidationRule} from '@/models/IValidationRule'
import {AutoRepairManPersianStatus} from '@/models/Enums/AutoRepairEnums'
import type {AutoRepairManStatusTypes} from '@/models/Enums/AutoRepairEnums'
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
  color?: string
  rules?: ValidationRule[]
  validationOrder?: number,
  returnObject: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  label: 'انتخاب شرط کد تخفیف ',
  disabled: false,
  color: 'success',
  returnObject: true
})

// Emits
const emits = defineEmits<IEmits>()

// Variables
const tempSelectedType = defineModel()

const DiscountTypes = [
  {
    name: 'وابسته به دسته بندی',
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
    :rules="props.rules"
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
