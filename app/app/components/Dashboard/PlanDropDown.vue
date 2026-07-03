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
          class="w-full flex cursor-pointer flex-row justify-between items-center">
        <div class="flex flex-row justify-start items-center gap-x-2">
          <NuxtImg v-if="props.plan.imageUrl" :src="props.plan.imageUrl" class="w-8 h-8 rounded-full object-cover"></NuxtImg>
          <NuxtImg v-else src="/common/no-image.png" class="w-8 h-8 rounded-full object-cover"></NuxtImg>
          <strong class="text-gray-800 text-sm">{{ props.plan.title }}</strong>
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
      <div class="w-full">
        <p v-html="props.plan.description" class="text-gray-700 text-justify my-2">
        </p>
        <div v-if="props.plan.price" class="flex items-center p-2 justify-between">
          <span class="text-gray-400">
            قیمت
          </span>
          <div class="flex flex-col">
          <span
              :class="{'!text-gray-400 line-through':props.plan.discountPrice}">{{
              Intl.NumberFormat('fa-IR').format(props.plan.price)
            }} تومان</span>
            <span
                v-if="props.plan.discountPrice">{{
                Intl.NumberFormat('fa-IR').format(props.plan.finalPrice)
              }} تومان</span>
          </div>
        </div>
        <button type="button" @click="isRenderingPurchasePlanDialog = true" class="bg-primary btn w-full text-white rounded-full">
          خرید
        </button>
    <LazyUtilsDialogsPurchasePlanDialog :plan="props.plan" v-model="isRenderingPurchasePlanDialog"></LazyUtilsDialogsPurchasePlanDialog>
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
