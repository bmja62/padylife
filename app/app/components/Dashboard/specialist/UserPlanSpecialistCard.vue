<script setup lang="ts">
import type {IExpert} from "~/services/PlanService";

interface IProps {
  expert:IExpert
}
const props:IProps = defineProps({
  expert:{
    type:Object as PropType<IExpert>
  }
})
</script>

<template>
<nuxt-link to="/dashboard/chat" class="w-full shadow rounded-lg bg-white p-1 flex gap-2">
  <NuxtImg
      v-if="props.expert?.profileImage"
      :src="props.expert?.profileImage"
      class="w-16 h-16 rounded-lg object-cover"
  />
  <NuxtImg
      v-else
      src="/common/no-image.png"
      class="w-16 h-16 rounded-lg object-cover"
  />
  <div class="flex flex-col gap-2 h-full justify-between">
    <strong>{{props.expert.fullName}}</strong>
    <span>{{props.expert.specialization}}</span>
  </div>
</nuxt-link>
</template>

<style scoped>

</style>