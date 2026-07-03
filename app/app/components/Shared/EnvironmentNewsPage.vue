<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IBlog, IGetBlogsParams} from "~/services/BlogService";

const {$api} = useNuxtApp<IApiProvider>()
const debounceTimeout = ref(null)
const totalCount = ref(null)
const blogs = ref<IBlog[]>([])
const route = useRoute()
const blogsListFilters = ref<IGetBlogsParams>({
  pageNumber: 1,
  count: 10,
  search: '',
  blogCategoryId: route.params.id ? route.params.id : ''
})

const debouncedSearch = computed({
  get() {
    return blogsListFilters.value.search;
  },
  set(val) {
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }
    debounceTimeout.value = setTimeout(() => {
      blogsListFilters.value.search = val
      blogsListFilters.value.pageNumber = 1

      getAllBlogs()
    }, 600);
  },
});


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
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>اخبار زیست محیطی</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <LazySharedEnvironmentCategories></LazySharedEnvironmentCategories>

        <div class="w-full grid grid-cols-12 gap-2">
          <input
              v-model="debouncedSearch"
              class="col-span-12 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
              placeholder="جستجو در اخبار"
          />
          <!--          <div class="col-span-3 flex flex-row justify-start gap-2">-->
          <!--            <div-->
          <!--                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"-->
          <!--            >-->
          <!--              <Icon name="icon:filter" size="20" class="[&_*]:stroke-black"/>-->
          <!--            </div>-->
          <!--            <div-->
          <!--                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"-->
          <!--            >-->
          <!--              <Icon name="icon:sort" size="20" class="[&_*]:stroke-black"/>-->
          <!--            </div>-->
          <!--          </div>-->
        </div>

        <div class="w-full flex flex-col gap-y-4">
          <template v-if="blogs.length">
            <LazyDashboardEnvironmentBlogCard
                v-for="blog in blogs"
                :key="blog.id" :blog="blog"></LazyDashboardEnvironmentBlogCard>
            <div class="w-full flex items-center justify-center">
              <UtilsCustomPagination
                  :page-number="blogsListFilters.pageNumber"
                  :count="blogsListFilters.count"
                  :total-count="totalCount"
                  @change-page="changePage"
              />
            </div>
          </template>
          <span v-else>  اخباری یافت نشد</span>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>

</template>

<style scoped>

</style>