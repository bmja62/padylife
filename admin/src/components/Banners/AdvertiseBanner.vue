<template>
  <div   :style="[props.height]" :to="props.link" class="w-100 cursor-pointer  rounded-xl bg-teal-darken-1">
    <img :style="[props.height]" :class="[{'object-cover':props.imageFit},{'object-contain':!props.imageFit}]" :src="props.image"
         alt="" class="w-100 rounded-xl">
  </div>
</template>

<script lang="ts" setup>
import type PropType from "vue";

const props = defineProps({
  height: {
    type: String as PropType<string>,
    default: 'h-full'
  },
  link: {
    type: String as PropType<string>,
    required: false
  },
  image: {
    type: String as PropType<string>,
    required: false,
    default: '/Capture.png'
  },

  imageFit: {
    type: Boolean,
    default: true
  }
})

</script>

<style scoped>
.object-cover {
  object-fit: cover;
}

.object-contain {
  object-fit: cover;
}
</style>
