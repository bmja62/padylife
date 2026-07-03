<script setup lang="ts">
import {useField} from "vee-validate";

interface IProps {
  name: string;
  label?: string;
  placeholder: string;
  type?: InputTypeHTMLAttribute;
  inputmode?: InputMode;
  disabled?: boolean;
  maxlength?: string | number;
  dir?: string;
  labelClass?: string;
  inputClass?: string;
  readonly?: boolean;
  bordered?: boolean;
  autocomplete?: string;
}
const props = withDefaults(defineProps<IProps>(), {
  name: undefined,
  label: "وارد کنید",
  placeholder: "وارد کنید",
  type: "text",
  inputmode: "text",
  disabled: false,
  maxlength: undefined,
  dir: "auto",
  labelClass: "",
  inputClass: "",
  readonly: false,
  bordered: false,
  autocomplete: "off",
});

const model = defineModel<string>({
  default: "",
});

const { handleChange, handleBlur, meta } = useField(props.name, undefined, {
  initialValue: model.value,
});

watch(() => model.value, (newVal) => {
  if (props.type === 'tel' || props.inputmode === 'numeric') {
    model.value = useUtils().convertNumbers2English(newVal);
  } else {
    model.value = newVal;
  }
}, {immediate: true});


</script>

<template>
  <UtilsCustomInputWrapper :key="props.name" :name="props.name">
    <label
      dir="auto"
      class="input flex items-center   gap-2 rounded-2xl !outline-none bg-white"
      :class="[props.labelClass,{'!border border-gray-300' : props.bordered}]"
    >
      <template v-if="$slots.icon">
        <slot name="icon" />
      </template>
      <input
        v-model="model"
        :readonly="readonly"
        :valid="false"
        :type="props.type"
        :placeholder="props.placeholder"
        :maxlength="props.maxlength"
        :disabled="props.disabled"
        :name="props.name"
        :dir="props.dir"
        :autocomplete="props.autocomplete"
        class="grow placeholder:text-right placeholder:text-[#8C8C8C]"
        :class="[
          props.inputClass,
          meta.valid ? '' : '',
          $slots.icon ? 'placeholder:pl-[2rem]' : '',
        ]"
        @change="handleChange"
        @blur="handleBlur"
      />
      <template v-if="$slots.prepend">
        <slot name="prepend" />
      </template>
    </label>
  </UtilsCustomInputWrapper>
</template>
