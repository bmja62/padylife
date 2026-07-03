<script setup lang="ts">
interface IProps {
  stepsCount: number;
  currentStep: number;
  isFinished?: boolean;
}

const props = defineProps<IProps>();
</script>
<template>
  <div class="overflow-x-auto">
    <ul class="steps">
      <li
        v-for="(step, index) in props.stepsCount"
        :key="index"
        class="step after:border-4 after:border-white after:!font-bold"
        :class="[
          isFinished
            ? 'after:!bg-[#00ABFB] after:!text-white before:!bg-[#00ABFB]'
            : index <= currentStep
            ? 'after:!bg-[#85D8FF]'
            : 'after:!bg-[#D7D7D7]',
        ]"
      ></li>
    </ul>
  </div>
</template>
