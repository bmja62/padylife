<script setup lang="ts" generic="T">
interface IProps {
  dir?: string;
  label?: string | null;
  name: string;
  labelCustomClass?: string | null;
  radioCustomClass?: string | null;
  checked?: boolean;
  value: T;
}

const props = withDefaults(defineProps<IProps>(), {
  dir: "auto",
  name: "custom-radio",
  checked: false,
  label: null,
  labelCustomClass: null,
  radioCustomClass: null,
});

const model = defineModel<T>();
</script>

<template>
  <label
    class="label cursor-pointer justify-start gap-x-3 py-0"
    :dir="props.dir"
  >
    <input
      v-model="model"
      :value="props.value"
      type="radio"
      :name="props.name"
      class="radio custom-radio"
      :class="[radioCustomClass]"
    />
    <span class="label-text" :class="labelCustomClass">
      <slot name="label"></slot>
    </span>
  </label>
</template>
<style scoped>
.custom-radio {
  box-shadow: 0 0 0 3px var(--radio-background-color) inset,
    0 0 0 3px var(--radio-background-color) inset !important;
}
</style>
