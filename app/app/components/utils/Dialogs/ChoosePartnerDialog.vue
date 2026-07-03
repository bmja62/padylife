<script setup lang="ts">

import type {IApiProvider} from "~/models/IApiProvider";
import type {IGetUserPlansFilters} from "~/services/PlanService";

const isRendering = defineModel()

interface IProps {
  userPlanId: string | number
}

const router = useRouter()
const props: IProps = defineProps({
  userPlanId: {
    type: [Number, String] as PropType<string | number>
  }
})
const selectedTab = ref(1)
const selectedUserId = ref(null)
const userPlanCompanionFilters = ref<IGetUserPlansFilters>({
  userPlanId: props.userPlanId,
  pageNumber: 1,
  count: 10,
})
const userPlanCompanions = ref([])
const totalCount = ref(1)
const similarUsersPicker = useTemplateRef('similarUsersPicker')
const {$api} = useNuxtApp<IApiProvider>()
const fields = ref([
  {
    key: 'companionFullName',
    label: 'نام',
    formatter: (item) => item ? item : '-'
  },
  {
    key: 'action',
    label: 'عملیات'
  },
])
function checkCompanionSelected() {
  if (selectedTab.value === 1) {
    pickRandomCompanion()
  } else if (selectedTab.value === 2 && selectedUserId.value) {
    addCompanion()
  } else {
    useAlerts().error('لطفا یک همراه انتخاب کنید')
  }
}

async function addCompanion() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.addUserCompanion({
      userPlanId: props.userPlanId,
      companionUserId: selectedUserId.value
    })
    if (response.isSuccess) {
      useAlerts().success('همراهتو با موفقیت انتخاب کردی')
      router.push('/dashboard')
    } else {
      useAlerts().error(response.message)
    }
    isRendering.value = false
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function removeUserCompanion(userCompanion) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.removeUserCompanion(userCompanion.companionUserId)
    if (response.isSuccess) {
      useAlerts().success('همراهتو با موفقیت حذف کردی')
      getUserPlanCompanions()
    } else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

function closeAll(){
  router.push('/dashboard')
  isRendering.value  = false
}

async function getUserPlanCompanions() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getUserPlanCompanions(userPlanCompanionFilters.value)
    userPlanCompanions.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
async function pickRandomCompanion() {
  await similarUsersPicker.value.selectRandom()
  await addCompanion()
}

await getUserPlanCompanions()
</script>

<template>
  <LazyUtilsDialogsBaseDialog v-model="isRendering" dialog-id="ChoosePartner" full-height>
    <template #title>
      <span>انتخاب همراه</span>
    </template>
    <template #default>
      <div class="w-full flex h-full flex-col gap-2">
        <div class="w-full flex items-center shadow rounded-lg border border-gray-200 p-2 gap-2">
          <div @click="selectedTab = 2" :class="{'[&_*]:!fill-white bg-primary text-white':selectedTab===2}"
               class="flex [&_*]:fill-gray-400  p-2 rounded-lg items-center w-1/2 gap-1">
            <Icon name="icon:user-search" class=" w-5 h-5"/>
            <small>انتخاب همراه دلخواه</small>

          </div>
          <div @click="selectedTab = 1" :class="{'[&_*]:!fill-white bg-primary text-white':selectedTab===1}"
               class="flex [&_*]:fill-gray-400 items-center p-2 rounded-lg w-1/2 gap-1">
            <Icon name="icon:computer-check" class=" w-5 h-5"/>
            <small>انتخاب سیستمی همراه</small>
          </div>
        </div>
        <div v-show="selectedTab === 2"
             class="w-full flex items-center shadow rounded-lg border border-gray-200 p-2 gap-2">
          <LazyUtilsPickersUserSimilarPicker
              ref="similarUsersPicker" v-model="selectedUserId"
              class="w-full"></LazyUtilsPickersUserSimilarPicker>
        </div>
        <div v-if="userPlanCompanions.length" class="w-full flex py-4 flex-col gap-2">
          <strong>همراهان فعلی شما</strong>
          <LazyUtilsCustomTable
              :table-headers="fields" :table-items="userPlanCompanions"
              :page-number="userPlanCompanionFilters.pageNumber"
              :count="userPlanCompanionFilters.count"
              :total-count="totalCount">
            <template #action="data">
              <Icon
                  @click="removeUserCompanion(data.item)"
                  name="icon:trash"
                  class="[&_*]:fill-primary cursor-pointer"
                  size="18"
              />
            </template>
          </LazyUtilsCustomTable>
        </div>
        <div class="w-full flex flex-col gap-2">
          <button class=" btn bg-primary  text-white w-full" type="button" @click="checkCompanionSelected">
            ثبت نهایی برنامه
          </button>
          <button class=" btn bg-white border border-primary !text-primary w-full" type="button" @click="closeAll">
            ادامه بدون انتخاب همراه
          </button>
        </div>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>