<script setup lang="ts">
import type {IUserPlan} from "~/services/PlanService";

interface IProps {
  plan: IUserPlan
}

const props:IProps = defineProps({
  plan: {
    type: Object as PropType<IUserPlan>
  }
})

const isRenderingChoosePartnerDialog = ref(false)
</script>

<template>
  <div
    class="w-full bg-primary rounded-xl p-1 flex items-center justify-between relative custom-course-card-shadow"
  >
      <NuxtImg
          v-if="props.plan.imageUrl"
        :src="props.plan.imageUrl"
        class="w-12 h-12 rounded-full object-cover"
      />
    <NuxtImg
        v-else
        src="/common/no-image.png"
        class="w-12 h-12 rounded-full object-cover"
    />
    <div class="w-full flex flex-col gap-2 px-2">
      <div class="flex justify-between items-center">
        <p class="text-[#EFEFEF] line-clamp-1 ">{{ props.plan.planName }}</p>


      </div>
        <p class="text-[#EFEFEF]">{{ props.plan.planLevel }}</p>
      <div v-if="props.plan.lastAnswerExercises.length" class="flex items-center justify-between">
        <div class="flex items-center ">
          <span
              class="w-7 h-7 rounded-full flex items-center justify-start "
          >
            <Icon name="icon:page" size="20" class="[&_*]:stroke-white" />
          </span>
          <p class="text-sm text-[#EFEFEF] mt-1">{{ props.plan.lastAnswerExercises.length }} بخش</p>
        </div>
        <!-- <div class="dropdown dropdown-left dropdown-top">
          <div tabindex="0" role="button">
            <div class="flex items-end gap-x-1 mb-2">
              <LazyIconsDotIcon class="fill-white"></LazyIconsDotIcon>
            </div>
          </div>
          <ul tabindex="0" class="dropdown-content menu bg-base-100 rounded-box z-[99999] relative w-52 p-2 shadow">
            <li @click="isRenderingChoosePartnerDialog = true">
              <a class="font-bold">
                انتخاب همراه
              </a>
            </li> -->
            <!-- <li><a class="font-bold">
              آرشیو
            </a></li> -->
          <!-- </ul>
        </div> -->
        <slot name="trailing"></slot>
      </div>
      <div  class="w-full flex  items-center gap-1">
        <nuxt-link v-if="props.plan.nextUnansweredQuestion" :to="`/introduction/${props.plan.planId}/${props.plan.userPlanId}`" type="button" class="btn btn-sm w-1/2 bg-white !text-primary ">
          پاسخدهی
        </nuxt-link>

      </div>
    </div>
    <LazyUtilsDialogsChoosePartnerDialog :userPlanId="props.plan.userPlanId" v-model="isRenderingChoosePartnerDialog"></LazyUtilsDialogsChoosePartnerDialog>

  </div>
</template>

<style scoped>
.custom-course-card-shadow {
  box-shadow: 0px -2px 6px 0px rgba(10, 37, 64, 0.35) inset,
    0px 2px 4px -20px rgba(50, 50, 93, 0.25),
    0px 1px 4px -30px rgba(0, 0, 0, 0.3);
}
</style>
