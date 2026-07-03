<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IPlan} from "@/services/PlanService";
import PlanStatusPicker from "@/components/Plan/PlanStatusPicker.vue";

// Interfaces
interface Props {
  dialogState: boolean,
  defaultPlan: IPlan
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

const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions
async function changePlanStatus() {
  try {
    spinner.showSpinner()

    const response = await $api?.plan.changePlanStatus({
      id: props.defaultPlan.id,
      status: props.defaultPlan.status
    })
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response.data.errorMessage)
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


</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="تغییر وضعیت پلن"
    @update:dialog-state="updateDialogState"
    @create="changePlanStatus"
  >
    <VRow>
      <VCol cols="12">
        <PlanStatusPicker :clearable="false" v-model="defaultPlan.status"></PlanStatusPicker>
      </VCol>
    </VRow>

  </CustomCreateDialog>
</template>
