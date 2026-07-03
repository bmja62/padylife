<script setup lang="ts">
import type {IExerciseDetail} from "~/services/ExerciseService";
import {CommentType} from "~/models/Enums/CommentType";
import {RateType} from "~/services/RateService";
import type {IUserPlanInfo} from "~/services/PlanService";

interface IProps {
  currentExercise: IExerciseDetail
}

const props: IProps = defineProps({
  currentExercise: {
    type: Object as PropType<currentExercise>
  },

})
const isRenderingExerciseReviewsDialog = ref(false)
const commentDescription = ref('')
const rate = ref(1)
const commented = ref(false)
const rated = ref(false)
const {$api} = useNuxtApp()
const alert = useAlerts()
const router = useRouter();
const route = useRoute();
const authStore = useAuthStore()
const planInfo = ref<IUserPlanInfo>(null)

function goToCoursesPage() {
  router.push(`/dashboard`);
}

async function completeExercise() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.completeExerciseForUser({
      userPlanId: route.params.userPlanId,
      userId: authStore.getUser.id,
      exerciseId: props.currentExercise.id,
    })
    if (response.isSuccess) {

    } else {
      alert.success('شما قبلا این تمرین را به پایان رسانده اید')
    }
    goToCoursesPage()

  } catch (e) {
    console.error(e)
  } finally {
    checkIsLastExercise()
    useSpinner().hideSpinner()
  }

}

function checkIsLastExercise() {
  const idx = planInfo.value.exercises.findIndex(e => e.exerciseId == route.params.exerciseId)
  if (idx > -1) {
    if (idx === planInfo.value.exercises.length - 1) {
      completeUserPlan()
    }
  }
}
async function createComment() {
  if (commentDescription.value) {
    try {
      useSpinner().renderSpinner()
      const response = await $api.comment.createComment({
        entityId: route.params.exerciseId,
        entityType: CommentType.Excersie,
        text: commentDescription.value,
        parentCommentId: null
      })
      if (response.isSuccess) {
        alert.success('نظرت با موفقیت ثبت شدش و پس از تایید به نمایش درمیاد')
        commented.value = true
      }else{
        alert.error(response.message)
      }
    } catch (e) {
      console.error(e)
    } finally {
      useSpinner().hideSpinner()
    }
  } else {
    alert.error('لطفا متن نظر خودت رو وارد کن')
  }
}

async function createRating() {
    try {
      useSpinner().renderSpinner()
      const response = await $api.rate.createRating({
        entityId: route.params.exerciseId,
        ratingValue:rate.value,
        entityType:RateType.Excersie,
      })
      if (response.isSuccess) {
        rated.value = true
      }else{
        alert.error(response.message)
      }
    } catch (e) {
      console.error(e)
    } finally {
      useSpinner().hideSpinner()
    }

}

async function getExerciseById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getUserPlanExercises(route.params.userPlanId)
    planInfo.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}


async function completeUserPlan() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.completePlanForUser(route.params.userPlanId)
    if (response.isSuccess) {
      useAlerts().success('تبریک ! موفق شدی این پلن رو به اتمام برسونی.')
      goToCoursesPage()

    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
async function getMyRatingStats() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.rate.getMyRatingStats({
      entityId: route.params.exerciseId,
      entityType: CommentType.Excersie,
    })

    if (response.isSuccess) {
      if(response.data.isRated){
        rated.value = true
      }
    } else {
      alert.error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}
watch(()=>rate.value,async (val)=>{
  createRating()
})

getMyRatingStats()
getExerciseById()
</script>


<template>
  <!--<p style="overflow-wrap: anywhere" v-html="currentExercise.exerciseGoal"></p>-->

  <div class="w-full min-h-[calc(100vh-200px)] flex flex-col justify-between">
    <div class="flex flex-col ">

      <h3 class="text-black text-center mt-7 text-xl">
      این برنامه رو با موفقیت به اتمام رسوندی
    </h3>
    <div class="flex flex-col mt-4 gap-3">

      <button @click="isRenderingExerciseReviewsDialog = true" type="button"
              class="btn border !border-primary bg-white !text-primary ">
        مشاهده نظرات هم‌مسیر ها
      </button>
      <template v-if="!commented">

        <strong>میتونی نظر خودت رو در مورد این تمرین به اشتراک بذاری</strong>
      <textarea
          v-model="commentDescription"
          class="textarea textarea-bordered rounded-[4px] w-full"
          placeholder="نظر خودت رو اینجا نویس"
      ></textarea>
        <button @click="createComment" type="button" class=" w-1/5 btn bg-primary text-white border-none">
        ثبت
      </button>

      </template>
      <template v-if="!rated">
        <strong>از 1 تا 5 به این تمرین چه امتیازی میدی ؟</strong>
        <ClientOnly>
          <StarRating
              v-model:rating="rate" class="xl:order-1 order-2" :show-rating="false" :star-size="20"
              :increment="1"></StarRating>
        </ClientOnly>
      </template>
    </div>
    </div>

    <div class="flex flex-col w-full self-end gap-2">
      <div class="w-full finish-page-background">
        <span>هدف تمرین</span>
      <p class="text-black leading-10 text-xl text-justify" v-html="props.currentExercise.exerciseGoal">
      </p>

    </div>
    <button
      type="button"
      class="btn w-full btn-primary !rounded-[28px] mt-auto"
      @click="completeExercise"
    >
      تایید
    </button>
    </div>
    <LazyUtilsDialogsExerciseMainReviewsDialog
        v-model="isRenderingExerciseReviewsDialog"
    ></LazyUtilsDialogsExerciseMainReviewsDialog>
  </div>
</template>

<style scoped>
.finish-page-background {
  background-image: url("/junks/dashboard-user-bg.png");
  background-repeat: no-repeat;
  background-position: center center;
}
</style>
