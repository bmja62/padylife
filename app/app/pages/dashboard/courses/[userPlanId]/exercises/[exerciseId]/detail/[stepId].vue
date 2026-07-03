<script setup lang="ts">
import type {IStepDetail, IStepOption, IUserStepAnswerPayload} from "~/services/StepService";
import type {IApiProvider} from "~/models/IApiProvider";
import MultiText from "~/components/Courses/QuestionTypes/MultiText.vue";
import VideoQuestion from "~/components/Courses/QuestionTypes/VideoQuestion.vue";
import TaskQuestion from "~/components/Courses/QuestionTypes/TaskQuestion.vue";
import CallToAction from "~/components/Courses/QuestionTypes/CallToAction.vue";
import SingleImage from "~/components/Courses/QuestionTypes/SingleImage.vue";
import TextQuestion from "~/components/Courses/QuestionTypes/TextQuestion.vue";
import type {IExerciseDetail, IExerciseStep} from "~/services/ExerciseService";
import {CommentType} from "~/models/Enums/CommentType";

interface IProps {
  stepDetail: IStepDetail,
  stepsCount:number
  currentStepIndex:number
  currentExercise: IExerciseDetail
  nextStep: IExerciseStep
}

const props: IProps = defineProps({
  stepDetail: {
    type: Object as PropType<IStepDetail>
  },
  currentExercise: {
    type: Object as PropType<currentExercise>
  },
  stepsCount: {
    type: Number as PropType<number>
  },
  nextStep: {
    type: Object as PropType<IExerciseStep>
  },
  currentStepIndex: {
    type: Number as PropType<number>
  },
})


const route = useRoute();
const router = useRouter();
const {$api} = useNuxtApp<IApiProvider>()
const alert = useAlerts()
const currentStepOption = ref<IStepOption>(null)
const chatMessage = ref<string>("");
const isCourseFinished = ref<boolean>(false);
const isRenderingExerciseReviewsDialog = ref<boolean>(false);
const optionComponents = shallowRef({
  MultipleChoice: MultiText,
  Video: VideoQuestion,
  Task: TaskQuestion,
  Action: CallToAction,
  Text: TextQuestion,
  Image: SingleImage,
})
const stepAnswerPayload = ref<IUserStepAnswerPayload>({
  excersieId: route.params.exerciseId,
  stepId: route.params.stepId,
  userPlanId: route.params.userPlanId,
  selectedStepOptionId: null,
  text: '',
  imageUrl: '',
  selectedChoiceIds: []
})


async function sendMessage() {
  if (chatMessage.value) {
  try {
    useSpinner().renderSpinner()
    stepAnswerPayload.value.selectedStepOptionId = currentStepOption.value.id
    const response = await $api.comment.createComment({
      entityId: currentStepOption.value.stepId,
      entityType: CommentType.Step,
      text: chatMessage.value,
      parentCommentId: null
    })
    if (response.isSuccess) {
      alert.success('تجربت با موفقیت ثبت شدش و پس از تایید به نمایش درمیاد')
      chatMessage.value = ''
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
  } else {
    alert.error('لطفا متن تجربه خودت رو وارد کن')
  }
}

function goNext() {
  if (props.nextStep) {
    checkHasAnswered()
    router.push(`/dashboard/courses/${route.params.userPlanId}/exercises/${route.params.exerciseId}/detail/${props.nextStep.stepId}`);
  } else {
    isCourseFinished.value = true;
  }
}

async function checkHasAnswered() {
  try {
    useSpinner().renderSpinner()
    stepAnswerPayload.value.selectedStepOptionId = currentStepOption.value.id
    const response = await $api.step.hasAnsweredStepOption({
      userPlanId: stepAnswerPayload.value.userPlanId,
      stepId: stepAnswerPayload.value.stepId,
      excersieId: stepAnswerPayload.value.excersieId,
    })
    if (response.isSuccess) {
      answerCurrentStep()
    } else {
    }

  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
async function answerCurrentStep() {
  try {
    useSpinner().renderSpinner()
    stepAnswerPayload.value.selectedStepOptionId = currentStepOption.value.id
    const response = await $api.step.createUserStepAnswer(stepAnswerPayload.value)
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
async function getStepOptions() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.step.getStepOptions(route.params.stepId)
    currentStepOption.value = response.data.data[0]
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getSelectedOptions(answers) {
  stepAnswerPayload.value.selectedChoiceIds = answers.length ? answers : [answers]
}
getStepOptions()

useHead({
  title: 'گام‌های تمرین'
})

</script>

<template>
  <div
      v-if="currentStepOption"
    class="w-full h-full custom-pattern-bg-image overflow-y-auto overflow-x-hidden"
  >

    <UtilsWrappersPageWrapper>
      <template #header>
        <BaseLogoHeader></BaseLogoHeader>
      </template>
      <CoursesCourseStepsHeader
        :steps-count="stepsCount"
        :current-step="currentStepIndex"
        :is-finished="isCourseFinished"
      ></CoursesCourseStepsHeader>
      <template v-if="!isCourseFinished">
        <strong class="text-center my-4">{{ stepDetail.name }}</strong>
        <CoursesDescriptionBox v-if="currentStepOption" :step-option="currentStepOption"></CoursesDescriptionBox>

        <template v-if="optionComponents[currentStepOption.type]">
          <component
              :is="optionComponents[currentStepOption.type]"
              :step-option="currentStepOption"
              class="mb-4"
              @set-selected-answers="getSelectedOptions"
          ></component>
        </template>

        <div class="w-full self-end mt-auto">

          <CoursesQuestionButtonsGroup
            class="mt-5"
            @next="goNext"
          ></CoursesQuestionButtonsGroup>
        </div>
      </template>
      <template v-else>
        <div class="w-full">
          <CoursesFinishPage :current-exercise="props.currentExercise"></CoursesFinishPage>
        </div>
      </template>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsExerciseReviewsDialog
        v-model="isRenderingExerciseReviewsDialog"
        :step="currentStepOption"></LazyUtilsDialogsExerciseReviewsDialog>
  </div>
</template>
