<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import CustomDatePicker from "@/components/Utilities/CustomDatePicker.vue";
import {useAuthStore} from "@/stores/auth";
import {ICreateDiscountPayload} from "@/services/DiscountService";
import {VForm} from "vuetify/components/VForm";


// Interfaces
interface Props {
  dialogState: boolean
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = defineProps<Props>()
const emit = defineEmits<Emit>()
const spinner = useSpinner()
const alert = useAlerts()
const auth = useAuthStore()
const refVForm = ref(null)
const $api = inject<IApiProvider>('$api')

const couponPayload = ref<ICreateDiscountPayload>({
  code: '',
  discountAmount: 0,
  discountPercentage: 0,
  startDate: null,
  endDate: null,
  isSpecial: false,
})
const selectedType = ref(1)
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createDiscount()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createDiscount() {
  try {
    spinner.showSpinner()
    const response = await $api?.discounts.createCoupon(couponPayload.value)
    alert.success('کد تخفیف با موفقیت ایجاد شد!')
    emit('update:dialogState', false)
    emit('refetch')
  } catch (error: unknown) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

const computedDiscountPercentage = computed({
  get(){
    return couponPayload.value.discountPercentage
  },
  set(val){
    if(val>100){
    couponPayload.value.discountPercentage =100
       return 100
    }else{
      couponPayload.value.discountPercentage =val
      return val
    }
  }
})
</script>

<template>
  <CustomCreateDialog
    width="1000"
    persistent
    :dialog-state="props.dialogState"
    title="ایجاد تخفیف جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>

        <VCol
          cols="12"
          md="12"
        >
          <VTextField
            v-model.trim="couponPayload.code"
            density="compact"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
            label="کد استفاده از تخفیف *"
            color="success"
          />
        </VCol>
        <VCol cols="12" md="6">
          <CouponTypePicker
            required
            :clearable="false"
            v-model="selectedType" :return-object="false"></CouponTypePicker>
        </VCol>
        <VCol v-if="selectedType===2" cols="12" md="6">
          <VTextField
            v-model.trim="couponPayload.discountAmount"
            density="compact"
            label="مقدار تخفیف(تومان) *"
            color="success"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
          />
        </VCol>
        <VCol cols="12" md="6" v-if="selectedType===1">
          <VTextField
            v-model.trim="computedDiscountPercentage"
            density="compact"
            label="مقدار تخفیف(درصد) *"
            color="success"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
          />
        </VCol>
        <VCol cols="12">
          <CustomSwitch label="تخفیف ویژه" v-model="couponPayload.isSpecial"></CustomSwitch>
        </VCol>
        <VCol
          v-if="couponPayload.isSpecial"
          cols="12"
          md="6"
        >
          <CustomDatePicker
            v-model="couponPayload.startDate"
            density="compact"
            input-id="startDate"
            label="تاریخ شروع تخفیف را انتخاب کنید"
          />
        </VCol>
        <VCol
          cols="12"
          md="6"
        >
          <CustomDatePicker
            v-model="couponPayload.endDate"
            density="compact"
            input-id="endDate"
            required
            label="تاریخ پایان تخفیف را انتخاب کنید"
          />
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
