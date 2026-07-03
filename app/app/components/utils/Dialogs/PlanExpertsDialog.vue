<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IPlan} from "~/services/PlanService";

const isRendering = defineModel()
const {$api} = useNuxtApp<IApiProvider>()

interface IProps {
  plan: IPlan;
}

const props = defineProps<IProps>();
const router = useRouter()
const authStore = useAuthStore()
const filters = ref({
  planId: props?.plan?.id,
  globalGrid: {
    pageNumber: 1,
    count: 10,
    search: ''
  }
})
const users = ref([])
const totalCount = ref(1)

async function getPlanUsers() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getPlanExperts(filters.value)
    users.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

function changePage(page: number) {
  filters.value.globalGrid.pageNumber = page
  getPlanUsers()
}

watch(() => props.plan, async (val) => {
  if (val) {
    getPlanUsers()
  }
}, {immediate: true})
</script>

<template>
  <LazyUtilsDialogsBaseDialog :dialog-id="`plan${props.plan.id}`" v-model="isRendering">
    <template #title>
      <span class="!text-base">متخصصانی که این پلن را همراهی میکنند</span>
    </template>
    <template #default>
      <div v-if="props.plan" class="w-full flex flex-col justify-start items-start gap-4">
        <template v-if="users.length">
        <div class="w-full flex items-center gap-3" v-for="user in users">
          <img v-if="user.profileImage" :src="user.profileImage" class="w-10 h-10 object-cover rounded-full shadow"
               alt="">
          <img v-else src="/common/no-image.png" class="w-10 h-10 object-cover rounded-full shadow" alt="">
          <span>{{ user.fullName ? user.fullName : '-' }}</span>
        </div>
        </template>
        <LazyUtilsEmptyView v-else empty-text="در حال حاضر متخصص فعالی این برنامه را همراهی نمیکند"></LazyUtilsEmptyView>
        <div class="w-full flex items-center justify-center">
          <UtilsCustomPagination
              :page-number="filters.globalGrid.pageNumber"
              :count="filters.globalGrid.count"
              :total-count="totalCount"
              @change-page="changePage"
          />
        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>