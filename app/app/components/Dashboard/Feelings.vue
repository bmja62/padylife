<script setup lang="ts">

import {feelingsIconMap, feelingsMap} from "~/services/DailyFeelings";

const currentFeeling = defineModel<Feelings>();

function selectFeeling(selectedFeeling: Feelings) {
  currentFeeling.value = selectedFeeling;
}

function isFeelingSelected(selectedFeeling: Feelings) {
  return currentFeeling.value == selectedFeeling;
}

onMounted(() => {
  currentFeeling.value = feelingsMap[0].type
})
</script>

<template>
  <div class="w-5/6 mx-auto grid justify-items-center grid-cols-5 mt-4">
    <button
        v-for="item in feelingsMap"
      type="button"
      class="p-1"
      :class="
        isFeelingSelected(item.type)
          ? 'border border-primary rounded-[10px]'
          : 'border border-transparent'
      "
        @click="selectFeeling(item.type)"
    >

      <Icon
          :name="`feelings:${feelingsIconMap[item.type]}`"
          size="40"
          class="w-10 h-10"
      />
    </button>

  </div>
</template>
