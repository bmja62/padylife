<template>
  <LazyUtilsDropDown v-model:is-open="isOpen">
    <template #title>
      <div @click="isOpen = !isOpen" class="w-full flex flex-row justify-between">
        <span class="text-gray-800 text-base">
          {{ props.item.question }}
        </span>
        <span >
          <Icon
            name="icon:plus"
            color="#000000"
            class="[&_*]:stroke-black [&_*]:fill-black transform transition-all"
            :class="[isOpen ? 'rotate-45 [&_*]:!stroke-primary [&_*]:!fill-primary' : '']"
            size="20"
          />
        </span>
      </div>
    </template>
    <template #content>
      <div class="w-full">
        <p class="text-gray-600 text-sm">
          {{props.item.answer}}
        </p>
      </div>
    </template>
  </LazyUtilsDropDown>
</template>

<script setup lang="ts">
// Variables
const isOpen = ref(false);
const props = defineProps({
  item:{
    type:Object as PropType<object>
  }
})
</script>

<style scoped></style>
