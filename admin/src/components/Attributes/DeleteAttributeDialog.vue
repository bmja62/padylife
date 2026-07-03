<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IAttribute} from "@/services/ProductAttributes";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IAttribute
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

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteProductAttribute() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()

      const response = await $api?.productAttributes.deleteProductAttribute(props.selectedItem.id)
      if (response.data.isSuccess) {
        alert.success('ویژگی با موفقیت حذف شد!')
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
    @update:dialog-state="updateDialogState"
    @delete="deleteProductAttribute"
  >
    <VCol cols="12">
      <p>
        آیا از حذف این ویژگی
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
