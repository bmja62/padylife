<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateActionOption} from "@/services/StepOptionService";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";

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
    createActionStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}


const actionPayload = ref<ICreateActionOption>({
  actionCommand: '',
  actionParameters: '',
  requiresConfirmation: false,
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

async function createActionStep() {
  try {
    spinner.showSpinner()
    const response = await $api?.stepOption.createAction(actionPayload.value)
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
          v-model.trim="actionPayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="actionPayload.actionCommand"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="نحو تعامل"
        />
      </VCol>
      <VCol cols="12" md="12">
        <FroalaEditor
          v-model="actionPayload.actionParameters"
          editor-placeholder="نکات کلیدی"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="actionPayload.description"
          color="success"
          label="توضیحات"
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
