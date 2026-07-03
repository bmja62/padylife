<script setup lang="ts">
import {type IChallenge} from "~/services/ChallengeService";

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
  <nuxt-link :title="props.challenge.title"
             :to="`/dashboard/challenges/${props.challenge.id}`"
             class="w-full flex flex-col items-center gap-y-2 border border-[#00B08B] rounded-lg p-2"
  >
    <NuxtImg v-if="props.challenge.imageUrl" class="w-full rounded-[10px] h-20 object-cover"
             :src="props.challenge.imageUrl"/>
    <NuxtImg v-else class="w-full rounded-[10px] h-20 object-cover" src="/common/no-image.png"/>

    <strong class="text-gray-800 text-center line-clamp-1">{{ props.challenge.title }}</strong>

    <!--    <span class="text-gray-500">52/100</span>-->
  </nuxt-link>
</template>

<style scoped>

</style>