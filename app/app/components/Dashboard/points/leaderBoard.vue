<script setup lang="ts">
import type {ILeaderboardUser} from "~/services/LeaderBoardService";

interface IProps {
  leaderboard: ILeaderboardUser[]
}

const props: IProps = defineProps({
  leaderboard: {
    type: Array as PropType<ILeaderboardUser[]>
  }
})
const selectedTab = defineModel()
const authStore = useAuthStore()
function getFilteredLeaders(rank: number): ILeaderboardUser {
  return props.leaderboard.find(e => e.rank === rank)
}

</script>
<template>
  <div class="w-full flex flex-col ">
    <div class="w-full mb-8 flex items-center justify-between">
      <div class="w-1/2 text-center border-b-2 border-gray-300"
           :class="{'!text-primary !border-primary':selectedTab===1}" @click="selectedTab = 1">کاربران
      </div>
      <div class="w-1/2 text-center border-b-2 border-gray-300"
           :class="{'!text-primary !border-primary':selectedTab===2}" @click="selectedTab = 2"> متخصصان
      </div>
    </div>
    <div class="w-full flex flex-row justify-between items-center pb-14">
      <div class="flex flex-col items-center gap-y-1 transform translate-y-10">
        <div class="relative">
          <NuxtImg
              v-if="getFilteredLeaders(3)?.profileImage"
              :src="getFilteredLeaders(3)?.profileImage"
              class="w-[70px] h-[70px] object-contain rounded-full border-2 border-black"
          />
          <NuxtImg
              v-else
              src="/common/no-image.png"
              class="w-[70px] h-[70px] object-contain rounded-full border-2 border-black"
          />
          <span
            class="w-6 h-6 bg-[#FFAD38] absolute -bottom-2 rounded-full flex flex-row justify-center items-center text-white text-xs left-1/2 transform -translate-x-1/2"
            >3</span
          >
        </div>
        <span class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(3)?.userName }}</span>
        <span class="text-gray-600 text-xs">{{ getFilteredLeaders(3)?.totalPoints }} امتیاز</span>
      </div>
      <div class="flex flex-col items-center gap-y-1">
        <div class="relative">
          <Icon
            name="icon:crown"
            class="absolute left-1/2 -translate-x-1/2 -top-4"
            color="#FFE138"
          />
          <NuxtImg
              v-if="getFilteredLeaders(1)?.profileImage"
              :src="getFilteredLeaders(1)?.profileImage"
            class="w-[80px] h-[80px] object-contain rounded-full border-2 border-black"
          />
          <NuxtImg
              v-else
              src="/common/no-image.png"
              class="w-[80px] h-[80px] object-contain rounded-full border-2 border-black"
          />
          <span
            class="w-6 h-6 bg-[#FFE138] absolute -bottom-2 rounded-full flex flex-row justify-center items-center text-white text-xs left-1/2 transform -translate-x-1/2"
            >1</span
          >
        </div>
        <span class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(1).userName }}</span>
        <span class="text-gray-600 text-xs">{{ getFilteredLeaders(1).totalPoints }} امتیاز</span>
      </div>
      <div class="flex flex-col items-center gap-y-1 transform translate-y-10">
        <div class="relative">

          <NuxtImg
              v-if="getFilteredLeaders(2)?.profileImage"
              :src="getFilteredLeaders(2)?.profileImage"
            class="w-[70px] h-[70px] object-contain rounded-full border-2 border-black"
          />
          <NuxtImg
              v-else
              src="/common/no-image.png"
              class="w-[70px] h-[70px] object-contain rounded-full border-2 border-black"
          />
          <span
            class="w-6 h-6 bg-[#FFAD38] absolute -bottom-2 rounded-full flex flex-row justify-center items-center text-white text-xs left-1/2 transform -translate-x-1/2"
            >2</span
          >
        </div>
        <span class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(2).userName }}</span>
        <span class="text-gray-600 text-xs">{{ getFilteredLeaders(2).totalPoints }} امتیاز</span>
      </div>
    </div>
    <div class="w-full flex flex-col gap-y-2">
      <!--      .filter((e,idx)=> {return idx>2})-->
      <div
          v-for="leader in leaderboard"
          :key="leader.userId"
          :class="{'!bg-primary !text-white [&_*]:!fill-white':leader.userId === authStore.getUser.id}"
        class="w-full rounded-[12px] text-[#212121]  bg-[#ffff] flex flex-row justify-between items-center p-3 custom-leaderboard-shadow"
      >
        <div class="flex flex-row justify-start items-center gap-x-2">
          <NuxtImg
              v-if="leader.profileImage"
              :src="leader.profileImage"
            class="w-[20px] h-[20px] object-contain rounded-full"
          />
          <NuxtImg
              v-else
              src="/common/no-image.png"
              class="w-[20px] h-[20px] object-contain rounded-full"
          />
          <span class="text-sm font-bold">{{ leader.userId === authStore.getUser.id ? 'شما' : leader.userName }}</span>
        </div>
        <div class="flex flex-row justify-end items-center gap-x-2">
          <Icon :name="leader.totalPoints ?`icon:diamond` :`icon:star`" color="#212121" size="20"/>
          <span class="text-sm">{{ leader.totalPoints ? leader.totalPoints : leader.avgRate }}</span>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts"></script>

<style scoped>
.custom-leaderboard-shadow {
  box-shadow: 0px 2px 6px rgba(60, 64, 67, 0.15),
    0px 1px 2px rgba(60, 64, 67, 0.3);
}
</style>
