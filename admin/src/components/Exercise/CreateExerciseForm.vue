<script setup lang="ts">
import {IExercise} from "@/services/ExerciseService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {VForm} from "vuetify/components/VForm";

const emits = defineEmits<{
  (e: 'changeSlug', slugName: string): void
}>()

const refVForm = ref(null)
const documentUrl = ref(null)
const imageUrl = ref(null)
const exercisePayload: IExercise = defineModel()

async function changeSlug() {
  documentUrl.value.getFiles()
  imageUrl.value.getFiles()
  emits('changeSlug', 'ExerciseSteps')
}

function setFile(files: []) {
  if (files.length) {
    exercisePayload.value.documentLink = files[0]
  }
}

function setExerciseImage(medias) {
  if (medias.length) {
    exercisePayload.value.imageUrl = medias[0]
  }
}
</script>

<template>
  <VForm
    class="w-100"
    ref="refVForm"
    @submit.prevent="changeSlug"
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
                  @getFiles="setExerciseImage" label="تصویر اصلی تمرین"
                  :file-type="UploaderTypes.Excersie"></Uploader>
      </VCol>
      <VCol md="12" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="green"
        >
          بعدی
        </VBtn>

      </VCol>
    </VRow>
  </VForm>
</template>

<style scoped lang="scss">

</style>
