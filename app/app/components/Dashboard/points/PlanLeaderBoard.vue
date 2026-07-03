<script setup lang="ts">
import type {ILeaderboardUser} from "~/services/LeaderBoardService";
import type {IApiProvider} from "~/models/IApiProvider";

interface IProps {
  planId: number | string
}

const {$api}: IApiProvider = useNuxtApp()

const props: IProps = defineProps({
  planId: {
    type: [Number, String] as PropType<number | string>
  }
})
const authStore = useAuthStore()
const leaderBoard = ref([])

function getFilteredLeaders(rank: number): ILeaderboardUser {
  const idx = leaderBoard.value.findIndex(e => e.rank === rank)
  if (idx > -1) {
    return leaderBoard.value[idx]
  }
}

async function getPlanLeaderboard() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.leaderboard.getPlanLeaderboard(12, props.planId)
    leaderBoard.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

await getPlanLeaderboard()

</script>
<template>
  <div v-if="leaderBoard.length" class="w-full flex flex-col ">
    <div class="w-full flex flex-row justify-between items-center pb-14">
      <div  class="flex flex-col items-center gap-y-1 transform translate-y-10">
        <div v-if="getFilteredLeaders(3)" class="relative">
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
        <span v-if="getFilteredLeaders(3)" class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(3)?.fullName }}</span>
        <span v-if="getFilteredLeaders(3)" class="text-gray-600 text-xs">{{ getFilteredLeaders(3)?.totalPoints }} امتیاز</span>
      </div>
      <div class="flex flex-col items-center gap-y-1">
        <div v-if="getFilteredLeaders(1)"  class="relative">
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
        <span v-if="getFilteredLeaders(1)" class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(1).fullName }}</span>
        <span v-if="getFilteredLeaders(1)" class="text-gray-600 text-xs">{{ getFilteredLeaders(1).totalPoints }} امتیاز</span>
      </div>
      <div class="flex flex-col items-center gap-y-1 transform translate-y-10">
        <div  v-if="getFilteredLeaders(2)"  class="relative">

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
        <span  v-if="getFilteredLeaders(2)" class="text-gray-800 font-bold mt-1 line-clamp-1"
              style="overflow-wrap: anywhere">{{ getFilteredLeaders(2).fullName }}</span>
        <span  v-if="getFilteredLeaders(2)" class="text-gray-600 text-xs">{{ getFilteredLeaders(2).totalPoints }} امتیاز</span>
      </div>
    </div>
    <div class="w-full flex flex-col gap-y-2">
      <!--      .filter((e,idx)=> {return idx>2})-->
      <div
          v-for="leader in leaderBoard"
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
          <span class="text-sm font-bold">{{ leader.userId === authStore.getUser.id ? 'شما' : leader.fullName ? leader.fullName : '-'}}</span>
        </div>
        <div class="flex flex-row justify-end items-center gap-x-2">
          <Icon :name="`icon:diamond`" color="#212121" size="20"/>
          <span class="text-sm">{{ leader.totalPoints }}</span>
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
