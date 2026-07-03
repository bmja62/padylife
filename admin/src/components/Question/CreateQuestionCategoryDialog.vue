<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateOrUpdateQuestionCategoryPayload} from "@/services/questionService";

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

const questionCategoryPayload = ref<ICreateOrUpdateQuestionCategoryPayload>({
  name: null,
})

const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions
async function createQuestionCategory() {
  try {
    spinner.showSpinner()

    const response = await $api?.question.createQuestionCategory(questionCategoryPayload.value)
    if (response.data.isSuccess) {
      questionCategoryPayload.value = {
        name: null,
      }
      alert.success('دسته بندی با موفقیت ایجاد شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response.data.message)
    }
  } catch (error: unknown) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.errorMessage || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createQuestionCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد دسته بندی جدید"
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
          v-model.trim="questionCategoryPayload.name"
          variant="outlined"
          density="compact"
          label="نام دسته بندی"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
        />
      </VCol>
    </VForm>

  </CustomCreateDialog>
</template>
