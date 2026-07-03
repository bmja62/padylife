<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IDiscount} from "@/services/DiscountService";


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

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteCoupon() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.discounts.deleteCoupon(props.selectedItem.id)
      alert.success('کد تخفیف با موفقیت حذف شد!')
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
  <CustomDeleteDialog
    :dialog-state="props.dialogState"
    :title="`حذف کد تخفیف`"
    @update:dialog-state="updateDialogState"
    @delete="deleteCoupon"
  >
    <VCol cols="12">
      <p>
        آیا از حذف این کد تخفیف
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
