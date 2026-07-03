<script setup lang="ts">
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";
import {VForm} from "vuetify/components/VForm";
import {IPlanGet, IPlanQuestion, IPlanQuestionOption} from "@/services/PlanService";
import {InternalPlanQuestionOptionActions} from "@/models/Enums/InternalPlanQuestionOptionActions";
import type {VDataTable} from "vuetify/components";
import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import RelatedPlansPage from "@/components/Plan/RelatedPlansPage.vue";
import PlanDiscount from "@/components/Plan/PlanDiscount.vue";

const spinner = useSpinner()
const currentPlan = ref<IPlanGet>(null)
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const refVForm = ref(null)
const imageUrl = ref(null)
const refVFormMainQuestion = ref(null)
const isRenderingLinkDialog = ref(false)
const router = useRouter()
const route = useRoute()
const selectedProperty = ref<IPlanQuestionOption>(null)
const selectedAction = ref<InternalPlanQuestionOptionActions | null>(null)
const addPlanQuestionPayload = ref({
  isMainQuestion: false,
  questionId: null,
  planId: route.params.id
})
const totalCount = ref(null)
const isFree = ref(false)
const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'planQuestionId'},
  {title: 'عنوان سوال', key: 'displayText'},
  // {title: 'عملیات', key: 'actions'},
]
const planQuestionsListFilters = ref({
  planId: route.params.id,
  pageNumber: 1,
  count: 10
})
const planQuestionsList = ref([])
async function updatePlan() {
  try {
    spinner.showSpinner()
    //  don ask why, the dto from backend includes more items so i had to create mine before requesting, see type definitions for further explanations
    imageUrl.value.getFiles()
    const data = await $api?.plan.createOrUpdatePlan({
      planCategoryId: currentPlan.value.planCategoryId,
      title: currentPlan.value.title,
      id: currentPlan.value.id,
      description: currentPlan.value.description,
      level: currentPlan.value.level,
      price: currentPlan.value.price,
      imageUrl: currentPlan.value.imageUrl,
      isSignUpPlan:currentPlan.value.isSignUpPlan
    })
    if (data.data.isSuccess) {
      alert.success('پلن با موفقیت ویرایش شد')
      getPlanById()
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

function setFile(medias) {
  if (medias.length) {
    currentPlan.value.imageUrl = medias[0]
  }
}

async function getPlanById() {
  try {
    spinner.showSpinner()
    const data = await $api?.plan.getPlanById(route.params.id)
    if (data.data.isSuccess) {
      currentPlan.value = data.data.data
      generateItemsForTreeView()
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function getPlanQuestions() {
  try {
    spinner.showSpinner()
    const data = await $api?.plan.getPlanQuestions(planQuestionsListFilters.value)
    if (data.data.isSuccess) {
      planQuestionsList.value = data.data.data.data
      totalCount.value = data.data.data.totalCount
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

function generateItemsForTreeView() {
  currentPlan.value.planQuestions.forEach((question: IPlanQuestion) => {
    searchRecursiveForQuestionOptions(question)
  })
}

function generateChildrenBasedOnQuestionOptions(question: IPlanQuestion): IPlanQuestionOption[] {
  const resultArray = []
  let tempObj = {
    linkedExercise: null,
    linkedExercises: null,
    linkedQuestion: null,
    linkedQuestionId: null,
    questionId: null,
    id: null,
    text: null
  }
  for (let i = 0; i < question.readOnlyQuestionOptions.length; i++) {
    tempObj = {
      linkedQuestion: question.questionOptions[i] ? question.questionOptions[i].linkedQuestion : null,
      linkedQuestionText: question.questionOptions[i] && question.questionOptions[i].linkedQuestion ? question.questionOptions[i].linkedQuestion.text : null,
      questionId: question.questionId,
      planQuestionId: question.planQuestionId,
      linkedExercise: question.questionOptions[i] ? question.questionOptions[i].linkedExercise : null,
      linkedPlanQuestionId: question.questionOptions[i] ? question.questionOptions[i].linkedPlanQuestionId : null,
      linkedExercises: question.questionOptions[i] ? question.questionOptions[i].linkedExercises : null,
      text: question.readOnlyQuestionOptions[i].text,
      id: question.readOnlyQuestionOptions[i].id,
    }
    resultArray.push(tempObj)
  }
  return resultArray
}


function searchRecursiveForQuestionOptions(question: IPlanQuestion) {
  if (question.readOnlyQuestionOptions.length) {
    // the children property is mapped alongside questionOptions with no difference, only because the tree view component accepts children property to scan nested into it
    question['children'] = generateChildrenBasedOnQuestionOptions(question)
    for (let i = 0; i < question.questionOptions.length; i++) {
      if (question.children[i].linkedQuestion) {
        // again, the children property is mapped out of linkedQuestion because the tree view component cannot go nested into properties of object.
        // it is only looking for the children property in the flat object
        if (question.children[i].linkedQuestion.readOnlyQuestionOptions.length) {
          question.children[i]['children'] = [question.children[i].linkedQuestion]
        }
        searchRecursiveForQuestionOptions(question.questionOptions[i].linkedQuestion)
      }
    }
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updatePlan()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function validateMainQuestion() {
  const isValid: Object = await refVFormMainQuestion?.value?.validate();
  if (isValid.valid) {
    addQuestionToPlan()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function addQuestionToPlan() {
  try {
    spinner.showSpinner()
    const data = await $api?.plan.addPlanQuestion(addPlanQuestionPayload.value)
    console.log(data)
    if (data.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      getPlanById()
      getPlanQuestions()
      addPlanQuestionPayload.value = {
        planId: route.params.id,
        questionId: null,
        isMainQuestion: false,
      }
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function changePage(pageNumber: number | string) {
  planQuestionsListFilters.value.pageNumber = +pageNumber
  await getPlanById()
}
function setSelectedProperty(item: IPlanQuestionOption, action: InternalPlanQuestionOptionActions) {
  selectedProperty.value = item
  selectedAction.value = action
  renderLinkDialog()
}

function renderLinkDialog() {
  isRenderingLinkDialog.value = true
}


const isMainQuestionPresent = computed(() => {
  let isPresent = false
  if (planQuestionsList.value.length) {
    const idx = planQuestionsList.value.findIndex(e => e.isMain)
    if (idx > -1) {
      isPresent = true
    }
  }
  return isPresent
})

function refetchAll() {
  getPlanById()
  getPlanQuestions()
}
onMounted(() => {
  getPlanById()
  getPlanQuestions()
})
const formattedPrice = computed({
  get: function () {
    return currentPlan.value.price;
  },
  set: function (newValue) {
    if (newValue && newValue !== "") {
      // Remove all characters that are NOT numbers
      const cleanedValue = newValue.replace(/[^\d]/g, "");

      // Format the cleaned value with commas
      if (cleanedValue) {
        currentPlan.value.price = Number(cleanedValue).toLocaleString("en-US");
      } else {
        currentPlan.value.price = null;
      }
    } else {
      currentPlan.value.price = null;
    }
  },
});
</script>

<template>
  <div class="w-100">
  <PageWrapper style="overflow-x: auto">
    <template #title>
      ویرایش پلن
    </template>
    <VForm
      v-if="currentPlan"
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <VRow>
        <VCol cols="12" md="6">
          <PlanCategoryPicker required v-model="currentPlan.planCategoryId"></PlanCategoryPicker>
        </VCol>
        <VCol cols="12" md="6">
          <VTextField
            v-model="currentPlan.title"
            type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']"
            required
            label="عنوان پلن"
          />
        </VCol>
        <VCol cols="12" md="12">
          <VTextField
            v-model="currentPlan.level"
            type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']"
            required
            label="سطح"
          />
        </VCol>
        <VCol md="12" cols="12" class="">
          <Uploader ref="imageUrl"
                    :key="currentPlan.imageUrl"
                    :default-medias="currentPlan.imageUrl"
                    @getFiles="setFile" label="تصویر پلن"
                    :file-type="UploaderTypes.Plan"></Uploader>
        </VCol>
        <VCol cols="12" md="6">
          <CustomSwitch
            v-model="isFree"
            label="پلن رایگان"
            id="customizer-menu-collapsed"
            :has-tooltip="false"
            class="ms-2"
          />
          <VTextField
            v-if="!isFree"
            v-model="formattedPrice"
            type="text"
            :rules="[!isFree ? (value) => !!value || 'این فیلد اجباری است' : '']"
            required
            label="قیمت پلن (تومان)"
          />
        </VCol>
        <VCol cols="12" md="12">
          <FroalaEditor
            v-model="currentPlan.description"
            editor-placeholder="توضیحات پلن *"
          />
        </VCol>
        <VCol md="12" cols="12" class="d-flex justify-end">
          <VBtn
            type="submit"
            id="buy-now-btn"
            class="product-buy-now"
            color="green"
          >
            ویرایش
          </VBtn>
        </VCol>
      </VRow>
    </VForm>
    <VDivider class="my-4"></VDivider>

    <template
      v-if="currentPlan"
    >
      <VForm
        class="w-100 mb-2"
        ref="refVFormMainQuestion"
        @submit.prevent="validateMainQuestion"
      >
        <VRow>
          <VCol cols="12" md="12">
            <strong class="text-xl">
              نقشه راه
            </strong>
          </VCol>
          <VCol
            cols="12" md="12">
            <QuestionPicker required v-model="addPlanQuestionPayload.questionId"></QuestionPicker>
          </VCol>
          <VCol
            v-if="!isMainQuestionPresent"
            cols="12" md="12">
            <CustomSwitch v-model="addPlanQuestionPayload.isMainQuestion" label="آیا سوال اصلی است ؟"></CustomSwitch>
          </VCol>
          <VCol
            md="12" cols="12" class="d-flex justify-end">
            <VBtn
              type="submit"
              id="buy-now-btn"
              class="product-buy-now"
              color="warning"
            >
              افزودن سوال
            </VBtn>
          </VCol>
        </VRow>
      </VForm>
      <CustomTable
        :items-list="planQuestionsList"
        :count="planQuestionsListFilters.count"
        :page-number="planQuestionsListFilters.pageNumber"
        :table-headers="tableHeaders"
        :total-count="totalCount"
        @change-page="changePage"
      >
        <template #text="data">
          <div v-html="data.item.text"></div>
          <!--          <VBtn-->
          <!--            color="transparent"-->
          <!--            elevation="0"-->
          <!--            icon="mdi-delete"-->
          <!--            @click="renderDeleteDialog(data.item as ItempPlan)"-->
          <!--          />-->
        </template>
      </CustomTable>
      <VDivider></VDivider>
      <CustomTreeView style="min-width: max-content;overflow-x: auto" @getSelectedProperty="setSelectedProperty" v-if="currentPlan"
                      :items="currentPlan.planQuestions"></CustomTreeView>
    </template>

    <LinkToPlanDialog
      v-model:dialogState="isRenderingLinkDialog"
      :default-item="selectedProperty"
      :type="selectedAction"
      @refetch="refetchAll"
    />
  </PageWrapper>
    <PlanDiscount></PlanDiscount>
    <RelatedPlansPage></RelatedPlansPage>
  </div>

</template>

<style scoped lang="scss">

</style>
