<script setup lang="ts">
import type {ITextOption} from "~/services/StepService";

interface IProps {
  stepOption: ITextOption | null;
}

const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});

</script>

<template>
  <div class="w-full my-6">
    <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>

    <p v-if="!stepOption?.isHtml" style="overflow-wrap: anywhere">
      {{ stepOption.description }}
    </p>
    <p v-else v-html="stepOption?.content"></p>
  </div>
</template>

<!-- Keep the existing styles as they are correct -->
<style scoped>

</style>