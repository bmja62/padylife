<script setup lang="ts">
import type {IActionOption} from "~/services/StepService";

interface IProps {
  stepOption: IActionOption | null;
}

const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});

</script>

<template>
  <div class="w-full my-6">
    <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>
    <div class="flex flex-col gap-4">

      <div class="flex items-center justify-between gap-2">
        <strong>نوع تعامل :</strong>
        <span>{{ stepOption.actionCommand }}</span>
      </div>
      <div class="flex flex-col  gap-2">
        <strong>معیار های اصلی تعامل :</strong>
        <span v-html="stepOption.actionParameters"></span>
      </div>
    </div>
    <div class="divider"></div>
    <p v-html="stepOption?.description"></p>
  </div>
</template>

<!-- Keep the existing styles as they are correct -->
<style scoped>

</style>