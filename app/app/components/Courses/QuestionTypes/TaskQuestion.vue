<script setup lang="ts">
import type {IActionOption, ITaskOption} from "~/services/StepService";

interface IProps {
  stepOption: ITaskOption | null;
}

const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});

</script>

<template>
  <div class="w-full my-6">
    <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>
    <div class="flex flex-col gap-4">

      <div class="flex items-center justify-between">
        <strong>هدف از انجام این تکلیف :</strong>
        <span>{{ stepOption.assigneeRole }}</span>
      </div>
      <div class="flex items-center justify-between">
        <strong>زمان مورد نیاز روزانه :</strong>
        <span>{{ stepOption.estimatedMinutes }} دقیقه</span>
      </div>
      <div class="flex items-center justify-between">
        <strong>زمان انجام تکلیف :</strong>
        <span>{{ stepOption.deadlineDays }} روز</span>
      </div>
    </div>
    <div class="divider"></div>
    <p v-html="stepOption?.taskInstructions"></p>
  </div>
</template>

<!-- Keep the existing styles as they are correct -->
<style scoped>

</style>