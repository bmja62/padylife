<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IWarehouse} from "@/services/WarehouseService";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IWarehouse
}

// Props
const props = withDefaults(defineProps<IProps>(), {})

// Emits
const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateWarehouse()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateWarehouse() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.warehouse.updateWarehouse(props.selectedItem, props.selectedItem.id)
      if (response.data.isSuccess) {
        alert.success('انبار با موفقیت ویرایش شد!')
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
}

function setGeoLocation(lat, long) {
  props.selectedItem.latitude = lat;
  props.selectedItem.longitude = long;
}
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    title="ویرایش انبار"
    @update:dialog-state="updateDialogState"
    @update="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.name"
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
            v-model.trim="props.selectedItem.code"
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
            v-model.trim="props.selectedItem.contactPhone"
            variant="outlined"
            density="compact"
            label="شماره تماس"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.managerName"
            variant="outlined"
            density="compact"
            label="نام مدیر"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <VTextarea
            v-model.trim="props.selectedItem.address"
            variant="outlined"
            density="compact"
            label="آدرس"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <span>موقعیت مکانی</span>
          <SetLocation :default-marker-geo-loc="[props.selectedItem.latitude,props.selectedItem.longitude]" @getGeoLocation="setGeoLocation"></SetLocation>

        </VCol>
      </VRow>
    </VForm>


  </CustomUpdateDialog>
</template>
