


<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

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
const text = ref('')
const route = useRoute()
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions
async function addOptionToQuestion() {
  try {
    spinner.showSpinner()
    const response = await $api?.question.addOptionToQuestion({
      text: text.value,
      questionId: route.params.id
    })
    if (response.data.isSuccess) {
      text.value = ''
      alert.success('گزینه با موفقیت ایجاد شد!')
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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    addOptionToQuestion()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="افزودن گزینه جدید"
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
          v-model="text"
          type="text"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          :label="`متن گزینه را وارد کنید`"
        />
      </VCol>
    </VForm>

  </CustomCreateDialog>
</template>
