<script setup lang="ts">
interface IProps {
  innerClass?: string;
}

const props = defineProps<IProps>();
</script>

<template>
  <div class="w-full h-full">
    <slot name="header"></slot>
    <div
      class="w-full min-h-[calc(100svh-var(--wrapper-header-height))] h-full bg-[#FFFF] px-5 py-4 pt-6 rounded-t-[32px] sm:rounded-t-[0] flex flex-col"
      :class="[props.innerClass]"
    >
      <slot></slot>
    </div>
  </div>
</template>
