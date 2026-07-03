<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import CustomDatePicker from "@/components/Utilities/CustomDatePicker.vue";
import {VForm} from "vuetify/components/VForm";
import {IDiscount} from "@/services/DiscountService";

// Interfaces
interface Props {
  dialogState: boolean
  selectedItem: IDiscount
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  selectedItem: () => {
    return {}
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function updateCoupon() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.discounts.updateCoupon(props.selectedItem)
      alert.success('کد تخفیف با موفقیت ویرایش شد!')
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
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateCoupon()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`ویرایش تخفیف`"
    @update:dialog-state="updateDialogState"
    @update="validateData"
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
            v-model.trim="props.selectedItem.code"
            density="compact"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
            label="کد استفاده از تخفیف *"
            color="success"
          />
        </VCol>

        <VCol cols="12" md="6">
          <VTextField
            v-model.trim="props.selectedItem.discountAmount"
            density="compact"
            label="مقدار تخفیف(تومان) *"
            color="success"
          />
        </VCol>
        <VCol cols="12" md="6">
          <VTextField
            v-model.trim="props.selectedItem.discountPercentage"
            density="compact"
            label="مقدار تخفیف(درصد) *"
            maxlength="2"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <CustomSwitch label="تخفیف ویژه" v-model="props.selectedItem.isSpecial"></CustomSwitch>
        </VCol>
        <VCol
          v-if="props.selectedItem.isSpecial"
          cols="12"
          md="6"
        >
          <CustomDatePicker
            v-model="props.selectedItem.startDate"
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
            v-model="props.selectedItem.endDate"
            density="compact"
            input-id="endDate"
            required
            label="تاریخ پایان تخفیف را انتخاب کنید"
          />
        </VCol>
      </VRow>
    </VForm>

  </CustomUpdateDialog>
</template>
