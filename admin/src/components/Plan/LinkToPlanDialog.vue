<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IPlanQuestionOption} from "@/services/PlanService";
import {InternalPlanQuestionOptionActions} from "@/models/Enums/InternalPlanQuestionOptionActions";

// Interfaces
interface Props {
  dialogState: boolean
  defaultItem: IPlanQuestionOption,
  type: InternalPlanQuestionOptionActions
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
const selectedId = ref(null)
const route = useRoute()
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions
async function linkQuestionToPlan() {
  try {
    spinner.showSpinner()
    const response = await $api?.plan.createOrUpdateLinkedQuestion({
      linkedExerciseId: null,
      linkedPlanQuestionId: selectedId.value,
      planId: route.params.id,
      planQuestionId: props.defaultItem.planQuestionId,
      questionOptionId: props.defaultItem.id

    })
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد!')
      emit('update:dialogState', false)
      emit('refetch')
      selectedId.value = null
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

async function linkExerciseToPlan() {
  let tempPriority = 1
  const exerciseLinks = selectedId.value.map((item) => {
    return {
      exerciseId: item,
      priority: tempPriority++
    }
  })
  try {
    spinner.showSpinner()
    const response = await $api?.plan.createOrUpdateLinkedQuestion({
      exerciseLinks: exerciseLinks,
      linkedPlanQuestionId: null,
      planId: route.params.id,
      planQuestionId: props.defaultItem.planQuestionId,
      questionOptionId: props.defaultItem.id
    })
    if (response.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد!')
      emit('update:dialogState', false)
      emit('refetch')
      selectedId.value = null

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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    if (props.type === InternalPlanQuestionOptionActions.Question) {
      linkQuestionToPlan()
    } else if (props.type === InternalPlanQuestionOptionActions.Exercise) {
      linkExerciseToPlan()
    }
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    :title="props.type === InternalPlanQuestionOptionActions.Question ? 'اتصال به سوال' : 'اتصال به تمرین' "
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent
    >
      <VCol v-if="props.type === InternalPlanQuestionOptionActions.Question" cols="12">
        <PlanQuestionPicker filterIsMain required v-model="selectedId"></PlanQuestionPicker>
      </VCol>
      <VCol v-if="props.type === InternalPlanQuestionOptionActions.Exercise" cols="12">
        <ExercisePicker multiple chips required v-model="selectedId"></ExercisePicker>
      </VCol>
    </VForm>

  </CustomCreateDialog>
</template>
