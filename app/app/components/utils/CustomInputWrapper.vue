<script setup lang="ts">
import { ErrorMessage as VeeErrorMessage } from "vee-validate";
interface IProps {
  name: string;
}

const props = defineProps<IProps>();
</script>

<template>
  <div class="w-full ">
    <slot />
    <VeeErrorMessage
      v-slot="{ message }"
      :name="props.name"
      as="span"
      class="text-sm !text-red-700 my-2 flex items-center justify-start gap-x-2"
    >
      <Icon
        name="icon:exclamination-circle"
        class="shake-animation transition-all size-5"
      />
      {{ message }}
    </VeeErrorMessage>
  </div>
</template>
<style></style>
