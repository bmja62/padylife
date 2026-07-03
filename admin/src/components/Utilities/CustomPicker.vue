<script generic="T" lang="ts" setup>
import {useDebounceFn} from '@vueuse/core'
import type {ValidationRule} from '@/models/IValidationRule'

// Interfaces
interface IProps {
  itemTitle: null | undefined | ((item: any) => unknown | null)
  itemValue: null | undefined | ((item: any) => unknown | null)
  searchCallback?: (value: any) => void
  color: string
  label: string
  multiple?: boolean
  chips?: boolean
  resetAfterSearch?: boolean
  returnObject?: boolean
  rules?: null | ValidationRule[]
  clearable?: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  multiple: false,
  chips: false,
  resetAfterSearch: false,
  returnObject: true,
  clearable: true,
  searchCallback: () => {
  },
})

// Models
const items = defineModel<unknown[] | null>('items')
const isLoading = defineModel<boolean>('isLoading')
const model = defineModel<T>()
const search = ref<string | undefined>(undefined)

// Watchers
watch(model, () => {
  if (props.resetAfterSearch)

    search.value = undefined
})

// Functions
const debouncedRequest = useDebounceFn(value => {
  props.searchCallback(value)
}, 300)
</script>

<template>
  <VAutocomplete
    v-if="items"
    v-model="model"
    v-model:search="search"
    :chips="props.chips"
    :closable-chips="props.chips"
    :color="props.color"
    :item-title="props.itemTitle"
    :item-value="props.itemValue"
    :items="items"
    :label="props.label"
    :loading="isLoading"
    :multiple="props.multiple"
    :clearable="props.clearable"
    :rules="props.rules"
    hide-selected
    no-data-text="هیچ موردی یافت نشد"
    :return-object="props.returnObject"
    variant="outlined"
    @update:search="debouncedRequest"
  >

  </VAutocomplete>
</template>
