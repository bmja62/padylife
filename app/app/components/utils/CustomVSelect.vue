<script setup lang="ts" generic="T">
import VueSelect, { type Option } from "vue3-select-component";

interface IEmits {
  (e: "search", value?: string | null): void;
}

const emits = defineEmits<IEmits>();

interface IProps {
  options: Option<T>[];
  label?: keyof Option<T>;
  placeholder?: string;
  isLoading?: boolean;
}

const props = withDefaults(defineProps<IProps>(), {
  label: "label",
  placeholder: "انتخاب کنید",
  isLoading: false,
});

const model = defineModel<unknown>();

const searchModel = defineModel<string | null>("search");

const debounceTimeout = ref<ReturnType<typeof setTimeout> | null>(null);

const previousSearchValue = ref<string | null | undefined>(null);

const debouncedSearch = computed({
  get() {
    return searchModel.value;
  },
  set(val) {
    // Don't proceed if the value hasn't changed or it's an empty string
    if (val === previousSearchValue.value || val === "") {
      return;
    }

    // Clear the previous timeout if it exists
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }

    // Set up the debounce timeout
    debounceTimeout.value = setTimeout(() => {
      // Only update if val is not an empty string
      if (val) {
        model.value = undefined; // Reset model if necessary
        searchModel.value = val;
      } else {
        searchModel.value = null; // Or some default value
      }

      // Emit the search event only if the value is not empty
      emits("search", val);

      // Update the previous search value
      previousSearchValue.value = val;
    }, 600);
  },
});

function handleSearch(searchTerm: string | null) {
  debouncedSearch.value = searchTerm;
}
</script>

<template>
  <div class="w-full h-fit min-h-8" :class="props.options ? '' : 'skeleton'">
    <VueSelect
      v-if="props.options"
      v-model="model"
      dir="rtl"
      class="rounded-full [&_*]:!custom-scrollbar [--vs-border-radius:30px] [--vs-input-bg:transparent] [--vs-option-focused-color:transparent] [--vs-option-selected-color:#EA5348] [--vs-border:1px_solid_#000] [--vs-input-outline:unset]"
      :options="props.options"
      :get-option-label="(option) => `${option[props.label]}`"
      :placeholder="props.placeholder"
      :is-loading="props.isLoading"
      @search="handleSearch"
      @option-deselected="handleSearch(null)"
    >
      <template v-if="$slots.selected" #value="{ option }">
        <slot name="selected" :item="option">
          {{ option.value }}
        </slot>
      </template>
      <template v-if="$slots.option" #option="{ option }">
        <slot name="option" :item="option">
          {{ option.label }}
        </slot>
      </template>
    </VueSelect>
  </div>
</template>
<style scoped>
.vue-select :deep(.control.focused) {
  border: 1px solid #000;
  box-shadow: none !important;
}
.vue-select :deep(.menu) {
  padding: 0.8rem 0;
  flex-wrap: nowrap;
}
.vue-select :deep(.menu-option.selected) {
  color: #ea5348;
  background-color: transparent;
}

.vue-select :deep(.menu-option:not(:last-child)) {
  border-bottom: 1px solid #8d8d8d;
}
.vue-select :deep(.single-value) {
  white-space: nowrap;
}
</style>
