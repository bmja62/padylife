<script setup lang="ts">
import {ChallengeType, type IChallenge} from "~/services/ChallengeService";

interface IProps {
  challenge: IChallenge
}

const props: IProps = defineProps({
  challenge: {
    type: Object as PropType<IChallenge>
  }
})
</script>
<template>
  <div
      class="w-full flex flex-col gap-y-5 rounded-[20px] bg-white border border-[#E0E4E8] p-4"
  >
    <NuxtImg v-if="props.challenge.imageUrl" class="w-full rounded-[10px] max-h-[200px] object-cover"
             :src="props.challenge.imageUrl"/>
    <NuxtImg v-else class="w-full rounded-[10px] max-h-[200px] object-cover" src="/common/no-image.png"/>

    <div class="w-full flex flex-row justify-between items-center">
      <h3 class="text-gray-800 font-semibold">
        {{ props.challenge.title }}
      </h3>
      <div class="flex flex-row justify-center items-center gap-2">
        <Icon v-if="props.challenge.type === ChallengeType.Group" name="icon:comments-v1" size="20"/>
        <Icon v-if="props.challenge.type === ChallengeType.Single" name="icon:user-v2" size="20"/>
        <span class="text-black">
          <slot name="city"> {{ props.challenge.type === ChallengeType.Group ? 'گروهی' : 'فردی' }} </slot>
        </span>
      </div>
    </div>
    <div class="w-full flex flex-row justify-between items-center">

<!--      <div class="flex flex-row justify-center items-start gap-1">-->
<!--        <span class="text-xs text-gray-400"-->
<!--        >(-->
<!--          <slot name="commentsCount"> 125 </slot>-->
<!--          )</span-->
<!--        >-->
<!--        <span class="text-xs text-[#FFBE5B]">-->
<!--          <slot name="rate"> 4.7 </slot>-->
<!--        </span>-->
<!--        <Icon name="icon:star" class="[&_*]:fill-[#FFBE5B]" size="15"/>-->
<!--      </div>-->
    </div>
    <div class="w-full grid grid-cols-1 gap-2 justify-between items-center">
      <nuxt-link
          :to="`/dashboard/challenges/${props.challenge.id}`"
          class="w-full btn !rounded-xl btn-primary btn-outline font-normal"
      >
        مشاهده چالش
      </nuxt-link>

    </div>
  </div>
</template>
