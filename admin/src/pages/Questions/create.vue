<script setup lang="ts">
import { useSpinner } from "@/composables/spinner";
import { IApiProvider } from "@/models/IApiProvider";
import { inject } from "vue";
import { useAlerts } from "@/composables/alert";
import { useRouter } from "vue-router";
import { VForm } from "vuetify/components/VForm";
import { IQuestion } from "@/services/QuestionService";

const spinner = useSpinner()
const questionPayload = ref<IQuestion>({
  questionCategoryId: null,
  text: "",
  displayText: '',
  questionOptions: []
})
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const refVForm = ref(null)
const router = useRouter()

function addQuestionOption() {
  questionPayload.value.questionOptions.push({
    text: ''
  })
}

async function createQuestion() {
  try {
    spinner.showSpinner()
    const data = await $api?.question.createQuestion(questionPayload.value)
    if (data.data.isSuccess) {
      alert.success('تمرین با موفقیت ساخته شد')
      router.push('/Questions/list')
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    if (questionPayload.value.questionOptions.length) {

      createQuestion()
    } else {
      alert.error('لطفا حداقل یک گزینه برای سوال وارد کنید')
    }
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <PageWrapper>
    <template #title>
      ایجاد یک سوال جدید
    </template>
    <VForm class="w-100" ref="refVForm" @submit.prevent="validateData">
      <VRow>
        <VCol cols="12" md="6">
          <VTextField v-model="questionPayload.displayText" type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']" required label="عنوان نمایشی سوال" />
        </VCol>
        <VCol cols="12" md="6">
          <QuestionCategoryPicker required v-model="questionPayload.questionCategoryId"></QuestionCategoryPicker>
        </VCol>
        <VCol cols="12" md="12">
          <FroalaEditor v-model="questionPayload.text" editor-placeholder="متن سوال را وارد کنید *" />
        </VCol>
        <VCol @click="addQuestionOption" cols="12" md="6" class="d-flex align-content-center gap-2 cursor-pointer">
          <VIcon icon="mdi-plus-circle" color="primary" class="" size="30"></VIcon>
          <span class="mt-1">افزودن گزینه جدید</span>

        </VCol>
        <VCol cols="12" md="12" v-for="(item, idx) in questionPayload.questionOptions.length"
          class="d-flex align-content-center gap-2">
          <VTextField v-model="questionPayload.questionOptions[idx].text" type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']" required @keydown.tab="addQuestionOption"
            :label="`${item}) متن گزینه را وارد کنید`" />
          <VIcon icon="mdi-close" color="error" class="mt-1 cursor-pointer"
            @click="questionPayload.questionOptions.splice(idx, 1)" size="30"></VIcon>
        </VCol>

        <VCol md="12" cols="12" class="d-flex justify-end">
          <VBtn type="submit" id="buy-now-btn" class="product-buy-now" color="green">
            ثبت
          </VBtn>
        </VCol>
      </VRow>
    </VForm>

  </PageWrapper>
</template>

<style scoped lang="scss"></style>
