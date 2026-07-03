<script lang="ts" setup>
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateTransactionPayload, IWalletTransactionsListItem} from "@/services/WalletService";
import {isAxiosError} from "axios";

// Interfaces
interface Props {
  dialogState: boolean,
  selectedTransaction:IWalletTransactionsListItem

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
const $api = inject<IApiProvider>('$api')
const route = useRoute()

// Functions
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}





</script>

<template>
  <CustomWarningDialog
    :dialog-state="props.dialogState"
    persistent
    title="مشاهده توضیحات تراکنش"
    @update:dialog-state="updateDialogState"
  >
    <VCol cols="12">
      <p>{{ selectedTransaction.description }}</p>
    </VCol>


  </CustomWarningDialog>
</template>
