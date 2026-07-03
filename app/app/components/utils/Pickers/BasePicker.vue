<script setup lang="ts" generic="T">
import VueSelect, {type Option} from "vue3-select-component";

interface IEmits {
  (e: "search", value?: string | null): void;
}

const emits = defineEmits<IEmits>();

interface IProps {
  options: Option<T>[];
  label?: keyof Option<T>;
  placeholder?: string;
  isLoading?: boolean;
  selectClass?: null | string;
  isSearchable?: boolean;
  clearable?: boolean;
  required?: boolean;
  isMultiple?: boolean;
  title?: string;
}

const props = withDefaults(defineProps<IProps>(), {
  label: "label",
  placeholder: "انتخاب کنید",
  title: '',
  isLoading: false,
  selectClass: null,
  isSearchable: true,
  required: false,
  clearable: false,
  isMultiple: false,
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
watch(
    () => props.isMultiple,
    (val) => console.log(val)
);

function handleSearch(searchTerm: string | null) {
  debouncedSearch.value = searchTerm;
}
</script>

<template>
  <div class="h-fit min-h-8" :class="props.options ? '' : 'skeleton'">
    <span class="font-semibold ">{{ title }} <span v-if="required" class="text-red-500">*</span> </span>
    <VueSelect
        v-if="props.options"
        v-model="model"
        dir="rtl"
        class="text-[#1f2937] mt-2 [&_*]:!custom-scrollbar [--vs-icon-color:#000]
         [--vs-line-height:32px] [--vs-border-radius:15px] [--vs-border:1px_solid_#868686]"
        :class="props.selectClass"
        :options="props.options"
        :get-option-label="(option) => `${option[props.label]}`"
        :is-clearable="props.clearable"
        :placeholder="props.placeholder"
        :is-loading="props.isLoading"
        :is-searchable="props.isSearchable"
        :is-multi="props.isMultiple"
        @search="handleSearch"
        @option-deselected="handleSearch(null)"
    >
      <template #tag="{ option, removeOption }">
        <div
            class="flex items-center gap-1 badge cursor-pointer bg-blue" @click="removeOption">
          {{ option[props.label] }}
<!--          <LazyIconsIcon name="close" class="w-3 h-3  mb-1 fill-black"></LazyIconsIcon>-->
        </div>
      </template>
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
      <template #no-options> هیچ موردی یافت نشد.</template>
    </VueSelect>
  </div>
</template>
<style scoped>
.vue-select :deep(.menu) {
  flex-wrap: nowrap;
  max-height: 150px;
  border-radius: var(--vs-menu-radius);
}

.vue-select :deep(.value-container) {
  overflow-x: auto;
  padding-right: 10px;
  gap: 5px !important;
}

.customBorder {
  --vs-border-radius: 2px !important;
  --vs-border: none !important;
  box-shadow: 0 0 2px 0 rgba(0, 0, 0, 0.25);
}

.customBorder :deep(.menu) {
  --vs-border-radius: 2px;
}
</style>
