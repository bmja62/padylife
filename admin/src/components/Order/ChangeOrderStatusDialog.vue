<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import OrderStatusPicker from "@/components/Order/OrderStatusPicker.vue";

// Interfaces
interface Props {
  dialogState: boolean
  defaultOrder: ICreateOrUpdateBrandPayload
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultOrder: () => {
    return {}
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const selectedStatus = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}


async function changeOrderStatus() {
  if (props.defaultOrder) {
    try {
      spinner.showSpinner()
      const response = await $api?.order.changeOrderStatus({
        orderId: props.defaultOrder.orderId,
        newStatus: selectedStatus.value
      })
      alert.success('وضعیت سفارش با موفقیت تغییر یافت!')
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
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`تغییر وضعیت سفارش`"
    @update:dialog-state="updateDialogState"
    @update="changeOrderStatus"
  >
    <VCol cols="12" md="12">
      <OrderStatusPicker v-model="selectedStatus"></OrderStatusPicker>

    </VCol>

  </CustomUpdateDialog>
</template>
