<script setup lang="ts">
import type {IExerciseDetail} from "~/services/ExerciseService";
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPlanSubscriber} from "~/services/PlanService";

interface IUserAvatar {
  color: string;
}

interface IRadarUserAvatar extends IUserAvatar {
  top: number;
  left: number;
  imageUrl: string;
  width: number;
  height: number;
}

definePageMeta({
  auth: true

});
useHead({
  title:'معرفی تمرین'
})

const {$api}: IApiProvider = useNuxtApp()
const router = useRouter();
const route = useRoute()
const authStore = useAuthStore()
const planSubscribers = ref<IPlanSubscriber[]>([])
const visibleCount = 6
function goBack() {
  router.back(-1)
}

function startCourse() {
  router.push(`/dashboard/courses/${route.params.userPlanId}/exercises/${route.params.exerciseId}/detail/${currentExercise.value.exerciseStepsDTOs[0].stepId}`);
}

const mainUserAvatars = ref<IRadarUserAvatar[]>([
  {
    color: "#00617599",
    top: 20,
    left: 75,
    width: 49,
    height: 49,
    imageUrl: "/junks/avatars/one.png",
  },
  {
    color: "#0061758f",
    top: 25,
    left: 15,
    width: 25,
    height: 25,
    imageUrl: "/junks/avatars/two.png",
  },
  {
    color: "#006175",
    top: 55,
    left: 15,
    width: 49,
    height: 49,
    imageUrl: "/junks/avatars/three.png",
  },
  {
    color: "#00617554",
    top: 80,
    left: 25,
    width: 31,
    height: 31,
    imageUrl: "/junks/avatars/four.png",
  },
  {
    color: "#00617524",
    top: 75,
    left: 75,
    width: 49,
    height: 49,
    imageUrl: "/junks/avatars/five.png",
  },
  {
    color: "#0061754d",
    top: 50,
    left: 85,
    width: 31,
    height: 31,
    imageUrl: "/junks/avatars/six.png",
  },
]);

const otherUserAvatars = ref<IUserAvatar[]>([
  {
    color: "#1D87AA",
  },
  {
    color: "#50429A",
  },
  {
    color: "#0DF8DC",
  },
  {
    color: "#FFCB14",
  },
]);

const currentExercise = ref<IExerciseDetail>(null)

async function getCurrentExerciseDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.exercises.getExerciseById(route.params.exerciseId)
    currentExercise.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

function getPlanIdFromAuthorizedUser() {
  const idx = authStore.getUser.userPlans.findIndex(e => e.id == route.params.userPlanId)
  if (idx > -1) {
    return authStore.getUser.userPlans[idx].planId
  }
}

async function getPlanSubscribers() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanSubscribers({
      planId: getPlanIdFromAuthorizedUser() ? getPlanIdFromAuthorizedUser() : null,
      pageNumber: 1,
      count: 100
    })
    planSubscribers.value = response.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

onMounted(async () => {
  await authStore.fetchUser()

  getCurrentExerciseDetail()
  getPlanSubscribers()
})

const isSubscribersMoreThanVisibleCount = computed(() => {
  if (planSubscribers?.value?.length) {
    return planSubscribers.value.length > visibleCount
  }
})
const getFilteredSubscribers = computed(() => {
  if (isSubscribersMoreThanVisibleCount.value) {
    return planSubscribers.value.filter((e, idx) => idx < visibleCount)
  } else {
    return planSubscribers.value
  }
})
</script>

<template>
  <div
      class="w-full h-full custom-pattern-bg-image overflow-y-auto overflow-x-hidden"
  >
    <BaseMinimalHeader @go-back="goBack"></BaseMinimalHeader>
    <div

        class="w-full bg-[#F7F8FE] px-5 py-4 rounded-t-[32px]"
    >
      <p
          v-if="planSubscribers.length"
          class="px-5 pt-[10px] text-justify">
        میدونستی همزمان با تو، {{ planSubscribers.length }} نفر دیگه هم دارن این تمرین رو انجام میدن؟
      </p>
      <div
          v-if="planSubscribers.length"
          class="w-full flex items-center justify-center relative">
        <CoursesCourseRadar :user="planSubscribers[0]"></CoursesCourseRadar>
        <!--        <div-->
        <!--            v-for="(user, index) in planSubscribers"-->
        <!--            :key="index"-->
        <!--            class="rounded-full flex items-center justify-center absolute"-->
        <!--            :style="`top:${20}%; left: ${20}%; width: ${50}px; height: ${50}px;`"-->
        <!--        >-->
        <!--          <NuxtImg-->
        <!--              v-if="user.imageUrl"-->
        <!--              :src="user.imageUrl"-->
        <!--              class="w-full h-full object-cover rounded-full"-->
        <!--          />-->
        <!--          <NuxtImg-->
        <!--              v-else-->
        <!--              src="/common/no-image.png"-->
        <!--              class="w-full h-full object-cover rounded-full"-->
        <!--          />-->
        <!--          &lt;!&ndash; <Icon size="25" name="icon:user-new" :color="avatar.color" /> &ndash;&gt;-->
        <!--        </div>-->
      </div>
      <div class="-mt-5 flex items-center justify-center !z-50">
        <div class="avatar-group -space-x-5 rtl:space-x-reverse">
          <div
              v-for="(avatar, index) in getFilteredSubscribers"
              :key="index"
              class="w-8 h-8 avatar bg-white rounded-full flex items-center justify-center"
          >
            <NuxtImg
                v-if="avatar.imageUrl"
                :src="avatar.imageUrl"
                class="w-5 h-5 rounded-full object-cover"
            ></NuxtImg>
            <NuxtImg
                v-else
                src="/common/no-image.png"
                class="w-5 h-5 rounded-full object-cover"
            ></NuxtImg>
          </div>
          <div v-if="isSubscribersMoreThanVisibleCount" class="avatar placeholder w-8">
            <div class="bg-white text-black text-xs">
              <span>+{{ planSubscribers.length - visibleCount }}</span>
            </div>
          </div>
        </div>
      </div>
      <div class="w-full   relative py-10 ">
        <div class="w-full h-[16rem]  flex items-end justify-between">
<LazyUtilsInstagramHearts class="!w-1/3 flex pb-10"> </LazyUtilsInstagramHearts>
          <div class="flex flex-col gap-2 items-end h-[17rem] w-2/3 overflow-y-scroll">
            <div
                v-for="(avatar, index) in planSubscribers"
                :key="index"
                class="w-full flex items-center justify-end gap-2">
              <span>{{avatar.userFullName}}</span>
              <NuxtImg
                  v-if="avatar.imageUrl"
                  :src="avatar.imageUrl"
                  class="w-8 h-8 rounded-full object-cover"
              ></NuxtImg>
              <NuxtImg
                  v-else
                  src="/common/no-image.png"
                  class="w-8 h-8 rounded-full object-cover"
              ></NuxtImg>
            </div>
          </div>
        </div>

        <button
            type="button"
            class="w-full btn btn-primary bottom-0 sticky  !rounded-[28px] mt-5"
            @click="startCourse"
        >
          شروع
        </button>
      </div>
    </div>
  </div>
</template>

<style>
.test {
  color: #0061754d;
}
</style>
