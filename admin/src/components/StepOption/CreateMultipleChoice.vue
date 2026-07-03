<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import {ICreateMultiChoiceOption} from "@/services/StepOptionService";
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
const selectedAnswer = ref(null)


async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createMultiChoice()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}


const multiChoicePayload = ref<ICreateMultiChoiceOption>({
  choices: [
    {
      id: 1,
      text: "",
      isCorrect: false,
      order: 0
    }
  ],
  allowMultipleSelection: true,
  correctAnswerHint: "",
  stepId: route.params.id,
  title: "",
  description: "",
  order: 1
})

function addQuestionOption() {
  let tempId = multiChoicePayload.value.choices.length
  multiChoicePayload.value.choices.push({
    id: +tempId++,
    text: "",
    isCorrect: false,
    order: 0
  })
}

function sanitizeChoices() {
  if (!multiChoicePayload.value.allowMultipleSelection) {
    const idx = multiChoicePayload.value.choices.findIndex(e => e.id === selectedAnswer.value.id)
    if (idx > -1) {
      multiChoicePayload.value.choices[idx].isCorrect = true
    }
  } else {
    multiChoicePayload.value.choices.forEach((item) => {
      item.isCorrect = false
    })
  }
}
async function createMultiChoice() {
  try {
    spinner.showSpinner()
    sanitizeChoices()
    const response = await $api?.stepOption.createMultiChoice(multiChoicePayload.value)
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
          v-model.trim="multiChoicePayload.title"
          color="success"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextarea
          v-model="multiChoicePayload.description"
          color="success"
          label="توضیحات"
          variant="outlined"
        />
      </VCol>
      <VCol cols="12" md="12">
        <VTextField
          v-model.trim="multiChoicePayload.correctAnswerHint"
          color="success"
          label="راهنمایی پاسخ صحیح"
        />
      </VCol>
      <VCol cols="12" md="12">
        <CustomSwitch
          label="قابلیت انتخاب چند جواب"
          id="customizer-menu-collapsed"
          :has-tooltip="false"
          v-model="multiChoicePayload.allowMultipleSelection"
          class="ms-2"
        />
      </VCol>
      <VCol @click="addQuestionOption" cols="12" md="6" class="d-flex align-content-center gap-2 cursor-pointer">
        <VIcon icon="mdi-plus-circle" color="primary" class="" size="30"></VIcon>
        <span class="mt-1">افزودن گزینه جدید</span>
      </VCol>
      <VCol cols="12" md="12" v-for="(item,idx) in multiChoicePayload.choices.length"
            class="d-flex align-content-center justify-center gap-2">
        <template
          v-if="!multiChoicePayload.allowMultipleSelection"
        >
        <small class="mt-2">پاسخ صحیح</small>
          <input type="radio" name="selectable" :value="multiChoicePayload.choices[idx]" v-model="selectedAnswer">
        </template>
        <VTextField
          v-model="multiChoicePayload.choices[idx].text"
          type="text"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          @keydown.tab="addQuestionOption"
          :label="`${item}) متن گزینه را وارد کنید`"
        />
        <VIcon icon="mdi-close" color="error" class="mt-1 cursor-pointer"
               @click="multiChoicePayload.choices.splice(idx,1)" size="30"></VIcon>
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
