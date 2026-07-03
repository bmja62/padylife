<script lang="ts" setup>
import { ChallengeType, type IChallenge } from "~/services/ChallengeService";
import type { IApiProvider } from "~/models/IApiProvider";
import { CommentType } from "~/models/Enums/CommentType";

definePageMeta({
  layout: "dashboard",
  auth: true
});
useHead({
  title: 'معرفی چالش'
})
const route = useRoute()
const challengeDetail = ref<IChallenge>(null)
const { $api } = useNuxtApp<IApiProvider>()
const isRenderingSubmitRateAndCommentDialog = ref(false)
const entityComments = ref([])
const averageRating = ref(null)
const entityCommentFilters = ref({
  entityId: route.params.id,
  entityType: CommentType.Challenge,
  pageNumber: 1,
  count: 100
})

async function getChallengeById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.challenge.getChallengeById(route.params.id)
    challengeDetail.value = response.data
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function participateInChallenge() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.challenge.participate(route.params.id)
    if (response.isSuccess) {
      getChallengeById()
      isRenderingSubmitRateAndCommentDialog.value = true
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
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
      entityId: route.params.id,
      entityType: CommentType.Challenge,
    })
    averageRating.value = response.data.avg
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

await Promise.all([
  getChallengeById(),
  getEntityComments(),
  getAverageRating()
])
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image" v-if="challengeDetail">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>{{ challengeDetail.title }}</template>
        </BaseNotificationHeader>
      </template>

      <div class="w-full h-auto bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4">
        <NuxtImg v-if="challengeDetail.imageUrl" class="w-full rounded-[10px] max-h-[200px] object-cover"
          :src="challengeDetail.imageUrl" />
        <NuxtImg v-else class="w-full rounded-[10px] max-h-[200px] object-cover" src="/common/no-image.png" />

        <div class="w-full flex flex-row justify-between items-center">
          <span class="text-gray-800 text-base">نوع چالش</span>
          <span class="text-gray-800 text-base">{{
            challengeDetail.type === ChallengeType.Group ? 'گروهی' : 'فردی'
          }}</span>
        </div>

        <div class="w-full flex flex-col rounded-[16px] border border-[#6D6D6D] p-4 gap-y-2">


          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:well-done-new" color="#00ABFB" size="20" />
            <span class="text-gray-700 text-sm">{{ challengeDetail.participantCount }} همراهی موفق</span>
          </div>

          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:star-outline-new" color="#FFBE5B" size="20" />
            <span class="text-gray-700 text-sm">{{ Math.round(averageRating) }} ({{ entityComments.length }} نظر)</span>
          </div>
        </div>

        <button v-if="!challengeDetail.hasParticipantByMe" type="button" class="btn btn-primary w-full !rounded-full"
          @click="participateInChallenge">
          چالش را انجام دادم ( 1 امتیاز )
        </button>
        <LazySharedShareBtn></LazySharedShareBtn>


        <!--        <CoursesDescriptionBox-->
        <!--            second-box-class="bg-[#ffff]"-->
        <!--            :is-rendering-question="false"-->
        <!--        >-->
        <!--          (باکس صحبت‌های همراه) لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از-->
        <!--          صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه-->
        <!--          و مجله-->
        <!--        </CoursesDescriptionBox>-->

        <div class="w-full flex flex-col gap-y-2">
          <div class="w-full flex flex-row justify-start items-center py-2 border-b-2 border-gray-300">
            <strong class="text-black text-xl">توضیحات</strong>
          </div>

          <p v-html="challengeDetail.description" style="overflow-wrap: anywhere"></p>
        </div>
        <div class="w-full flex flex-col gap-y-2">
          <div class="w-full flex flex-row justify-start items-center py-2 border-b-2 border-gray-300">
            <strong class="text-black text-xl">نظرات افرادی که این چالش را انجام داده اند</strong>
          </div>
          <div class="w-full flex flex-col gap-4">
            <template v-if="entityComments.length">
              <LazySharedCommentCard v-for="comment in entityComments" :key="comment.id" @refetch="getEntityComments"
                :comment="comment"></LazySharedCommentCard>
            </template>
            <LazyUtilsEmptyView v-else empty-text="نظری یافت نشد" />
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsSubmitRateAndCommentDialog :entityId="challengeDetail.id" label="چالش"
      :comment-type="CommentType.Challenge" v-model="isRenderingSubmitRateAndCommentDialog">
    </LazyUtilsDialogsSubmitRateAndCommentDialog>
  </div>
</template>

<style scoped></style>
