4
<script setup lang="ts">
import {isAxiosError} from 'axios'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {IIncreaseOrDecreaseStockPayload, IReserveOrReleaseStockPayload} from "@/services/InventoryService";
import {useSpinner} from "@/composables/spinner";
import {useAlerts} from "@/composables/alert";
import {IWarehouseStock} from "@/services/WarehouseService";

interface IProps {
  dialogState: boolean,
  selectedItem: IWarehouseStock
}

const props = defineProps<IProps>()

const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()
const $api = inject<IApiProvider>('$api')
const route = useRoute()
const refVForm = ref(null)
const stockPayload = ref<IReserveOrReleaseStockPayload>({
  "productId": 0,
  "variantId": 0,
  "warehouseId": 0,
  "quantity": 0,
  "referenceId": ""
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    increaseStock()
  } else {
    useAlerts().error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function increaseStock() {
  try {
    useSpinner().showSpinner()
    const response = await $api?.inventory.increaseStock(stockPayload.value)
    if (response.data.isSuccess) {
      stockPayload.value = {
        productId: null,
        variantId: null,
        warehouseId: null,
        quantity: null,
        reason: ''
      }
      useAlerts().success('موجودی با موفقیت افزوده شد')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      useAlerts().error(response.data.message)
    }
  } catch (error: unknown) {
    if (isAxiosError(error))
      useAlerts().error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  } finally {
    useSpinner().hideSpinner()
  }
}


</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="رزرو موجودی از این انبار"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100 py-2"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <WarehousePicker :return-object="false" required v-model="stockPayload.warehouseId"></WarehousePicker>
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="stockPayload.quantity"
            variant="outlined"
            density="compact"
            label="تعداد"
            type="number"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="stockPayload.reason"
            variant="outlined"
            density="compact"
            label="علت افزایش موجودی"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
