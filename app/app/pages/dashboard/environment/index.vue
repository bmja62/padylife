<script setup lang="ts">

import type {IApiProvider} from "~/models/IApiProvider";
import type {IBlog} from "~/services/BlogService";

const {$api} = useNuxtApp<IApiProvider>()

definePageMeta({
  layout: "dashboard",
  auth:true
});
useHead({
  title:'محیط زیست'
})
const authStore = useAuthStore()
const blogsListFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: '',
})
const blogs = ref<IBlog[]>([])
const totalCount = ref(null)



async function getAllBlogs() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.blog.getAllBlogs(blogsListFilters.value)
    blogs.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

function changePage(page: number) {
  blogsListFilters.value.pageNumber = page
  getAllBlogs()
}
getAllBlogs()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 260px">
      <template #header>
        <BaseNotificationHeader>
          <template #title> محیط زیست </template>
        </BaseNotificationHeader>
        <div class="w-full flex flex-col items-center gap-y-2 pb-4">
          <NuxtImg src="/junks/user.png" class="w-[80px] h-[80px]" />
          <strong class="text-white text-sm">{{ authStore.getUser.fullName }}</strong>
          <div class="w-full grid grid-cols-2">
<!--            <div class="flex flex-col items-center gap-y-2">-->
<!--              <span class="text-sm text-white">ردپای کربن</span>-->
<!--              <span class="text-[#EFEFEF] text-sm">8</span>-->
<!--            </div>-->
            <div
              class="flex flex-col items-center gap-y-2 border-r border-l border-gray-300"
            >
              <span class="text-sm text-white">امتیاز</span>
              <span class="text-[#EFEFEF] text-sm">22</span>
            </div>
            <div class="flex flex-col items-center gap-y-2">
              <span class="text-sm text-white">چالش‌ها</span>
              <span class="text-[#EFEFEF] text-sm">11</span>
            </div>
          </div>
        </div>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
<!--        <div class="w-full flex flex-row justify-center items-center gap-x-2">-->
<!--          <strong class="text-gray-800">ردپای کربنی من</strong>-->
<!--          <span-->
<!--            class="flex flex-row justify-center items-center bg-green-500 text-white w-10 h-10 p-1 rounded-full"-->
<!--            >8</span-->
<!--          >-->
<!--        </div>-->

<!--        <NuxtImg-->
<!--          src="/junks/environment.png"-->
<!--          class="w-full h-[15rem] object-contain"-->
<!--        />-->

        <LazySharedEnvironmentCategories></LazySharedEnvironmentCategories>
        <NuxtLink
          to="/dashboard/environment/group-challenges"
          class="w-full relative !rounded-lg"
        >
          <NuxtImg
            src="/junks/chalenge.jpg"
            class="w-full h-[120px] rounded-lg object-cover"
          />
          <div
            class="w-full h-full absolute top-0 left-0 flex flex-row justify-center items-center"
          >
            <strong class="text-white">چالش‌های گروهی</strong>
          </div>
        </NuxtLink>

        <div class="w-full flex flex-row justify-between items-center">
          <span class="text-gray-800">اخبار زیست محیطی</span>
          <nuxt-link to="/dashboard/environment/all" class="text-gray-700 text-xs">مشاهده همه</nuxt-link>
        </div>

        <div v-if="blogs.length" class="w-full flex flex-col gap-y-4">
          <LazyDashboardEnvironmentBlogCard v-for="blog in blogs"
                                            :key="blog.id" :blog="blog"></LazyDashboardEnvironmentBlogCard>
          <div class="w-full flex items-center justify-center">
            <UtilsCustomPagination
                :page-number="blogsListFilters.pageNumber"
                :count="blogsListFilters.count"
                :total-count="totalCount"
                @change-page="changePage"
            />
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
