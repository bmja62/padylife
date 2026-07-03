<script lang="ts" setup>
import DatePicker from 'vue3-persian-datetime-picker'
import {VForm} from 'vuetify/components'
import type {ValidationRule} from '@/models/IValidationRule'

// Interfaces
interface IProps {
  inputId: string
  label?: string
  disabled?: boolean
  simple?: boolean
  color?: string
  rules?: ValidationRule[]
  validationOrder?: number
  min?: string
  max?: string
  defaultDate?: Date | string | null
  density?: Density
}

type Density = null | 'default' | 'comfortable' | 'compact'

interface IEmit {
  (e: 'getselectedDate', value: Date | null): void
}


// Props
const props = withDefaults(defineProps<IProps>(), {
  inputId: 'dateSelector',
  label: 'تاریخ را انتخاب کنید',
  disabled: false,
  simple: false,
  color: 'success',
  min: '',
  max: '',
  density: 'default',
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const selectedDate = defineModel()




// Computed
const formattedDate = computed({
  get() {
    if (selectedDate.value)
      return new Date(selectedDate.value).toLocaleDateString('fa-IR')
  },

  set() {
    // Note: we don't need to set value in here.
  },
})

// Functions
function clearDateFilter() {
  selectedDate.value = null
}
</script>

<template>
  <div>
    <Teleport to="body">
      <DatePicker
        @click.prevent
        v-model="selectedDate"
        :custom-input="`#${props.inputId}`"
        :max="props.max"
        :min="props.min"
        class="custom-date-picker"
        clearable
        display-format="jYYYY/jMM/jDD"
        format="YYYY-MM-DD"
        input-format="YYYY/MM/DD"
      />
    </Teleport>
    <VForm>
      <VTextField
        :id="props.inputId"
        v-model="formattedDate"
        :append-inner-icon="
          selectedDate ? 'tabler:x' : 'tabler:calendar-clock'
        "
        density="compact"
        :color="props.color"
        :density="props.density"
        :disabled="props.disabled"
        :label="props.label"
        :rules="props.rules"

        rounded="lg"
        variant="outlined"
        @click:append-inner="clearDateFilter"
      />
    </VForm>
  </div>
</template>

<style scoped>
.custom-date-picker {
  position: absolute;
  visibility: none;
}
</style>
