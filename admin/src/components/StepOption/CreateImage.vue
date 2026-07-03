<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateImageOption} from "@/services/StepOptionService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const emits = defineEmits<{
  (e: 'refetch')
}>()
const refVForm = ref(null)
const spinner = useSpinner()
const imageUrl = ref(null)
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const route = useRoute()

async function validateData() {
  imageUrl.value.getFiles()
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createImageStep()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

function setFile(medias) {
  if (medias.length) {
    imagePayload.value.imageUrl = medias[0]
  }
}

const imagePayload = ref<ICreateImageOption>({
  imageUrl: "",
  altText: "",
  caption: "",
  width: null,
  height: null,
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

async function createImageStep() {
  try {
    spinner.showSpinner()
    imagePayload.value.altText = imagePayload.value.title
    const response = await $api?.stepOption.createImage(imagePayload.value)
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
          v-model.trim="imagePayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="6">
        <VTextField
          v-model.trim="imagePayload.caption"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="کپشن عکس"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="imagePayload.description"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12">
        <Uploader accept="image/*" required :default-medias="imagePayload.imageUrl" ref="imageUrl"
                  @getFiles="setFile" label="عکس اصلی"
                  :file-type="UploaderTypes.StepOption"></Uploader>
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
