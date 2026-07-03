<script setup lang="ts">
import type {IPlanSubscriber} from "~/services/PlanService";

interface IProps {
  user: IPlanSubscriber
}

const props:IProps = defineProps({
  user: {
    type: Object as PropType<IPlanSubscriber>
  }
})
</script>

<template>
  <div class="w-72 h-72 rounded-full flex items-center justify-center">
    <div
      class="w-[170px] h-[170px] rounded-full bg-[#DBE9EC] flex items-center justify-center"
    >
      <NuxtImg
          v-if="props.user.imageUrl"
          :src="props.user.imageUrl"
        class="w-36 h-36 object-cover rounded-full"
      />
      <NuxtImg
          v-else
          src="/common/no-image.png"
          class="w-36 h-36 object-cover rounded-full"
      />
    </div>
  </div>
</template>
