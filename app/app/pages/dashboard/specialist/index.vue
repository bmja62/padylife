<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});
useHead({
  title: 'اطلاعات حساب کاربری'
})
const {$api} = useNuxtApp()
const router = useRouter();
const authStore = useAuthStore()
const incompleteCompanionsCount = ref(null)
const completeCompanionsCount = ref(null)
function redirectToCreateNewCourse() {
  window.location.href = 'https://admin.padylife.com/'
  router.push("/dashboard/specialist/my-courses/add");
}

function redirectToMessages() {
  router.push("/dashboard/notifications");
}

async function getSpecialistCompanionsCount(isComplete) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getExpertCompanionCount(null, isComplete)
    if (isComplete) {
      completeCompanionsCount.value = response.data.count

    } else {

      incompleteCompanionsCount.value = response.data.count
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

await getSpecialistCompanionsCount(false)
await getSpecialistCompanionsCount(true)
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 160px">
      <template #header>
        <DashboardUserDashboardHeader user-role="specialist">
          <template #title>
            <p v-if="authStore.isLogged" class="text-white">خوش اومدی {{  authStore.getUser.fullName ? authStore.getUser.fullName : '' }} </p>
          </template>
        </DashboardUserDashboardHeader>

        <div v-if="authStore.isLogged" class="w-full flex flex-col items-center gap-y-2 pb-4">
          <NuxtImg
              v-if="authStore?.getUser?.profileImage"
              :src="authStore.getUser.profileImage"
              class="w-[80px] h-[80px] rounded-full object-cover"
          />
          <NuxtImg
              v-else
              src="/common/no-image.png"
              class="w-[80px] h-[80px] rounded-full object-cover"
          />
          <p class="text-white text-sm">{{ authStore.getUser.jobTitle ? authStore.getUser.jobTitle : '-' }}</p>
        </div>
      </template>

      <div class="w-full grid grid-cols-2 gap-4">
        <UtilsStatsCard class="col-span-2">
          <template #title>
            <span class="text-[#636363] text-sm">امتیاز</span>
          </template>
          <template #subtitle>
            <span class="text-black text-sm"> {{ authStore?.getUserPoints?.earnedPoints }} </span>
          </template>
          <template #icon>
            <NuxtImg class="w-7 h-7 object-contain" src="/core/diamond.png" />
          </template>
        </UtilsStatsCard>

        <!--        <UtilsStatsCard>-->
        <!--          <template #title>-->
        <!--            <span class="text-[#636363] text-sm">درجه</span>-->
        <!--          </template>-->
        <!--          <template #subtitle>-->
        <!--            <span class="text-black text-sm"> 2 </span>-->
        <!--          </template>-->
        <!--          <template #icon>-->
        <!--            <NuxtImg-->
        <!--              class="w-[30px] h-[30px] object-contain"-->
        <!--              src="/junks/gold-trophy.png"-->
        <!--            />-->
        <!--          </template>-->
        <!--        </UtilsStatsCard>-->

        <!--        <UtilsStatsCard class="col-span-2">-->
        <!--          <template #title>-->
        <!--            <span class="text-[#5C5C5C] text-sm">تبریک!</span>-->
        <!--          </template>-->
        <!--          <template #subtitle>-->
        <!--            <span class="text-black text-sm">-->
        <!--              تو این هفته 19 ساعت برای بهتر شدن حال بقیه تلاش کردی :)-->
        <!--            </span>-->
        <!--          </template>-->
        <!--          <template #icon>-->
        <!--            <Icon size="24" name="icon:clock-circle-v1" />-->
        <!--          </template>-->
        <!--        </UtilsStatsCard>-->

        <UtilsStatsCard>
          <template #title>
            <span class="text-[#636363] text-sm">تکمیل شده</span>
          </template>
          <template #subtitle>
            <span
                class="text-black text-sm"> {{ completeCompanionsCount ? completeCompanionsCount : '-' }} همراهی </span>
          </template>
          <template #icon>
            <NuxtImg
              class="w-[30px] h-[30px] object-contain"
              src="/junks/dart-done.png"
            />
          </template>
        </UtilsStatsCard>

        <UtilsStatsCard>
          <template #title>
            <span class="text-[#636363] text-sm">در حال همراهی</span>
          </template>
          <template #subtitle>
            <span class="text-black text-sm"> {{
                incompleteCompanionsCount ? incompleteCompanionsCount : '-'
              }} نفر </span>
          </template>
          <template #icon>
            <NuxtImg
              class="w-[30px] h-[30px] object-contain"
              src="/junks/dart.png"
            />
          </template>
        </UtilsStatsCard>

        <!--        <UtilsStatsCard class="col-span-2">-->
        <!--          <template #subtitle>-->
        <!--            <p class="text-[#545454] text-sm tracking-wide">-->
        <!--              تو از-->
        <!--              <span class="text-black"> 43% </span>-->
        <!--              همراهان اپلیکیشن-->
        <!--              <span class="text-black"> بهتر </span>-->
        <!--              عمل کردی.-->
        <!--            </p>-->
        <!--          </template>-->
        <!--          <template #icon>-->
        <!--            <NuxtImg-->
        <!--              class="w-[45px] h-[45px] object-contain"-->
        <!--              src="/junks/happy.png"-->
        <!--            />-->
        <!--          </template>-->
        <!--        </UtilsStatsCard>-->

        <UtilsStatsCard class="flex-col" @click="redirectToMessages">
          <template #title>
            <p class="text-[#545454] text-sm">
              <span class="text-black"> {{ authStore.getNotificationCount }} </span>
              پیام
              <span class="text-black"> خوانده‌نشده </span>
              داری.
            </p>
          </template>
          <template #icon>
            <NuxtImg
              src="/junks/message.png"
              class="object-contain w-14 h-auto"
            />
          </template>
        </UtilsStatsCard>

<!--        <UtilsStatsCard class="flex-col" @click="redirectToCreateNewCourse">-->
<!--          <template #title>-->
<!--            <p class="text-black text-sm">ساخت دوره جدید</p>-->
<!--          </template>-->
<!--          <template #icon>-->
<!--            <NuxtImg src="/junks/page.png" class="object-contain w-14 h-auto" />-->
<!--          </template>-->
<!--        </UtilsStatsCard>-->
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>
