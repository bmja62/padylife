<script setup lang="ts">
import type { TicketTypes } from '@/models/Enums/TicketTypes'
import type { ValidationRule } from '@/models/IValidationRule'

// Interfaces
interface IProps {
  label?: string
  disabled?: boolean
  color?: string
  rules?: ValidationRule[]
  validationOrder?: number
  density?: Density
}
interface IEmits {
  (e: 'getTicketType', value: TicketTypes | null): void
}
interface ITicketTypeItem {
  name: string
  value: TicketTypes
}

// Types
type Density = null | 'default' | 'comfortable' | 'compact'

// Props
const props = withDefaults(defineProps<IProps>(), {
  label: 'دسته بندی تیکت',
  color: 'success',
  density: 'compact',
})

// Emits
const emits = defineEmits<IEmits>()

// Variables
const selectedTicketType = ref<TicketTypes | null>(null)

const TicketTypesPicker: ITicketTypeItem[] = [
  {
    name: 'مشکل فنی',
    value: 'TechnicalIssue',
  },
  {
    name: 'اعلام تصادف',
    value: 'AccidentAnnouncement',
  },
  {
    name: 'سایر',
    value: 'Other',
  },
  {
    name: 'مالی',
    value: 'Financial',
  },
  {
    name: 'گزارش تخلف',
    value: 'ViolationReport',
  },
  {
    name: 'پیشنهادات',
    value: 'Suggestion',
  },
  {
    name: 'مشکل در پکیج نامه',
    value: 'InsurancePaperIssue',
  },
]

watch(
  () => selectedTicketType.value,
  () => emitSelectedValue(),
)

// Functions
function emitSelectedValue() {
  emits('getTicketType', selectedTicketType.value)
}
</script>

<template>
  <CustomPicker
    v-model="selectedTicketType"
    :items="TicketTypesPicker"
    :clearable="false"
    :item-title="(item: ITicketTypeItem) => item.name"
    :item-value="(item: ITicketTypeItem) => item.value"
    rounded="lg"
    :density="props.density"
    :rules="props.rules"
    :validation-order="props.validationOrder"
    :color="props.color"
    :disabled="props.disabled"
    :label="props.label"
    :return-object="false"
  />
</template>
