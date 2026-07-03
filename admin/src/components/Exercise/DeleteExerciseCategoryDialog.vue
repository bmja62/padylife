<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'


interface Props {
  dialogState: boolean
  defaultCategory: any
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultCategory: () => {
    return {
      name: null,
    }
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

async function deleteExerciseCategory() {
  if (props.defaultCategory) {
    try {
      spinner.showSpinner()
      const response = await $api?.exercise.deleteExerciseCategory(props.defaultCategory.id)
      if (response.data.isSuccess) {

        alert.success('دسته بندی با موفقیت حذف شد!')
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
}
</script>

<template>
  <CustomDeleteDialog
    :dialog-state="props.dialogState"
    :title="`حذف دسته بندی`"
    @update:dialog-state="updateDialogState"
    @delete="deleteExerciseCategory"
  >
    <VCol cols="12">
      <p>
        آیا از حذف این دسته بندی
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
