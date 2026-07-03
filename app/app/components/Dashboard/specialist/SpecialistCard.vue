<script setup lang="ts">
import type {IPrice} from "~/services/PlanService";
import RequestConsultantDialog from "~/components/utils/Dialogs/RequestConsultantDialog.vue";

interface IProps {
  specialist: IPrice
}

const route = useRoute()
const props: IProps = defineProps({
  specialist: {
    type: Object as PropType<IPrice>
  }
})
const isRenderingRequestConsultantDialog = ref(false)
</script>
<template>
  <div
    class="w-full flex flex-col gap-y-2 rounded-[20px] bg-white border border-[#E0E4E8] p-4"
  >
    <div class="w-full flex flex-row justify-between items-center">
      <h3 class="text-gray-800 font-semibold">
        {{ props.specialist.jobTitle }}
      </h3>

    </div>
    <div class="w-full flex flex-row justify-between items-center">
      <span class="text-gray-700 text-sm">
         {{ props.specialist.expertFullName }}
      </span>
      <span class="text-gray-700 text-sm">
        {{ Intl.NumberFormat('fa-IR').format(props.specialist.price) }}
        تومان
      </span>
    </div>
    <div class="w-full grid grid-cols-2 gap-2 justify-between items-center">
      <nuxt-link
          :to="`/dashboard/specialists/detail/${props.specialist.expertId}?planId=${route.params.planId}`"
        class="w-full btn !rounded-xl btn-primary btn-outline font-normal"
      >
        مشاهده پروفایل
      </nuxt-link>
      <button type="button" @click="isRenderingRequestConsultantDialog = true"
              class="w-full btn !rounded-xl btn-primary font-normal">
        درخواست همراهی
      </button>
    </div>
    <RequestConsultantDialog :user-id="props.specialist.expertId"
                             :plan-id="props.specialist.planId"
                             v-model="isRenderingRequestConsultantDialog"></RequestConsultantDialog>
  </div>
</template>
