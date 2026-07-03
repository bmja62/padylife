<script setup lang="ts">
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {ICreateWarehousePayload} from "@/services/WarehouseService";

interface IProps {
  dialogState: boolean
}

const props = defineProps<IProps>()

const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')

const refVForm = ref(null)
const warehousePayload = ref<ICreateWarehousePayload>({
  name: '',
  code: '',
  address: '',
  contactPhone: '',
  managerName: '',
  latitude: null,
  longitude: null
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createWarehouse()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createWarehouse() {
  try {
    spinner.showSpinner()
    const response = await $api?.warehouse.createWarehouse(warehousePayload.value)
    if (response.data.isSuccess) {
      warehousePayload.value = {
        name: '',
        code: '',
        address: '',
        contactPhone: '',
        managerName: '',
        latitude: null,
        longitude: null
      }
      alert.success('انبار با موفقیت ایجاد شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response.data.message)
    }
  } catch (error: unknown) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function setGeoLocation(lat, long) {
  warehousePayload.value.latitude = lat;
  warehousePayload.value.longitude = long;
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد انبار جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <VTextField
            v-model.trim="warehousePayload.name"
            variant="outlined"
            density="compact"
            label="نام انبار"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="warehousePayload.code"
            variant="outlined"
            density="compact"
            label="کد انبار"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="warehousePayload.contactPhone"
            variant="outlined"
            density="compact"
            label="شماره تماس"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="warehousePayload.managerName"
            variant="outlined"
            density="compact"
            label="نام مدیر"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <VTextarea
            v-model.trim="warehousePayload.address"
            variant="outlined"
            density="compact"
            label="آدرس"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <span>موقعیت مکانی</span>
          <SetLocation @getGeoLocation="setGeoLocation"></SetLocation>

        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
