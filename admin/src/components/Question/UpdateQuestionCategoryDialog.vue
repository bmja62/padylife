<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

// Interfaces
interface Props {
  dialogState: boolean
  defaultCategory: ICreateOrUpdateBrandPayload
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultCategory: () => {
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

async function updateExerciseCategory() {
  if (props.defaultCategory) {
    try {
      spinner.showSpinner()

      const response = await $api?.question.updateQuestionCategory(props.defaultCategory, props.defaultCategory.id)
      alert.success('دسته بندی با موفقیت ویرایش شد!')
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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateExerciseCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`ویرایش دسته بندی`"
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
          v-model.trim="defaultCategory.name"
          variant="outlined"
          density="compact"
          label="نام دسته بندی"
          color="update"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
        />
      </VCol>
    </VForm>
  </CustomUpdateDialog>
</template>
