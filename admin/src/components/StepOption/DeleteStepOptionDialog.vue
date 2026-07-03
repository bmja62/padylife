<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'


interface Props {
  dialogState: boolean
  selectedStepOption: any
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  selectedStepOption: () => {
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

async function deleteStepOption() {
  if (props.selectedStepOption) {
    try {
      spinner.showSpinner()
      const response = await $api?.stepOption.deleteStepOption({
        confrim: true,
        id: props.selectedStepOption.id
      })
      if (response.data.isSuccess) {
        alert.success('گام با موفقیت حذف شد!')
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
    :title="`حذف گام`"
    @update:dialog-state="updateDialogState"
    @delete="deleteStepOption"
  >
    <VCol cols="12">
      <p>
        آیا از حذف این گام
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
