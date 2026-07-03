<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateStepPayload} from "@/services/StepsService";
import {useAuthStore} from "@/stores/auth";

// Interfaces
interface Props {
  dialogState: boolean
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
const refVForm = ref(null)
const $api = inject<IApiProvider>('$api')
const authStore =  useAuthStore()
const stepPayload = ref<ICreateStepPayload>({
  name: null,
  createdByUserId:authStore.getUser.id
})

const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions
async function createStep() {
  try {
    spinner.showSpinner()

    const response = await $api?.steps.createStep(stepPayload.value)
    if (response.data.isSuccess) {
      stepPayload.value = {
        name: null,
      }
      alert.success('مرحله با موفقیت ایجاد شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response.data.errorMessage)
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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد مرحله جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent
    >
      <VCol cols="12">
        <VTextField
          v-model.trim="stepPayload.name"
          variant="outlined"
          density="compact"
          label="عنوان مرحله"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
        />
      </VCol>
    </VForm>

  </CustomCreateDialog>
</template>
