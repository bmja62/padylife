<script lang="ts" setup>
import type {IUserSession} from "~/services/UsersService";
import {CommentType} from "~/models/Enums/CommentType";
import RequestConsultantDialog from "~/components/utils/Dialogs/RequestConsultantDialog.vue";

definePageMeta({
  layout: "dashboard",
  auth:true
});
useHead({
  title:'معرفی متخصص'
})
const specialistInfo = ref<IUserSession>(null)
const staticFAQs = [
  {
    id: 1,
    question: "چطور بفهمم این برنامه واقعاً به درد من میخوره؟",
    answer: "در جلسه اول با هم بررسی می‌کنیم که چقدر این مسیر می‌تونه نیازهای تو رو پوشش بده."
  },
  {
    id: 2,
    question: "اگر وسط برنامه دچار تردید شدم چیکار کنم؟",
    answer: "در جلسات خصوصی مشکل رو دقیق بررسی می‌کنیم و راهکارهای عملی بهت می‌دم."
  },
  {
    id: 3,
    question: "این برنامه چقدر طول می‌کشه تا نتیجه بگیرم؟",
    answer: "بستگی به خودت داره! معمولاً با همراهی من پیشرفت محسوسی خواهی داشت."
  },
  {
    id: 4,
    question: "چطور می‌تونم تضمین کنم این هزینه رو به خودم مدیونم؟",
    answer: "اولین جلسه رو اون جوری که میخوای برگزار می‌کنم تا مطمئن بشی این مسیر به دردت می‌خوره یا نه."
  }
]
const route = useRoute()
const {$api} = useNuxtApp()
const isRenderingSubmitRateAndCommentDialog = ref(false)
const completeCompanionsCount = ref(null)
const isRenderingRequestConsultantDialog = ref(false)
const entityComments = ref([])
const averageRating = ref(null)
const entityCommentFilters = ref({
  entityId: route.params.id,
  entityType: CommentType.Specialist,
  pageNumber: 1,
  count: 100
})

async function getSpecialistCompanionsCount() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getExpertCompanionCount(route.params.id, true)
      completeCompanionsCount.value = response.data.count
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}
async function getUserDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.users.getUserById(route.params.id)
    specialistInfo.value = response.data
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
      entityType: CommentType.Specialist,
    })
    averageRating.value = response.data.avg
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

getUserDetail()
getEntityComments()
getAverageRating()
getSpecialistCompanionsCount()
</script>

<template>
  <div v-if="specialistInfo" class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title> معرفی متخصص </template>
        </BaseNotificationHeader>
      </template>

      <div
        class="w-full h-auto bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <NuxtImg
            v-if="specialistInfo?.profileImage"
            :src="specialistInfo.profileImage"
            class="w-full rounded-[10px]"
        />
        <NuxtImg
            v-else
            src="/common/no-image.png"
            class="w-full rounded-[10px]"
        />

        <div class="w-full flex flex-row justify-between items-center">
          <span class="text-gray-800 text-base">{{ specialistInfo.fullName }}</span>
          <span class="text-gray-800 text-base">{{ specialistInfo.jobTitle }}</span>
        </div>

        <div
          class="w-full flex flex-col rounded-[16px] border border-[#6D6D6D] p-4 gap-y-2"
        >
          <!--          <div class="w-full flex flex-row justify-start items-center gap-3">-->
          <!--            <Icon name="icon:location-new" color="#01CED1" size="20" />-->
          <!--            <span class="text-gray-700 text-sm">تهران, تهران</span>-->
          <!--          </div>-->

          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:well-done-new" color="#00ABFB" size="20" />
            <span class="text-gray-700 text-sm">{{ completeCompanionsCount }} همراهی موفق</span>
          </div>

          <div class="w-full flex flex-row justify-start items-center gap-3">
            <Icon name="icon:star-outline-new" color="#FFBE5B" size="20" />
            <span class="text-gray-700 text-sm">{{ averageRating }} ({{ entityComments.length }} نظر)</span>
          </div>

          <!--          <div class="w-full flex flex-row justify-start items-center gap-3">-->
          <!--            <span class="w-5 h-5 rounded-full bg-primary"></span>-->
          <!--            <span class="text-gray-700 text-sm"-->
          <!--              >میانگین زمان پاسخ‌دهی (4 ساعت)</span-->
          <!--            >-->
          <!--          </div>-->

          <!--          <div class="w-full flex flex-row justify-start items-center gap-3">-->
          <!--            <span class="w-5 h-5 rounded-full bg-primary"></span>-->
          <!--            <span class="text-gray-700 text-sm">لورم ایپسوم</span>-->
          <!--          </div>-->
        </div>

        <button @click="isRenderingRequestConsultantDialog = true" type="button" class="btn btn-primary w-full !rounded-full">
          درخواست همراهی
        </button>

<!--        <CoursesDescriptionBox-->
<!--          second-box-class="bg-[#ffff]"-->
<!--          :is-rendering-question="false"-->
<!--        >-->
<!--          (باکس صحبت‌های همراه) لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از-->
<!--          صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه-->
<!--          و مجله-->
<!--        </CoursesDescriptionBox>-->

        <div class="w-full flex flex-col gap-y-2">
          <div
            class="w-full flex flex-row justify-start items-center py-2 border-b-2 border-gray-300"
          >
            <strong class="text-black text-xl">درباره همراهت بیشتر بدون</strong>
          </div>
          <p>
            اگر احساس می‌کنی در زندگی سرگردانی و احساس سردرگمی می‌کنی، اگر نمی‌دانی چه مسیری واقعاً مناسب تو هست و
            استعدادها و علایق واقعی‌ات را به خوبی نمی‌شناسی، اینجا می‌توانم کمکت کنم.

            این همراهی به تو کمک می‌کند خودت را بهتر بشناسی، توانایی‌های واقعی‌ات را کشف کنی و در نهایت مسیری روشن و
            مناسب برای زندگی‌ات پیدا کنی. در تمام این مدت پشتیبان تو هستم و هیچ‌وقت تنها نمی‌گذارمت.

            اگر دوست داری بیشتر بدانی یا برای جلسه اول وقت بگذاری، می‌توانیم از همین امروز شروع کنیم.
          </p>
          <!--          <div-->
          <!--              v-for="(item, idx) in staticFAQs"-->
          <!--            :key="idx"-->
          <!--            class="w-full border-gray-300"-->
          <!--              :class="{ 'border-b-2': idx  !== staticFAQs.length-1 }"-->
          <!--          >-->
          <!--            <LazyDashboardFAQDropDown :item="item"/>-->
          <!--          </div>-->
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
    <RequestConsultantDialog :user-id="specialistInfo.id" :plan-id="route.query.planId" v-model="isRenderingRequestConsultantDialog"></RequestConsultantDialog>

    <LazyUtilsDialogsSubmitRateAndCommentDialog :entityId="specialistInfo.id" label="متخصص"
                                                :comment-type="CommentType.Specialist"
                                                v-model="isRenderingSubmitRateAndCommentDialog"></LazyUtilsDialogsSubmitRateAndCommentDialog>
  </div>
</template>

<style scoped></style>
