<template>
  <div
    :class="[
      isOpen ? 'rounded-lg' : 'rounded-lg',
      `bg-[${props.bgColor}]`,
    ]"
    class="w-full flex flex-col p-4"
  >
    <div class="w-full">
      <slot name="title"></slot>
    </div>
    <Transition mode="in-out" name="slide">
      <div v-show="isOpen" class="w-full transition duration-300">
        <slot name="content"></slot>
      </div>
    </Transition>
  </div>
</template>

<script lang="ts" setup>
// Variables
const isOpen = defineModel("isOpen");

// Props
const props = defineProps({
  bgColor: {
    default: "#fffff",
  },
});
</script>

<style scoped>
.slide-enter-active {
  animation: slide-down 2s ease-out;
  overflow: hidden;
}

.slide-leave-active {
  animation: slide-up 0.3s ease-out;
  overflow: hidden;
}

@keyframes slide-down {
  0% {
    display: hidden;
    max-height: 0px;
  }
  100% {
    display: block;
    max-height: 400px;
  }
}

@keyframes slide-up {
  0% {
    display: block;
    max-height: 100px;
  }
  100% {
    display: hidden;
    max-height: 0px;
  }
}
</style>
