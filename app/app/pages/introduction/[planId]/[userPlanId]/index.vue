<script setup lang="ts">
import type {IplanInfo} from "~/services/PlanService";

const router = useRouter();
const route = useRoute();
const {$api} = useNuxtApp();
const planInfo = ref<IplanInfo | null>(null)
const authStore = useAuthStore()
useHead({
  title:'معرفی'
})
definePageMeta({
  auth: true
})
function startInterview() {
  // router.push("/dashboard");
  router.push(`/introduction/${route.params.planId}/${route.params.userPlanId}/interview`);
}

async function getPlanInfo() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanById(route.params.planId);
    planInfo.value = response.data
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

getPlanInfo()
</script>

<template>
  <div class="w-full h-full relative custom-pattern-bg-image">
    <BaseMinimalHeader @go-back="router.back(-1)"></BaseMinimalHeader>
    <div
      class="w-full h-[calc(100svh-64px)] bg-[#E9E9E9] px-5 py-4 rounded-t-[32px] flex flex-col justify-between"
    >
      <div v-if="planInfo" class="px-5">
        <p class="pt-4 text-[#333333] font-medium">
          لطفا قبل از شروع، متن زیر را مطالعه کنید.
        </p>
        <p v-html="planInfo.description"></p>
      </div>
      <button
        type="button"
        class="w-full btn btn-primary bottom-0 !rounded-[28px]"
        @click="startInterview"
      >
        شروع
      </button>
    </div>
  </div>
</template>
