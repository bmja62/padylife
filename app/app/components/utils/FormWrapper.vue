<script lang="ts" setup generic="T">
import { Form as VeeForm } from "vee-validate";
import type { ObjectSchema, AnyObject } from "yup";

interface IProps {
  schema: ObjectSchema<AnyObject>;
  errorMessages?: Record<string, unknown>;
}

const props = defineProps<IProps>();

const emits = defineEmits<{
  (e: "submit"): void;
}>();

function submitForm() {
  emits("submit");
}
function onInvalidSubmit(e){
  console.log(e)
}
</script>
<template>
  <VeeForm
    :initial-errors="errorMessages"
    :validation-schema="props.schema"
    @submit="submitForm"
    @invalid-submit="onInvalidSubmit"
  >
    <slot />
  </VeeForm>
</template>
