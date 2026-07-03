<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

// Interfaces
interface Props {
  dialogState: boolean
  defaultStep: ICreateOrUpdateBrandPayload
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultStep: () => {
    return {}
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function updateStep() {
  if (props.defaultStep) {
    try {
      spinner.showSpinner()
    delete props.defaultStep.stepOptions
      const response = await $api?.steps.updateStep(props.defaultStep, props.defaultStep.id)
      if (response.data.isSuccess) {

        alert.success('مرحله با موفقیت ویرایش شد!')
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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`ویرایش مرحله`"
    @update:dialog-state="updateDialogState"
    @update="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent
    >
      <VCol cols="12">
        <VTextField
          v-model.trim="defaultStep.name"
          variant="outlined"
          density="compact"
          label="عنوان مرحله"
          color="update"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
        />
      </VCol>
    </VForm>
  </CustomUpdateDialog>
</template>
