<script lang="ts" setup>
import type {IApiProvider} from "~/models/IApiProvider";
import {CommentType} from "~/models/Enums/CommentType";
import type {IBlogDetail} from "~/services/BlogService";

definePageMeta({
  layout: "dashboard",
  auth: true
});

const route = useRoute()
const blogDetail = ref<IBlogDetail>(null)
const {$api} = useNuxtApp<IApiProvider>()
const isRenderingSubmitRateAndCommentDialog = ref(false)
const entityComments = ref([])
const averageRating = ref(null)
const entityCommentFilters = ref({
  entityId: null,
  entityType: CommentType.Blog,
  pageNumber: 1,
  count: 100
})

async function getBlogBySeoUrl() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.blog.getBlogByseourl(route.params.seourl)
    blogDetail.value = response.data
    entityCommentFilters.value.entityId = blogDetail.value.id

  } catch (error) {
    console.error(error.message);
  } finally {
    getAverageRating()
    getEntityComments()

    useSpinner().hideSpinner()
  }
}

async function getEntityComments() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.comment.getEntityComments(entityCommentFilters.value)
    entityComments.value = response.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getAverageRating() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.rate.getAverageRating({
      entityId: entityCommentFilters.value.entityId,
      entityType: CommentType.Blog,
    })
    averageRating.value = response.data.avg
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

  getBlogBySeoUrl()

</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image" v-if="blogDetail">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>{{ blogDetail.title }}</template>
        </BaseNotificationHeader>
      </template>

      <div
          class="w-full h-auto bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <NuxtImg v-if="blogDetail.mainImageUrl" class="w-full rounded-[10px] max-h-[200px] object-cover"
                 :src="blogDetail.mainImageUrl"/>
        <NuxtImg v-else class="w-full rounded-[10px] max-h-[200px] object-cover" src="/common/no-image.png"/>

        <div class="w-full flex flex-row justify-between items-center">
          <span class="text-gray-800 text-base">{{
              blogDetail.author.fullName ? blogDetail.author.fullName : 'نویسنده'
            }}</span>
          <span class="text-gray-800 text-base">{{
              new Date(blogDetail.createdAt).toLocaleDateString('fa-IR')
            }}</span>
        </div>
        <div
            class="w-full flex flex-col rounded-[16px] border border-[#6D6D6D] p-4 gap-y-2"
        >
          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:clock-circle-v1" color="#01CED1" size="20"/>
            <span class="text-gray-700 text-sm">{{ blogDetail.spendTimeForRead }}</span>
          </div>


          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:well-done-new" color="#00ABFB" size="20"/>
            <span class="text-gray-700 text-sm">{{ blogDetail.blogCategoryTitle }} </span>
          </div>

          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:star-outline-new" color="#FFBE5B" size="20"/>
            <span class="text-gray-700 text-sm">{{ averageRating }} ({{ entityComments.length }} نظر)</span>
          </div>
        </div>

        <div class="w-full flex flex-col gap-y-2">
          <div
              class="w-full flex flex-row justify-start items-center py-2 border-b-2 border-gray-300"
          >
            <p style="overflow-wrap: anywhere">{{ blogDetail.shortDescription }}</p>
          </div>

          <p v-html="blogDetail.content" style="overflow-wrap: anywhere"></p>
        </div>
        <div class="w-full flex flex-col gap-y-2">
          <div
              class="w-full flex flex-row justify-start items-center py-2 border-b-2 border-gray-300"
          >
            <div class="w-full flex items-center justify-between">
              <strong class="text-black text-xl">نظرات</strong>
              <button type="button" @click="isRenderingSubmitRateAndCommentDialog = true"
                      class="btn btn-sm bg-transparent border border-primary !text-primary ">
                ثبت نظر
              </button>
            </div>
          </div>
          <div class="w-full flex flex-col gap-4">
            <LazySharedCommentCard v-for="comment in entityComments" :key="comment.id" @refetch="getEntityComments"
                                   :comment="comment"></LazySharedCommentCard>
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsSubmitRateAndCommentDialog :entityId="blogDetail.id" label="مقاله"
                                                :comment-type="CommentType.Blog"
                                                v-model="isRenderingSubmitRateAndCommentDialog"></LazyUtilsDialogsSubmitRateAndCommentDialog>
    <LazyUtilsUseMyMeta v-if="blogDetail" :page-data="blogDetail" :seoMeta="{
  ogLocale: 'fa_IR',
  ogType: 'article',
  ogTitle: `${blogDetail.ogTitle}`,
  ogImageUrl:blogDetail.ogMainPicUrl,
  ogDescription:blogDetail.seoDescription,
  articleModifiedTime:blogDetail.createdAt,
  ogUrl: blogDetail.ogurl,
  ogSiteName: 'پادی لایف',
}"></LazyUtilsUseMyMeta>
  </div>
</template>

<style scoped></style>
