<template>
  <div
    class="w-full flex flex-row justify-between items-center bg-white rounded-[16px] p-2"
  >
    <div class="flex flex-col items-start gap-y-2">
      <template v-if="$slots.title">
        <span class="text-gray-500 text-sm">
          <slot name="title"></slot>
        </span>
      </template>
      <template v-if="$slots.subtitle">
        <span class="text-gray-700 text-sm">
          <slot name="subtitle"></slot>
        </span>
      </template>
    </div>
    <div class="h-full flex flex-row justify-center items-center mt-4">
      <slot name="icon"></slot>
    </div>
  </div>
</template>
