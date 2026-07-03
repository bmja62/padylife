<script setup lang="ts">
import { isAxiosError } from 'axios'
import type { IApiProvider } from '@/models/IApiProvider'
import { useSpinner } from '@/composables/spinner'
import { useAlerts } from '@/composables/alert'

// Interfaces
interface IProps {
  dialogState: boolean
}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}

// Props
const props = defineProps<IProps>()

// Emits
const emit = defineEmits<IEmit>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}
async function closeTicket() {
  try {
    spinner.showSpinner()

    const response = await $api?.tickets.closeATicket(route.params.id as string)

    if (response?.data.isSuccess) {
      updateDialogState(false)
      emit('refetch')
    }

    else { alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید') }
  }
  catch (error) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  }
  finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <CustomDeleteDialog
    :dialog-state="props.dialogState"
    title="تغییر وضعیت تیکت"
    action-text="تایید"
    @update:dialog-state="updateDialogState"
    @delete="closeTicket"
  >
    <VCol cols="12">
      آیا از تغییر وضعیت این تیکت به بسته شده اطمینان دارید؟
    </VCol>
  </CustomDeleteDialog>
</template>
