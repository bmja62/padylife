<script setup lang="ts">

import {type IDailyFeeling} from "~/services/DailyFeelings";

interface IProps {
  dailyFeeling: IDailyFeeling
}

const props: IProps = defineProps({
  dailyFeeling: {
    type: Object as PropType<IDailyFeeling>
  }
})
const moodMap = {
  Glad: {label: 'عالی', emoji: 'new-better', color: '#1DF576'},
  Happy: {label: 'خوشحال', emoji: 'new-good', color: '#70B2FF'},
  Poker: {label: 'پوکر', emoji: 'new-meh', color: '#FFD900'},
  Sad: {label: 'ناراحت', emoji: 'new-sad', color: '#FF9500'},
  Bad: {label: 'خشمگین', emoji: 'new-sadder', color: '#FF2D2D'},
}

const moodInfo = computed(() => moodMap[props.dailyFeeling.type])
const renderAudio = ref(false)

const formattedDate = computed(() => {
  const date = new Date(props.dailyFeeling.createdAt)
  return date.toLocaleDateString('fa-IR', {weekday: 'long', day: 'numeric', month: 'long'})
})
</script>

<template>
  <div class="bg-orange-50 rounded-2xl  p-4 shadow-md mb-6 space-y-4">
    <!-- Header: Mood Type & Date -->
    <div class="w-full flex  relative">
    <div  v-for="i in 10" :style="`left: ${i*28}px`"  class="absolute w-2 h-8 bg-gray-400 border border-gray-400 rounded-lg -top-6"></div>
    </div>
    <div class="flex justify-between items-center text-sm text-gray-600">

      <Icon
          :name="`feelings:${moodInfo.emoji}`"
          size="35"
          class="w-10 h-10"
          color="#4C4C4C"
      />
      <span>{{ formattedDate }}</span>
    </div>

    <!-- Description -->
    <p class="text-gray-800 leading-relaxed whitespace-pre-wrap">
      {{ props.dailyFeeling.description }}
    </p>

    <!-- Voice Note -->
    <div v-if="props.dailyFeeling.voiceUrl">
      <span
          class="inline-block cursor-pointer bg-yellow-300 text-gray-800 font-semibold px-4 py-2 rounded-full transition hover:bg-yellow-400"
          @click="renderAudio = true"
      >
        🎤 پخش صدا
      </span>
    </div>
    <audio v-if="renderAudio" class="w-full" :src="props.dailyFeeling.voiceUrl" controls></audio>

    <!-- Footer: Author Info -->
    <div class="text-left text-xs text-gray-500">
      {{ props.dailyFeeling.userInfo.fullName }} <span
        v-if="props.dailyFeeling.userInfo.age">• {{ props.dailyFeeling.userInfo.age }} ساله</span>
    </div>
  </div>
</template>

<style scoped>

</style>