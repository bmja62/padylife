<script lang="ts" setup>
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {InternalPointsActionType, IPointsPayload} from "@/services/PointsService";
import {VForm} from "vuetify/components/VForm";

// Interfaces
interface Props {
  dialogState: boolean
  actionType?: InternalPointsActionType
}
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  dialogState: false,
  actionType: null
})

const emit = defineEmits<Emit>()
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const route = useRoute()
const refVForm = ref(null)
const pointsPayload = ref<IPointsPayload>({
  userId: route.params.id,
  amount: null,
  reason: '',
  referenceId: null,
  referenceType: "Product"
})

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    if (props.actionType === InternalPointsActionType.Consume) {
      consumeUserPoints()
    } else if (props.actionType === InternalPointsActionType.Earn)
      earnUserPoints()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function consumeUserPoints() {
  try {
    spinner.showSpinner()
    const response = await $api?.points.consumeUserPoints(pointsPayload.value)
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      emit('update:dialogState', false)
      emit('refetch')
      pointsPayload.value = {
        userId: route.params.id,
        amount: null,
        reason: '',
        referenceId: null,
        referenceType: "Product"
      }
    } else {
      alert.error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function earnUserPoints() {
  try {
    spinner.showSpinner()
    const response = await $api?.points.earnUserPoints(pointsPayload.value)
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      emit('update:dialogState', false)
      emit('refetch')
      pointsPayload.value = {
        userId: route.params.id,
        amount: null,
        reason: '',
        referenceId: null,
        referenceType: "Product"
      }
    } else {
      alert.error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    :title="`${props.actionType === InternalPointsActionType.Earn ? 'افزایش' : 'کسر'} امتیاز کاربر`"
    @create="validateData"
    @update:dialog-state="updateDialogState"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>

        <VCol cols="12">
          <VTextField
            v-model.trim="pointsPayload.amount"
            color="success"
            density="compact"
            label="میزان امتیاز"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
            variant="outlined"
          />
        </VCol>
        <VCol cols="12">
          <EntityTypePicker required dropdown-label="انتخاب دسته بندی امتیاز"
                            v-model="pointsPayload.referenceType"></EntityTypePicker>
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="pointsPayload.referenceId"
            color="success"
            density="compact"
            :label="`شناسه را وارد کنید`"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
            type="number"
            variant="outlined"
          />
        </VCol>
        <VCol cols="12">
          <VTextarea
            v-model.trim="pointsPayload.reason"
            variant="outlined"
            density="compact"
            label="توضیحات"
            color="success"
          />
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
