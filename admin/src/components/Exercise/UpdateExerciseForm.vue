<script setup lang="ts">
import {IGetExercise} from "@/services/ExerciseService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {VForm} from "vuetify/components/VForm";
import type {VDataTable} from "vuetify/components";
import {IGetStep} from "@/services/StepsService";
import {isAxiosError} from "axios";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";
import {useAlerts} from "@/composables/alert";

const emits = defineEmits<{
  (e: 'refetch'): void,
  (e: 'updateExercise'): void,
}>()

const refVForm = ref(null)
const documentUrl = ref(null)
const imageUrl = ref(null)
const isRenderingCreateDialog = ref(false)
const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const exercisePayload: IGetExercise = defineModel()
const tableHeaders: VDataTable['headers'] = ref(
  [
    {title: 'شناسه', key: 'stepId'},
    {title: 'نام', key: 'name'},
    {title: 'عملیات', key: 'actions'},
  ]
)

function setFile(files: []) {
  if (files.length) {
    exercisePayload.value.documentLink = files[0]
  }
}

async function removeStep(step: IGetStep) {
  try {
    spinner.showSpinner()
    const response = await $api?.steps.removeStepFromExercise({
      stepId: step.stepId,
      excersiesId: step.exerciseId,
    })
    if (response.data.isSuccess) {
      alert.success('مرحله با موفقیت حذف شد !')
      emits('refetch')
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
    updateExercise()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

function setExerciseImage(medias) {
  if (medias.length) {
    exercisePayload.value.imageUrl = medias[0]
  }
}
function updateExercise() {
  documentUrl.value.getFiles()
  imageUrl.value.getFiles()
  emits('updateExercise')
}
</script>

<template>
  <VForm
    class="w-100"
    ref="refVForm"
    @submit.prevent="validateData"
  >
    <VRow>
      <VCol cols="12" md="6">
        <VTextField
          v-model="exercisePayload.title"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          label="نام تمرین "
        />
      </VCol>
      <VCol cols="12" md="6">
        <ExerciseCategoryPicker :return-object="false" required
                                v-model="exercisePayload.exerciseCategoryId"></ExerciseCategoryPicker>
      </VCol>
      <VCol cols="12" md="6">
        <ExerciseTypePicker :return-object="false" required v-model="exercisePayload.exerciseType"></ExerciseTypePicker>
      </VCol>
      <VCol cols="12" md="12">
        <Uploader :withPreview="false" :default-medias="exercisePayload.documentLink" ref="documentUrl"
                  @getFiles="setFile" label="فایل مربوطه"
                  :file-type="UploaderTypes.ExerciseDocument"></Uploader>
      </VCol>
      <VCol md="12" cols="12">
        <VTextarea
          v-model="exercisePayload.exerciseGoal"
          color="success"
          label="هدف تمرین"
          max="500"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          variant="outlined"
        />
      </VCol>
      <VCol md="12" cols="12">
        <VTextarea
          v-model="exercisePayload.practiceMethod"
          color="success"
          label="روش تمرین"
          max="500"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          variant="outlined"
        />
      </VCol>
      <VCol md="12" cols="12">
        <VTextField
          v-model="exercisePayload.exerciseEstimate"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          label="مدت زمان تمرین"
        />
      </VCol>
      <VCol md="12" cols="12">
        <VTextField
          v-model="exercisePayload.exerciseCount"
          type="number"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          label="تعداد مراحل تمرین"
        />
      </VCol>
      <VCol md="12" cols="12" class="">
        <Uploader ref="imageUrl"
                  :key="exercisePayload.imageUrl"
                  :default-medias="exercisePayload.imageUrl"
                  @getFiles="setExerciseImage" label="تصویر اصلی تمرین"
                  :file-type="UploaderTypes.Excersie"></Uploader>
      </VCol>
      <VCol md="12" cols="12" class="d-flex align-content-center justify-end">
        <VBtn
          type="submit"
          class="product-buy-now"
          color="green"
        >
          ویرایش تمرین
        </VBtn>
      </VCol>
      <VDivider></VDivider>
      <VCol md="12" cols="12" class="d-flex align-content-center justify-space-between">
        <strong class="text-xl">مراحل تمرین</strong>
        <VBtn
          @click="isRenderingCreateDialog = true"
          type="button"
          id="buy-now-btn"
          class="product-buy-now"
          color="green"
        >
          افزودن مرحله
        </VBtn>
      </VCol>
      <VCol v-if="exercisePayload.exerciseStepsDTOs" md="12" cols="12" class="d-flex justify-end">
        <CustomTable
          :items-list="exercisePayload.exerciseStepsDTOs"
          :count="exercisePayload.exerciseStepsDTOs.length"
          :page-number="1"
          :table-headers="tableHeaders"
          :total-count="exercisePayload.exerciseStepsDTOs.length"
        >

          <template #actions="data">
            <VBtn
              @click="removeStep(data.item)"
              color="transparent"
              elevation="0"
              icon="mdi-delete"
            />
          </template>
        </CustomTable>

      </VCol>
    </VRow>
    <AddStepToExerciseDialog
      @refetch="emits('refetch')"
      v-model:dialogState="isRenderingCreateDialog"></AddStepToExerciseDialog>
  </VForm>
</template>

<style scoped lang="scss">

</style>
