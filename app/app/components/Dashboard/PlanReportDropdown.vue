<script setup lang="ts">

import type {IPlan} from "~/services/PlanService";

interface IProps {
  plan: IPlan;
}

const props = defineProps<IProps>();

const isOpen = ref<boolean>(false);
const isRenderingPurchasePlanDialog = ref<boolean>(false);

// Props

function toggleOpen() {
  isOpen.value = !isOpen.value;
}

</script>

<template>
  <LazyUtilsDropDown v-model:is-open="isOpen" class="custom-card-shadow bg-white">
    <template #title>
      <div
          @click="toggleOpen"
          class="w-full flex flex-row justify-between items-center">
        <div class="flex flex-row justify-start items-center gap-x-2">
          <NuxtImg v-if="props.plan.planImageUrl" :src="props.plan.planImageUrl"
                   class="w-8 h-8 rounded-full object-cover"></NuxtImg>
          <NuxtImg v-else src="/common/no-image.png" class="w-8 h-8 rounded-full object-cover"></NuxtImg>
          <strong class="text-gray-800 text-sm">{{ props.plan.planName }}</strong>
        </div>
        <div
            class="flex flex-row justify-start items-center gap-x-2">

          <button
              type="button"
              :class="{'bg-primary !border-primary':isOpen}"
              class="rounded-full w-6 h-6 border-2 transition-all border-gray-700 flex flex-row justify-center items-center p-1"
          >
            <Icon
                name="icon:plus"
                color="#374151"
                class="[&_*]:fill-gray-700 transition-all"
                :class="[{'[&_*]:!fill-white transform rotate-45 ':isOpen}]"
                size="10"
            />
          </button>
        </div>
      </div>
    </template>
    <template #content>
      <div class="w-full flex flex-col gap-3 py-3">
        <div class="w-full flex items-center px-1 justify-between">
          <small class="font-bold">تعداد همراهان</small>
          <small>{{ props.plan.personCount }}</small>
        </div>
        <div class="w-full flex items-center px-1 justify-between">
          <small class="font-bold">میانگین سنی همراهان</small>
          <small>{{ props.plan.averageAge }} سال</small>
        </div>
        <div class="w-full flex  flex-col  px-1">
          <small class="font-bold">جنسیت همراهان</small>
          <div class="w-full flex items-center justify-center gap-3 py-4">
            <div class="flex flex-col items-center justify-center gap-2">
              <div class="radial-progress radial text-primary" :style="`--value:${props.plan.womanGender};`"
                   style="--size:4rem" role="progressbar">{{ props.plan.womanGender }}%
              </div>
              <small>زن</small>
            </div>
            <div class="flex flex-col  items-center justify-center gap-2">
              <div class="radial-progress radial text-blue-500" :style="`--value:${props.plan.manGender};`"
                   style="--size:4rem" role="progressbar">{{ props.plan.manGender }}%
              </div>
              <small>مرد</small>
            </div>
          </div>
        </div>
        <div class="w-full flex  flex-col px-1">
          <small class="font-bold">میانگین همراهانی که برنامه را به اتمام رسانده‌اند</small>
          <div class="w-full flex items-center justify-center gap-3 py-4">
            <div class="radial-progress radial text-third"
                 :style="`--value:${props.plan.percentageOfPeopleWhoCompletedThePlan};`" style="--size:4rem"
                 role="progressbar">
              {{ props.plan.percentageOfPeopleWhoCompletedThePlan }}%
            </div>
          </div>
        </div>

      </div>
    </template>
  </LazyUtilsDropDown>
</template>

<style scoped>
.custom-card-shadow {
  box-shadow: 0px 1px 2px 0px rgba(60, 64, 67, 0.3),
  0px 2px 6px 2px rgba(60, 64, 67, 0.15);
}
</style>
