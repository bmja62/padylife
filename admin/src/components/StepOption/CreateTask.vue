<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateTaskOption} from "@/services/StepOptionService";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const emits = defineEmits<{
  (e: 'refetch')
}>()
const refVForm = ref(null)
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const route = useRoute()

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createTaskStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}


const taskPayload = ref<ICreateTaskOption>({
  deadlineDays: null,
  assigneeRole: "",
  taskInstructions: "",
  estimatedMinutes: null,
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

async function createTaskStep() {
  try {
    spinner.showSpinner()
    const response = await $api?.stepOption.createTask(taskPayload.value)
    if (response.data.isSuccess) {
      alert.success('گام با موفقیت ساخته شد')
      emits('refetch')
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
  <VForm
    ref="refVForm"
    @submit.prevent="validateData"
  >
    <VRow>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="taskPayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="taskPayload.assigneeRole"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="هدف از انجام این تکلیف"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="taskPayload.deadlineDays"
          color="success"
          required
          type="number"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label=" مهلت انجام تکلیف به روز"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="taskPayload.estimatedMinutes"
          color="success"
          required
          type="number"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label=" زمان تقریبی به دقیقه"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="taskPayload.description"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="taskPayload.taskInstructions"
          color="success"
          label="روش انجام تکلیف"
          variant="outlined"
        />
      </VCol>

      <VCol md="12" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="green"
        >
          ثبت
        </VBtn>
      </VCol>
    </VRow>
  </VForm>
</template>

<style scoped lang="scss">

</style>
