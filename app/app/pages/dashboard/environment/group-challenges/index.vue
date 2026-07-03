<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import {ChallengeType, type IChallenge, type IGetChallengeFilters} from "~/services/ChallengeService";

definePageMeta({
  layout: "dashboard",
  auth:true
});
useHead({
  title: 'چالش‌های گروهی'
})

definePageMeta({
  auth: true
})


const {$api} = useNuxtApp<IApiProvider>()
const challengesListFilters = ref<IGetChallengeFilters>({
  pageNumber: 1,
  count: 10,
  search: '',
  type: ChallengeType.Group
})
const challengesList = ref<IChallenge[]>([])
const totalCount = ref(null)


async function getAllChallenges() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.challenge.getAllChallengesByFilter(challengesListFilters.value)
    challengesList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getAllChallenges()

function changePage(page: number) {
  challengesListFilters.value.pageNumber = page
  getAllChallenges()
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 155px">
      <template #header>
        <BaseNotificationHeader>
          <template #title> چالش‌های گروهی </template>
        </BaseNotificationHeader>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start p-2 gap-y-4"
      >
        <div class="w-full grid grid-cols-2 gap-4">
          <template v-if="challengesList.length">
            <LazyDashboardEnvironmentGroupChallengeCard
                v-for="challenge in challengesList"
                :key="challenge.id"
                :challenge="challenge"
            ></LazyDashboardEnvironmentGroupChallengeCard>
            <div class="w-full flex col-span-2 items-center justify-center">
              <UtilsCustomPagination
                  :page-number="challengesListFilters.pageNumber"
                  :count="challengesListFilters.count"
                  :total-count="totalCount"
                  @change-page="changePage"
              />
            </div>
          </template>
          <span v-else>  چالشی یافت نشد</span>


        </div>
        <!--        <div-->
        <!--          class="w-full flex flex-row justify-between items-center p-2 border border-[#00B08B] rounded-lg"-->
        <!--        >-->
        <!--          <span class="text-gray-700 text-sm">تعریف چالش جدید</span>-->
        <!--          <span-->
        <!--            class="w-6 h-6 flex flex-row justify-center items-center rounded-full border-2 border-black"-->
        <!--          >-->
        <!--            <Icon name="icon:plus" color="#000000" size="15" />-->
        <!--          </span>-->
        <!--        </div>-->
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
