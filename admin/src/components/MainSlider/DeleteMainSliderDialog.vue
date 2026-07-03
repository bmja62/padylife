<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

interface ITempBrand extends ICreateOrUpdateBrandPayload {
  id: string | number
}

interface Props {
  dialogState: boolean
  defaultSlider: ITempBrand
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultSlider: () => {
    return {
      id: 0,
      title: null,
      description: null,
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

async function deleteSlider() {
  if (props.defaultSlider) {
    try {
      spinner.showSpinner()

      const response = await $api?.products.deleteMainSlider(props.defaultSlider.id)
      alert.success('بنر با موفقیت حذف شد!')
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
</script>

<template>
  <CustomDeleteDialog
    :dialog-state="props.dialogState"
    :title="`حذف بنر `"
    @update:dialog-state="updateDialogState"
    @delete="deleteSlider"
  >
    <VCol cols="12">
      <p>
        آیا از حذف بنر
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
