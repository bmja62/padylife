<script setup lang="ts">
import { useSpinner } from "@/composables/spinner";
import { IApiProvider } from "@/models/IApiProvider";
import { inject } from "vue";
import { useAlerts } from "@/composables/alert";
import { useRouter } from "vue-router";
import { IGetQuestion, IQuestionOption } from "@/services/QuestionService";
import { VForm } from "vuetify/components/VForm";

const spinner = useSpinner()

const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const router = useRouter()
const route = useRoute()
const currentQuestion = ref<IGetQuestion | null>(null)
const isRenderingCreateDialog = ref(false)
const refVForm = ref(null)
async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateQuestion()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateQuestion() {
  try {
    spinner.showSpinner()
    const data = await $api?.question.updateQuestion(currentQuestion.value, currentQuestion.value.id)
    if (data.data.isSuccess) {
      alert.success('سوال با موفقیت بروزرسانی شد')
      getQuestionById()
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function getQuestionById() {
  try {
    spinner.showSpinner()
    const data = await $api?.question.getQuestionById(route.params.id)
    if (data.data.isSuccess) {
      currentQuestion.value = data.data.data
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function removeQuestionOption(questionOption: IQuestionOption, idx: number) {
  try {
    spinner.showSpinner()
    const data = await $api?.question.removeQuestionOption(questionOption.id)
    if (data.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      currentQuestion.value.options.splice(idx, 1)
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function updateQuestionOption(questionOption: IQuestionOption, idx: number) {
  try {
    spinner.showSpinner()
    const data = await $api?.question.updateQuestionOption({
      questionOptionId: questionOption.id,
      text: questionOption.text,
      priority: questionOption.priority
    })
    if (data.data.isSuccess) {
      useAlerts().success('عملیات با موفقیت انجام شد')
      getQuestionById()
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}



onMounted(() => {
  getQuestionById()
})
</script>

<template>
  <PageWrapper v-if="currentQuestion">
    <template #title>
      ویرایش سوال
    </template>

    <VForm class="w-100" ref="refVForm" @submit.prevent="validateData">
      <VRow>
        <VCol cols="12" md="6">
          <VTextField v-model="currentQuestion.displayText" type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']" required label="عنوان نمایشی سوال" />
        </VCol>
        <VCol cols="12" md="6">
          <QuestionCategoryPicker required v-model="currentQuestion.questionCategoryId"></QuestionCategoryPicker>
        </VCol>
        <VCol cols="12" md="12">
          <FroalaEditor v-model="currentQuestion.text" editor-placeholder="متن سوال را وارد کنید *" />

        </VCol>
        <VCol md="12" cols="12" class="d-flex justify-end">
          <VBtn type="submit" id="buy-now-btn" class="product-buy-now" color="green">
            ویرایش
          </VBtn>
        </VCol>
        <VCol @click="isRenderingCreateDialog = true" cols="12" md="6"
          class="d-flex align-content-center gap-2 cursor-pointer">
          <VIcon icon="mdi-plus-circle" color="primary" class="" size="30"></VIcon>
          <span class="mt-1">افزودن گزینه جدید</span>

        </VCol>
        <VCol cols="12" md="12" v-for="(item, idx) in currentQuestion.options"
          class="d-flex align-content-center gap-2">
          <VRow>
            <VCol cols="12" md="1">
              <VTextField v-model="currentQuestion.options[idx].priority" type="text" :label="`اولویت `" />
            </VCol>
            <VCol cols="12" md="11">
              <VTextField v-model="currentQuestion.options[idx].text" type="text"
                :label="`${idx + 1}) متن گزینه را وارد کنید`" />
            </VCol>
          </VRow>
          <VIcon icon="mdi-edit" color="primary" class="mt-1 cursor-pointer" @click="updateQuestionOption(item, idx)"
            size="30"></VIcon>
          <VIcon icon="mdi-close" color="error" class="mt-1 cursor-pointer" @click="removeQuestionOption(item, idx)"
            size="30"></VIcon>
        </VCol>


      </VRow>
    </VForm>
    <AddOptionToQuestionDialog @refetch="getQuestionById" v-model:dialogState="isRenderingCreateDialog">
    </AddOptionToQuestionDialog>
  </PageWrapper>
</template>

<style scoped lang="scss"></style>
