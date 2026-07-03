<script setup lang="ts">
import type {IBlogCategory} from "~/services/BlogCategoryService";
import type {IApiProvider} from "~/models/IApiProvider";

const blogCategories = ref<IBlogCategory[]>([])
const {$api} = useNuxtApp<IApiProvider>()
const blogCategoriesListFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 100,
  search: '',
})
const route = useRoute()
async function getBlogCategories() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.blogCategory.getAllBlogCategories(blogCategoriesListFilters.value)
    blogCategories.value = response.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

getBlogCategories()

</script>

<template>
  <div class="w-full flex flex-row justify-between items-center overflow-x-auto overflow-y-hidden">
    <nuxt-link :to="`/dashboard/environment/category/${blogCategory.id}`"
               class="flex flex-col items-center gap-y-1 min-w-max" v-for="blogCategory in blogCategories"
               :key="blogCategory.id">
      <NuxtImg
          v-if="blogCategory.imageUrl"
          :src="blogCategory.imageUrl"
          class="w-[75px] h-[75px] object-contain"
      />
      <NuxtImg
          v-else
          src="/common/no-image.png"
          class="w-[75px] h-[75px] object-contain"
      />
      <span class="text-gray-700 text-xs "
            :class="{'!text-primary':route?.params?.id == blogCategory.id}">{{ blogCategory.title }}</span>
    </nuxt-link>

  </div>

</template>

<style scoped>

</style>