<script setup lang="ts">
import medals from "@/components/Dashboard/points/medals.vue";
import leaderBoard from "@/components/Dashboard/points/leaderBoard.vue";
import padyClub from "@/components/Dashboard/points/padyClub.vue";
import type {IApiProvider} from "~/models/IApiProvider";
import type {ILeaderboardUser} from "~/services/LeaderBoardService";

definePageMeta({
  layout: "dashboard",
});
useHead({
  title:'امتیازات'
})
const authStore = useAuthStore()
// Variables
const selectedPage = ref(1);
const pages = shallowRef([
  {
    id: 1,
    name: "مدال‌ها",
    component: medals,
  },
  {
    id: 2,
    name: "لیدربورد",
    component: leaderBoard,
  },
  // {
  //   id: 3,
  //   name: "پادی کلاب",
  //   component: padyClub,
  // },
]);
const {$api}: IApiProvider = useNuxtApp()
const selectedTab = ref(1)
// Computed
const isComponent = computed(() => {
  return pages.value[selectedPage.value - 1]!.component;
});
const leaderboards = ref<ILeaderboardUser[]>([])
async function getLeaderBoard() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.leaderboard.getLeaderBoard(12)
    leaderboards.value = response.data.leaders
  } catch (e) {
    console.error(e)
  } finally {
    authStore.fetchUserPoints()
    useSpinner().hideSpinner()
  }
}

async function getExpertLeaderBord() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.leaderboard.getExpertLeaderboard(12)
    leaderboards.value = response.data.leaders
  } catch (e) {
    console.error(e)
  } finally {
    authStore.fetchUserPoints()
    useSpinner().hideSpinner()
  }
}

const findMyUser = computed(() => {
  const idx = leaderboards?.value?.findIndex((e: ILeaderboardUser) => e.userId === authStore.getUser.id)
  if (idx > -1) {
    return leaderboards.value[idx]
  }
})
watch(() => selectedTab.value, async (val) => {
  if (val) {
    if (val == 1) {
      await getLeaderBoard()

    } else {
      await getExpertLeaderBord()
    }
  }
}, {immediate: true})
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 155px">
      <template #header>
        <BaseNotificationHeader></BaseNotificationHeader>
        <div v-if="authStore.isLogged" class="w-full flex flex-col items-center gap-y-2 pb-4">
          <LazySharedHonoredUser></LazySharedHonoredUser>


          <strong class="text-white text-sm">{{
              authStore.getUser.fullName ? authStore.getUser.fullName : authStore.getUser.userName
            }}</strong>
          <div class="w-full grid grid-cols-2 gap-2 gap-x-4 px-4">
            <LazyDashboardWalletCard class="col-span-2"></LazyDashboardWalletCard>

            <div
                class="w-full rounded-[16px]  bg-white border border-gray-200 flex flex-col gap-3 p-2"
            >
              <div class="w-full flex items-center justify-between">
                <span class="text-sm text-gray-600 font-bold">امتیاز</span>
                <nuxt-link to="/dashboard/user/points/details"
                           class="btn btn-sm bg-white flex items-center justify-center [&_*]:fill-primary border-primary !rounded-xl tooltip"
                           data-tip="جزئیات امتیازات">
                  <Icon name="icon:dots" size="2"
                        class=""></Icon>
                </nuxt-link>
              </div>
              <div class="w-full flex items-center justify-between px-1">
                <span class="text-gray-700 text-sm">{{ authStore?.getUserPoints?.availablePoints ? authStore?.getUserPoints?.availablePoints : '-' }}</span>
                <NuxtImg
                    src="/core/diamond.png"
                    class="w-[30px] h-[30px] object-contain"
                />
              </div>
            </div>
            <div
              class="w-full rounded-[16px] bg-white border border-gray-200 flex flex-row justify-between items-center p-2"
            >
              <div class="flex flex-col items-center gap-y-2">
                <span class="text-sm text-gray-600 font-bold">رتبه</span>
                <span class="text-gray-700 text-sm">{{ findMyUser?.rank ? findMyUser?.rank : '-' }}</span>
              </div>
              <NuxtImg
                src="/junks/trophy.png"
                class="w-[30px] h-[30px] object-contain"
              />
            </div>
          </div>
        </div>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start p-2 gap-y-4"
      >
        <div class="w-full flex flex-row justify-between items-center">
          <div
            v-for="page in pages"
            :key="page.id"
            class="flex flex-col w-1/3 cursor-pointer items-center gap-y-2"
            @click="selectedPage = page.id"
          >
            <span
              :class="[
                page.id === selectedPage ? 'text-primary' : 'text-gray-600',
              ]"
              >{{ page.name }}</span
            >
            <span
              :class="[
                page.id === selectedPage ? 'bg-primary' : 'bg-transparent',
              ]"
              class="w-2 h-2 rounded-full"
            ></span>
          </div>
        </div>

        <Component :is="isComponent" :leaderboard="leaderboards" v-model="selectedTab"/>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>
