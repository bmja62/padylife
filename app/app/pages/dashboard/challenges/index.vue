<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import {ChallengeType, type IChallenge, type IGetChallengeFilters} from "~/services/ChallengeService";

definePageMeta({
  auth: true
})

definePageMeta({
  layout: "dashboard",
});
useHead({
  title: 'چالش‌ها'
})
const {$api} = useNuxtApp<IApiProvider>()
const challengesListFilters = ref<IGetChallengeFilters>({
  pageNumber: 1,
  count: 10,
  search: '',
  type: ChallengeType.Single
})
const challengesList = ref<IChallenge[]>([])
const debounceTimeout = ref(null)
const totalCount = ref(null)

const debouncedSearch = computed({
  get() {
    return challengesListFilters.value.search;
  },
  set(val) {
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }
    debounceTimeout.value = setTimeout(() => {
      challengesListFilters.value.search = val
      challengesListFilters.value.pageNumber = 1
      getAllChallenges()
    }, 600);
  },
});

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
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>چالش‌ها</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <div class="w-full grid grid-cols-12 gap-2">
          <input
              v-model="debouncedSearch"
              class="col-span-12 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
              placeholder="جستجو در چالش‌ها"
          />
<!--          <div class="col-span-3 flex flex-row justify-start gap-2">-->
<!--            <div-->
<!--                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"-->
<!--            >-->
<!--              <Icon name="icon:filter" size="20" class="[&_*]:stroke-black"/>-->
<!--            </div>-->
<!--            <div-->
<!--                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-[#ECEFFF]"-->
<!--            >-->
<!--              <Icon name="icon:sort" size="20" class="[&_*]:stroke-black"/>-->
<!--            </div>-->
<!--          </div>-->
        </div>

        <div class="w-full flex flex-col gap-y-4">
          <template v-if="challengesList.length">
            <LazyDashboardChallengesChallengeCard
                v-for="(challenge, index) in challengesList"
                :key="index"
                :challenge="challenge"
            >
          </LazyDashboardChallengesChallengeCard>
            <div class="w-full flex items-center justify-center">
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
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
