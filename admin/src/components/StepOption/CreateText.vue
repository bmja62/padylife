<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateTextOption} from "@/services/StepOptionService";
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
    createTextStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}


const textPayload = ref<ICreateTextOption>({
  content: "",
  isHtml: false,
  textFormat: "",
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

async function createTextStep() {
  try {
    spinner.showSpinner()
    const response = await $api?.stepOption.createText(textPayload.value)
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
          v-model.trim="textPayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="textPayload.textFormat"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="فرمت متن ورودی"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="textPayload.description"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12">
        <CustomSwitch v-model="textPayload.isHtml" :has-tooltip="false"
                      label="آیا به ویرایشگر متن نیاز دارید ؟"></CustomSwitch>
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-if="!textPayload.isHtml"
          v-model="textPayload.content"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
        <FroalaEditor
          v-else
          v-model="textPayload.content"
          editor-placeholder="توضیحات"
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
