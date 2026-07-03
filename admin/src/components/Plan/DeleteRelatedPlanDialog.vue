<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IRelatedPlan} from "@/services/RelatedPlans";


interface Props {
  dialogState: boolean
  selectedItem: IRelatedPlan
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  selectedItem: () => {
    return {
      name: null,
    }
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteRelatedPlan() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.relatedPlans.deletePlanRelation({
        targetPlanId: props.selectedItem.targetPlanId,
        sourcePlanId: route.params.id
      })
      if (response.data.isSuccess) {

        alert.success('اتصال با موفقیت حذف شد!')
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
    :title="`حذف اتصال دسته بندی`"
    @update:dialog-state="updateDialogState"
    @delete="deleteRelatedPlan"
  >
    <VCol cols="12">
      <p>
        آیا از انجام این عملیات
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
