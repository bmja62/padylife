<script lang="ts" setup>
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateTransactionPayload} from "@/services/WalletService";
import {isAxiosError} from "axios";

// Interfaces
interface Props {
  dialogState: boolean,
  transactionAction: number
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
const formRef = ref(null)
const createTransactionPayload = ref<ICreateTransactionPayload>({
  walletId: +route.params.id,
  description: "",
  credit: null

})

// Functions
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await formRef?.value?.validate();
  if (isValid.valid) {
    decideTransactionType()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function decideTransactionType() {
  if (props.transactionAction === 1) {
    await withdraw()
  } else if (props.transactionAction === 2) {
    await deposit()
  }
}

async function withdraw() {
  try {
    spinner.showSpinner()
    const response = await $api?.wallet.withdraw(createTransactionPayload.value)
    if (response?.data.isSuccess) {
      createTransactionPayload.value = {
        walletId: +route.params.id,
        description: "",
        credit: null
      }
      alert.success('تراکنش با موفقیت ثبت شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
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

async function deposit() {
  try {
    spinner.showSpinner()
    const response = await $api?.wallet.deposit(createTransactionPayload.value)
    if (response?.data.isSuccess) {
      createTransactionPayload.value = {
        walletId: +route.params.id,
        description: "",
        credit: null
      }
      alert.success('تراکنش با موفقیت ثبت شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
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
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد تراکنش جدید"
    @create="validateData"
    @update:dialog-state="updateDialogState"
  >
    <VCol cols="12">
      <VForm
        ref="formRef"
      >
        <VTextField
          v-model.trim="createTransactionPayload.credit"
          :rules="[(value) => !!value || 'فیلد مبلغ اجباری است']"
          color="success"
          density="compact"
          hide-details="auto"
          label="مبلغ (تومان) *"
          required
          type="number"
          variant="outlined"
        />
      </VForm>
    </VCol>
    <VCol cols="12">
      <VTextField
        v-model.trim="createTransactionPayload.description"
        color="success"
        density="compact"
        hide-details="auto"
        label="توضیحات"
        required
        variant="outlined"
      />
    </VCol>

  </CustomCreateDialog>
</template>
