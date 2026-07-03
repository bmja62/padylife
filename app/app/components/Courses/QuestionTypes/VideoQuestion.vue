<script setup lang="ts">
import type { IVideoOption } from "~/services/StepService";
import { ref, computed, onMounted, watchEffect } from 'vue';

const videoOption = useTemplateRef('videoOption');
const isPaused = ref(false);
const currentTime = ref(0);
const duration = ref(0);

interface IProps {
  stepOption: IVideoOption | null;
}

const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});

function doAction(action: string) {
  videoOption.value[action]();
  isPaused.value = !isPaused.value;
}

function fastForward() {
  videoOption.value.currentTime = Math.max(0, videoOption.value.currentTime + 10);
}

function rewind() {
  videoOption.value.currentTime = Math.max(0, videoOption.value.currentTime - 10);
}

function handleTimeUpdate() {
  currentTime.value = videoOption.value.currentTime;
}

function handleLoadedMetadata() {
  duration.value = videoOption.value.duration;
}

function handleInput(e: Event) {
  const target = e.target as HTMLInputElement;
  const value = parseFloat(target.value);
  currentTime.value = value;
  videoOption.value.currentTime = value;
}

const formattedCurrentTime = computed(() => {
  return formatTime(currentTime.value);
});

const formattedDuration = computed(() => {
  return formatTime(duration.value);
});

function formatTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60);
  const remainingSeconds = Math.floor(seconds % 60);
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
}

onMounted(() => {
  if (videoOption.value) {
    videoOption.value.addEventListener('timeupdate', handleTimeUpdate);
    videoOption.value.addEventListener('loadedmetadata', handleLoadedMetadata);
  }
});
onBeforeUnmount(()=>{
  videoOption.value = null;
})
</script>

<template>
  <div class="w-full my-6">
      <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>
    <div
        class="w-full h-[200px] rounded-[32px] bg-white border border-[#00ABFB] flex items-center justify-center"
    >

      <video
          ref="videoOption"
          class="w-full h-full rounded-[32px]"
          :src="props.stepOption?.videoUrl"
          :poster="props.stepOption?.thumbnailUrl ? props.stepOption?.thumbnailUrl : '/common/no-image.png'"
          @timeupdate="handleTimeUpdate"
          @loadedmetadata="handleLoadedMetadata"
      ></video>
    </div>
    <div class="px-7 mt-5">
      <input
          v-if="duration > 0"
          :value="currentTime"
          type="range"
          @input="handleInput"
          min="0"
          :max="Math.ceil(duration)"
          dir="ltr"
          class="w-full "
          :style="`--progress:${(currentTime / Math.ceil(duration)) * 100}%`"
      />
    </div>
    <div class="w-full flex items-center justify-between px-2">
      <p>{{ formattedCurrentTime }}</p>
      <p>{{ formattedDuration }}</p>
    </div>
    <div class="flex items-center justify-center gap-x-3">
      <Icon size="40" name="icon:proceed-new" @click="fastForward" color="white"/>
      <Icon v-if="!isPaused" @click="doAction('play')" size="40" name="icon:custom-play-new"/>
      <Icon v-else @click="doAction('pause')" size="40" name="icon:custom-pause"/>
      <Icon size="40" name="icon:rewind-new" @click="rewind"/>
    </div>
  </div>
</template>

<!-- Keep the existing styles as they are correct -->
<style scoped>
input[type="range"] {
  -webkit-appearance: none; /* Remove default styling */
  appearance: none;
  width: 100%; /* Full width */
  height: 5px; /* Adjust height as needed */
  background: linear-gradient(
      to right,
      #97d7ff var(--progress),
      #ecefff var(--progress)
  ); /* Progress bar */
  border-radius: 12px; /* Rounded corners */
  outline: none;
}

input[type="range"]::-webkit-slider-thumb {
  -webkit-appearance: none; /* Remove default styling */
  appearance: none;
  width: 14px; /* Thumb width */
  height: 14px; /* Thumb height */
  background: #00abfb; /* Thumb color */
  border-radius: 50%; /* Circular thumb */
  cursor: pointer;
  box-shadow: 0 0 2px rgba(0, 0, 0, 0.5); /* Optional shadow */
}

input[type="range"]::-moz-range-thumb {
  width: 14px; /* Thumb width */
  height: 14px; /* Thumb height */
  background: #00abfb; /* Thumb color */
  border-radius: 50%; /* Circular thumb */
  cursor: pointer;
  box-shadow: 0 0 2px rgba(0, 0, 0, 0.5); /* Optional shadow */
}

input[type="range"]::-moz-range-track {
  background: linear-gradient(
      to right,
      #97d7ff var(--progress),
      #ecefff var(--progress)
  ); /* Progress bar */
  border-radius: 12px; /* Rounded corners */
}
</style>