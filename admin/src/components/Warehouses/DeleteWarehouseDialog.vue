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
const warehouseCode = ref('')

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteWarehouse() {
  if (props.selectedItem) {
    if (warehouseCode.value && warehouseCode.value === props.selectedItem.code) {
      try {
        spinner.showSpinner()

        const response = await $api?.warehouse.deactiveWarehouse(props.selectedItem.id)
        if (response.data.isSuccess) {
          alert.success('انبار با موفقیت غیرفعال شد!')
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
    } else {
      alert.error('کد وارد شده معتبر نمی‌باشد')
    }
  }
}
</script>

<template>
  <CustomDeleteDialog
    action-text="تایید"

    :dialog-state="props.dialogState"
    @update:dialog-state="updateDialogState"
    @delete="deleteWarehouse"
  >
    <VCol cols="12">
      <p>
        <strong>هشدار !</strong>
        شما در حال غیرفعال کردن انبار <span class="text-error">{{ props.selectedItem.name }}</span> هستید. در صورت
        تایید, تمامی محصولاتی که در این انبار
        موجودی دارند غیرفعال خواهند شد و این تغییر بدون بازگشت می‌باشد.
      </p>
      <p>جهت تاییدیه لطفا کد انبار را در فیلد زیر وارد کنید</p>
    </VCol>
    <VCol cols="12">
      <VTextField
        v-model.trim="warehouseCode"
        variant="outlined"
        density="compact"
        label="کد انبار"
        color="success"
        required
        :rules="[(value) => !!value || 'فیلد اجباری است']"
      />
    </VCol>
  </CustomDeleteDialog>
</template>
