<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";
import type { IPlanAnswersRequestItem, IGetPlanAnswersRequestFilters } from "~/services/PlanService";

definePageMeta({
  layout: "dashboard",
  auth: true
});
useHead({ title: 'پاسخ‌های پلن' });

const { $api } = useNuxtApp<IApiProvider>();

const filters = ref<IGetPlanAnswersRequestFilters>({
  PlanId: undefined,
  OnlyCompleted: false,
  Search: '',
  PageNumber: 1,
  Count: 20
});

const data = ref<IPlanAnswersRequestItem[] | null>(null);
const totalCount = ref<number | null>(null);

async function fetchData() {
  try {
    useSpinner().renderSpinner();
    const response = await $api.plan.getPlanAnswersRequest(filters.value);
    // Assuming API returns list directly. If wrapped, adjust accordingly
    data.value = (response as any).data?.data?.data ?? response.data;
    totalCount.value = (response as any).data?.data?.totalCount ?? data.value?.length ?? 0;
  } catch (e: any) {
    console.error(e?.message || e);
  } finally {
    useSpinner().hideSpinner();
  }
}

function onSearch() {
  filters.value.PageNumber = 1;
  fetchData();
}

function changePage(page: number) {
  filters.value.PageNumber = page;
  fetchData();
}

onMounted(fetchData);
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>پاسخ‌های پلن</template>
        </BaseNotificationHeader>
      </template>
      <div class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col gap-y-4 p-4">
        <div class="w-full grid grid-cols-1 sm:grid-cols-4 gap-3">
          <input v-model.number="filters.PlanId" type="number" placeholder="PlanId" class="input input-bordered w-full" />
          <label class="flex items-center gap-2">
            <input v-model="filters.OnlyCompleted" type="checkbox" class="checkbox" />
            <span>فقط تکمیل‌شده‌ها</span>
          </label>
          <input v-model="filters.Search" @keyup.enter="onSearch" type="text" placeholder="جستجو" class="input input-bordered w-full" />
          <button class="btn btn-primary" @click="onSearch">جستجو</button>
        </div>

        <div class="w-full flex flex-col gap-3">
          <template v-if="data?.length">
            <div v-for="item in data" :key="item.userPlanId" class="card bg-white shadow p-4">
              <div class="flex flex-wrap items-center justify-between gap-2">
                <div class="font-semibold">{{ item.userFullName }}</div>
                <div class="text-sm">UserId: {{ item.userId }}</div>
                <div class="text-sm">UserPlanId: {{ item.userPlanId }}</div>
                <div class="text-sm">شروع: {{ new Date(item.startDate).toLocaleString() }}</div>
                <div class="text-sm" :class="{ 'text-green-600': item.isCompleted, 'text-amber-600': !item.isCompleted }">
                  {{ item.isCompleted ? 'تکمیل شده' : 'در حال انجام' }}
                </div>
              </div>
              <div class="mt-3">
                <div class="font-medium mb-2">پاسخ‌ها</div>
                <div class="space-y-2">
                  <div v-for="ans in item.answers" :key="ans.planQuestionId" class="border rounded p-2 bg-[#F9FAFB]">
                    <div class="text-sm">{{ ans.questionText }}</div>
                    <div class="text-xs text-gray-600">گزینه: {{ ans.selectedOptionText }} (ID: {{ ans.selectedOptionId }})</div>
                    <div class="text-[11px] text-gray-500" v-if="ans.isMain">سوال اصلی</div>
                  </div>
                </div>
              </div>
            </div>
            <div class="w-full flex items-center justify-center mt-2">
              <UtilsCustomPagination
                :page-number="filters.PageNumber"
                :count="filters.Count"
                :total-count="totalCount"
                @change-page="changePage"
              />
            </div>
          </template>
          <div v-else class="text-sm">موردی یافت نشد</div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>


