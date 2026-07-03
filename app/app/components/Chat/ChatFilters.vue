<script setup lang="ts">

import {ChatFilters} from "~~/types/chat";

const selectedFilter = defineModel({
  default: ChatFilters.Companion
})
</script>

<template>
  <div
      class="w-full flex flex-row  justify-around items-center pl-2 pb-4"
  >
    <div @click="selectedFilter = ChatFilters.Companion" class="w-1/2 border-b flex py-2 items-center justify-center"
         :class="{'border-b border-primary':selectedFilter === ChatFilters.Companion}">
    <span
        :class="{'!text-primary':selectedFilter === ChatFilters.Companion}"
        class="text-gray-800  cursor-pointer text-sm">همراه</span>
    </div>
    <div @click="selectedFilter = ChatFilters.Specialist" class="w-1/2 border-b flex py-2 items-center justify-center"
         :class="{'border-b border-primary':selectedFilter === ChatFilters.Specialist}">
    <span
        :class="{'!text-primary':selectedFilter === ChatFilters.Specialist}"
        class="text-gray-800  cursor-pointer text-sm">متخصص</span>
    </div>
  </div>
</template>

<style scoped>

</style>