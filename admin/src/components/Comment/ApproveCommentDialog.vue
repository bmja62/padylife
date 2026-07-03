<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IEntityComment} from "@/services/CommentService";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IEntityComment
}

// Props
const props = withDefaults(defineProps<IProps>(), {})

// Emits
const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function approveComment() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.comment.approve(props.selectedItem.id)
      if (response.data.isSuccess) {
        alert.success('عملیات با موفقیت انجام شد!')
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
  <CustomUpdateDialog
    action-text="بله"
    :dialog-state="props.dialogState"
    title="تایید و نمایش این تجربه"
    @update:dialog-state="updateDialogState"
    @update="approveComment"
  >
    <VRow>
      <VCol md="12" cols="12">
        <strong>
          {{ props.selectedItem.text }}
        </strong>
      </VCol>
      <VCol md="12" cols="12" class="">
        <span>آیا از تایید و نمایش این تجربه اطمینان دارید ؟</span>

      </VCol>
    </VRow>
  </CustomUpdateDialog>
</template>
