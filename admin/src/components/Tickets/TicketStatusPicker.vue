<script setup lang="ts">
import { TicketStatusesPersian } from '@/models/Enums/TicketTypes'
import type { TicketStatuses } from '@/models/Enums/TicketTypes'
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
  (e: 'getTicketStatus', value: TicketStatuses | null): void
}
interface ITicketTypeItem {
  name: string
  value: TicketStatuses
}

// Types
type Density = null | 'default' | 'comfortable' | 'compact'

// Props
const props = withDefaults(defineProps<IProps>(), {
  label: 'وضعیت تیکت',
  color: 'success',
  density: 'compact',
})

// Emits
const emits = defineEmits<IEmits>()

// Variables
const selectedTicketStatus = ref<TicketStatuses | null>(null)

const TicketTypesPicker: ITicketTypeItem[] = [
  {
    name: TicketStatusesPersian.WaitingForSupport,
    value: 'WaitingForSupport',
  },
  {
    name: TicketStatusesPersian.WaitingForUser,
    value: 'WaitingForUser',
  },
  {
    name: TicketStatusesPersian.Closed,
    value: 'Closed',
  },
]

watch(
  () => selectedTicketStatus.value,
  () => emitSelectedValue(),
)

// Functions
function emitSelectedValue() {
  emits('getTicketStatus', selectedTicketStatus.value)
}
</script>

<template>
  <CustomPicker
    v-model="selectedTicketStatus"
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
