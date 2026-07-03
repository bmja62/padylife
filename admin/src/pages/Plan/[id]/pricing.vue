<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {IPlan, IPricePayload,IActivityPayload} from "@/services/PlanService";
import {useAuthStore} from "@/stores/auth";
import {useAlerts} from "@/composables/alert";


// LifeCycles
onMounted(() => {
  getPlanById()
  getSpecialistPriceForPlan()
})

// Variables
const route = useRoute()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const authStore = useAuthStore()
const planInfo = ref<IPlan>(null)
const pricePayload = ref<IPricePayload>({
  expertId: null,
  planId: route.params.id,
  price: null,
})
const activityPayload = ref<IActivityPayload>({
  expertId:null,
  planId: route.params.id,
  isActive:null
})
async function getPlanById() {
  try {
    spinner.showSpinner()
    const data = await $api?.plan.getPlanById(route.params.id)
    if (data.data.isSuccess) {
      planInfo.value = data.data.data
    } else {
      useAlerts().error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function getSpecialistPriceForPlan() {
  try {
    spinner.showSpinner()
    const data = await $api?.plan.getSpecialistPriceForPlan({
      id: route.params.id,
      expertId: authStore.getUser.id
    })
    if (data.data.data) {
      pricePayload.value.price = data.data.data.price
      activityPayload.value.isActive = data.data.data.isActive
    } else {
      activityPayload.value.isActive = true
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function setExpertPlanPrice() {
  if (pricePayload.value.price) {

    try {
      spinner.showSpinner()
      if (!pricePayload.value.expertId) {
        pricePayload.value.expertId = authStore.getUser.id
      }
      if (typeof pricePayload.value.price!=='number')
        pricePayload.value.price = pricePayload.value.price.replaceAll(',', '')
      const data = await $api?.plan.setExpertPlanPrice(pricePayload.value)

      if (data.data.isSuccess) {
        useAlerts().success('قیمت گذاری با موفقیت انجام شد')
        getSpecialistPriceForPlan()
        pricePayload.value = {
          expertId: null,
          planId: route.params.id,
          price: null
        }
      } else {
        useAlerts().error(data.data.message)
      }
    } catch (e) {
      console.error(e)
    } finally {
      spinner.hideSpinner()
    }
  } else {
    useAlerts().error('لطفا یک قیمت وارد کنید')
  }
}

const formattedPrice = computed({
  get: function () {
    return pricePayload.value.price;
  },
  set: function (newValue) {
    if (newValue && newValue !== "") {
      // Remove all characters that are NOT numbers
      const cleanedValue = newValue.replace(/[^\d]/g, "");

      // Format the cleaned value with commas
      if (cleanedValue) {
        pricePayload.value.price = Number(cleanedValue).toLocaleString("en-US");
      } else {
        pricePayload.value.price = null;
      }
    } else {
      pricePayload.value.price = null;
    }
  },
});


async function setExpertPlanActivity () {
  activityPayload.value.isActive = !activityPayload.value.isActive


    try {
      spinner.showSpinner()
      if (!activityPayload.value.expertId) {
        activityPayload.value.expertId = authStore.getUser.id
      }
      delete activityPayload.value.isActive
      const data = await $api?.plan.setExpertPlanActivity(activityPayload.value)

      if (data.data.isSuccess) {
        useAlerts().success('اعمال تغیرات با موفقیت انجام شد')
        getSpecialistPriceForPlan()
        activityPayload.value = {
          expertId: null,
          planId: route.params.id,
        }
      } else {
        useAlerts().error(data.data.message)
      }
    } catch (e) {
      console.error(e)
    } finally {
      spinner.hideSpinner()
    }
}
</script>

<template>
  <PageWrapper
    v-if="planInfo"
  >
    <template #title>
      قیمت گذاری {{ planInfo.title }}
    </template>
    <VRow>
      <VCol cols="12" md="6">
        <CustomSwitch  @click="setExpertPlanActivity" v-model="activityPayload.isActive" label="وضعیت فعالیت شما برای این پلن"  :has-tooltip="false"
                      :has-icon="true"></CustomSwitch>
      </VCol>
     
    </VRow>
    <VRow v-if="activityPayload.isActive">
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="formattedPrice"
          color="success"
          label="قیمت  (تومان)"
        />
      </VCol>
      <VCol v-if="$can('manage','all')" cols="12" md="6">
        <UserPicker :return-object="false" role-filter="Specialist" v-model="pricePayload.expertId"></UserPicker>
      </VCol>
      <VCol cols="12" md="6">
        <VBtn
          color="success"
          variant="flat"
          @click="setExpertPlanPrice"
        >
          ثبت
        </VBtn>
      </VCol>
    </VRow>
  </PageWrapper>
</template>
