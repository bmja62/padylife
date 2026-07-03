<script setup lang="ts">
import type {IBlog, IBlogLite} from "~/services/BlogService";

interface IProps {
  blog: IBlogLite
}

const props: IProps = defineProps({
  blog: {
    type: Object as PropType<IBlogLite>
  }
})
</script>

<template>
  <nuxt-link :to="`/dashboard/environment/${props.blog.seoURL}`"

             class="w-full flex flex-row justify-start items-center gap-x-2"
  >
    <div
        class="w-20 h-16 flex flex-row justify-center rounded-[10px] items-center  bg-[#5A5A5A]"
    >
      <NuxtImg v-if="props.blog.image" class="w-full rounded-[10px] h-full object-cover"
               :src="props.blog.image"/>
      <NuxtImg v-else class="w-full rounded-[10px] h-full object-cover" src="/common/no-image.png"/>

    </div>
    <div class="w-full flex flex-col items-start gap-1">
      <strong class="text-base line-clamp-1 text-gray-700">{{ props.blog.title }}</strong>
      <span v-if="props.blog.shortDescription" class="text-sm line-clamp-1 text-gray-800"
      >{{ props.blog.shortDescription }}</span
      >
      <span class="text-xs  badge bg-primary text-white">{{ props.blog.blogCategoryTitle }}</span>
    </div>
  </nuxt-link>
</template>

<style scoped>

</style>