<template>
  <div class="w-full grid grid-cols-3 gap-4">
    <div v-for="item in medalsList" :key="item.id" class="flex flex-col relative items-center gap-1">
      <div v-if="item.isLocked"
           class="w-full h-full bg-black/50 rounded z-[2] flex items-center justify-center absolute top-0 left-0">
        <Icon
            name="icon:lock"
            class="[&_*]:stroke-white"
            size="50"
        />
      </div>
      <NuxtImg :src="item.iconUrl" class="w-full custom-medal-shadow z-[1]"/>
      <span>{{ item.title }}</span>
    </div>
    <div class="col-span-3 flex items-center justify-center">
      <UtilsCustomPagination
          :page-number="medalsFilters.pageNumber"
          :count="medalsFilters.count"
          :total-count="totalCount"
          @change-page="changePage"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IMedal} from "~/services/LeaderBoardService";

const {$api}: IApiProvider = useNuxtApp()
const medalsFilters = ref({
  pageNumber: 1,
  count: 10
})
const medalsList = ref<IMedal[]>([])
const totalCount = ref(null)
const authStore = useAuthStore()
function changePage(page: number) {
  medalsFilters.value.pageNumber = page
  getAllMedals()
}

async function checkAndAssignMedals() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.leaderboard.checkAndAssignMedals()
    if (response.isSuccess) {
      await authStore.fetchUser()
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getAllMedals() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.leaderboard.getAllMedals(medalsFilters.value)
    medalsList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

await Promise.all([
  checkAndAssignMedals(),
  getAllMedals()
])


</script>

<style scoped>
.custom-medal-shadow {
  filter: drop-shadow(0px 1px 2px rgba(60, 64, 67, 0.3))
    drop-shadow(0px 2px 6px rgba(60, 64, 67, 0.15));
}
</style>
