<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'


interface Props {
  dialogState: boolean
  defaultExercises: any
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultExercises: () => {
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

async function deleteExercises() {
  if (props.defaultExercises) {
    try {
      spinner.showSpinner()
      const response = await $api?.exercise.deleteExercise(props.defaultExercises.id)
      if (response.data.isSuccess) {

        alert.success('تمرین با موفقیت حذف شد!')
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
</script>

<template>
  <CustomDeleteDialog
    :dialog-state="props.dialogState"
    :title="`حذف تمرین`"
    @update:dialog-state="updateDialogState"
    @delete="deleteExercises"
  >
    <VCol cols="12">
      <p>
        آیا از حذف این تمرین
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
