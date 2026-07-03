<script setup lang="ts">
interface IProps {
  imageUrl: string;
}

const props = defineProps<IProps>();
</script>

<template>
  <div class="w-full h-1/3 flex items-center justify-center">
    <img :src="props.imageUrl" class="w-auto h-auto object-contain" />
  </div>
</template>
