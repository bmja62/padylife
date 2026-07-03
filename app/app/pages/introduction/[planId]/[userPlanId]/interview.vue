<script setup lang="ts">

definePageMeta({
  auth: true
})
interface IMessage {
  text: string;
  isMe: boolean;
  hasTail: boolean;
}

const router = useRouter();
const route = useRoute();
const { $api } = useNuxtApp()
const alert = useAlerts()
const authStore = useAuthStore()
const currentQuestion = ref<any>(null)
const currentPlan = ref<any>(null)
const messageRef = useTemplateRef("message");
const selectedItem = ref(null);
const messageList = ref<IMessage[]>([
  // {
  //   text: "سلام",
  //   isMe: false,
  //   hasTail: true,
  // },
  // {
  //   text: "ما اینجاییم تا با هم یک سری سوالات رو جواب بدیم تا بیشتر بشناسیمت",
  //   isMe: false,
  //   hasTail: false,
  // },
]);
const isRenderingButtons = ref<boolean>(false);
const isRenderingEnterAppButton = ref<boolean>(false);
const isRenderingChoosePartnerDialog = ref<boolean>(false);
const chatMessage = ref<string>("از آشناییتون خوشوقتم!");
useHead({
  title: 'پاسخ به سوالات'
})
function justSendMessage(text: string, isMe: boolean) {
  messageList.value.push({
    text: text,
    isMe: isMe,
    hasTail: true,
  });
  nextTick(() => {
    scrollToBottom();
  });
}
function sendMessage(isMe: boolean) {
  // justSendMessage(chatMessage.value, isMe)
  processMessage()
}
function processMessage() {
  // if (chatMessage.value == "از آشناییتون خوشوقتم!") {
  //   chatMessage.value = "";
  //   setTimeout(() => {
  //     justSendMessage("بیا شروع کنیم", false)
  //     justSendMessage("من چند تا سوال ازت می‌پرسم، لطفا با دقت جواب بده.", false)
  //     chatMessage.value = "باشه. حتما";
  //   }, 700);
  // } else if (chatMessage.value == "باشه. حتما") {
  //   chatMessage.value = "";
  //   setTimeout(() => {
  //   }, 700);
  // }
  //     setCurrentQuestion()
  //     isRenderingButtons.value = true;
  //     nextTick(() => {
  //       scrollToBottom();
  //     });
}
function exactlySetQuestion(data) {
  currentQuestion.value = {
    planQuestionId: data?.id,
    text: data?.text,
    questionOptions: data?.options
  }
}
function setCurrentQuestion() {

  exactlySetQuestion(currentPlan.value.nextUnansweredQuestion)
  justSendMessage(currentQuestion.value.text, false)
}
async function createUserAnswer() {
  if (selectedItem.value) {
    try {
      const response = await $api.plan.createUserAnswer({
        selectedQuestionOptionId: selectedItem.value.id,
        userPlanId: +route.params.userPlanId,
        planQuestionId: currentQuestion.value.planQuestionId
      })
      if (response.isSuccess) {
        justSendMessage(selectedItem.value.text, true)
        if (response?.data?.question) {
          exactlySetQuestion(response.data.question)
          justSendMessage(currentQuestion.value.text, false)
        } else if (response?.data?.excersie) {
          isRenderingButtons.value = false;
          isRenderingEnterAppButton.value = true
          justSendMessage('مرسی که وقت گذاشتی! با بازگشت به داشبورد، تمرینت رو انتخاب کن و شروع کن', false)
        }
        selectedItem.value = null
      } else {
        alert.error(response.message)
      }
    } catch (e) {
      console.log(e)
    }
  } else {
    useAlerts().error('لطفا یک پاسخ را انتخاب کنید')
  }
}

function scrollToBottom() {
  if (
    messageRef.value &&
    messageRef.value.length &&
    messageRef.value.length - 1 &&
    messageRef.value[messageRef.value.length - 1]
  ) {
    messageRef.value[messageRef.value.length - 1]?.scrollIntoView();
  }
}

async function getUserPlanStatus() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.plan.getUserPlans({
      planId: route.params.planId,
      userId: authStore.getUser.id,
      pageNumber: 1,
      count: 10
    });
    currentPlan.value = response.data.data[0]
    if (currentPlan.value.answeredQuestions.length) {
      setAnsweredQuestion()
    }
    if (!currentPlan.value.nextUnansweredQuestion) {
      justSendMessage('مرسی که وقت گذاشتی! با بازگشت به داشبورد، تمرینت رو انتخاب کن و شروع کن', false)
      isRenderingEnterAppButton.value = true
    }
  } catch (e) {
    console.log(e)
  } finally {
    setCurrentQuestion()
    isRenderingButtons.value = true;
    nextTick(() => {
      scrollToBottom();
    });
    useSpinner().hideSpinner()
  }
}

function setAnsweredQuestion() {
  // justSendMessage("از آشناییتون خوشوقتم!", true)
  // justSendMessage("بیا شروع کنیم", false)
  // justSendMessage("من چند تا سوال ازت می‌پرسم، لطفا با دقت جواب بده.", false)
  // justSendMessage("باشه حتما", true)
  isRenderingButtons.value = true;
  currentPlan.value.answeredQuestions.forEach((question) => {
    justSendMessage(question.questionText, false)
    justSendMessage(question.selectedOptionText, true)
  })
  setCurrentQuestion()
}

getUserPlanStatus()
</script>

<template>
  <div class="w-full h-full max-h-screen custom-pattern-bg-image">
    <div class="w-full flex items-center py-4 px-5 gap-x-3">
      <button type="button" class="w-8 h-8 flex items-center justify-center rounded-full" @click="router.back(-1)">
        <Icon name="icon:arrow-right" size="15" class="[&_*]:stroke-white" />
      </button>
      <div class="w-11 h-11 rounded-full flex items-center justify-center bg-white">
        <Icon name="icon:logo-typography-vertical" color="#01CED1" />
      </div>
      <div class="flex flex-col">
        <p class="text-white">پادی لایف</p>
        <p class="text-sm text-white">آنلاین</p>
      </div>
    </div>
    <div class="w-full h-[calc(100svh-76px)] bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-between">
      <div class="px-5 py-4 mt-5 overflow-y-auto">
        <div class="w-full flex flex-col gap-3">
          <div v-if="messageList.length" v-html="messageList[messageList.length - 1].text"></div>
          <template v-if="isRenderingButtons && !isRenderingEnterAppButton">
            <div v-for="option in currentQuestion?.questionOptions" :key="option.id"
              :class="selectedItem?.id === option.id ? '!bg-primary text-white' : ''"
              class="bg-white rounded-[28px] w-full shadow-[0px_1px_2px_0px_rgba(0,0,0,0.25),0px_2px_6px_3px_rgba(0,0,0,0.12)] p-4"
              @click="selectedItem = option">
              <div style="overflow-wrap: anywhere" v-html="option.text"></div>
            </div>
          </template>
        </div>
        <!--        <LazySharedQuestionBubble-->
        <!--            v-for="(message, index) in messageList"-->
        <!--            :key="index"-->
        <!--            ref="messageRef"-->
        <!--            :message="message"-->
        <!--        ></LazySharedQuestionBubble>-->
      </div>
      <div v-if="!isRenderingButtons && !isRenderingEnterAppButton" class="px-5 pb-4">
        <UtilsInputsBaseInput v-model="chatMessage" readonly name="chatMessage" placeholder="بنویسید"
          label-class="bg-white rounded-[28px]">
          <template #icon>
            <button :disabled="chatMessage.length ? false : true" type="button" @click="sendMessage(true)">
              <Icon name="icon:send" />
            </button>
          </template>
        </UtilsInputsBaseInput>
      </div>
      <div v-else-if="isRenderingButtons && !isRenderingEnterAppButton"
        class="bg-[#F7F8FE] rounded-t-lg p-4 space-y-2 shadow-[0px_-1px_4px_0px_rgba(0,0,0,0.25)]">
        <!--        <div-->
        <!--            v-for="option in currentQuestion?.questionOptions" :key="option.id"-->
        <!--          class="bg-white rounded-[28px] w-full shadow-[0px_1px_2px_0px_rgba(0,0,0,0.25),0px_2px_6px_3px_rgba(0,0,0,0.12)] p-4"-->
        <!--          :class="selectedItem?.id === option.id? '!bg-primary text-white' : ''"-->
        <!--          @click="selectedItem = option"-->
        <!--        >-->
        <!--          <div style="overflow-wrap: anywhere"  v-html="option.text"></div>-->
        <!--        </div>-->
        <button type="button" class="btn btn-primary !rounded-[28px] w-full p-4" @click="createUserAnswer">
          ارسال
        </button>
      </div>
      <div v-else class="px-5 pb-4">
        <nuxt-link  to="/dashboard" class="btn btn-primary !rounded-[28px] w-full">
          بازگشت
        </nuxt-link>
      </div>
    </div>
    <LazyUtilsDialogsChoosePartnerDialog :userPlanId="route.params.userPlanId" v-model="isRenderingChoosePartnerDialog">
    </LazyUtilsDialogsChoosePartnerDialog>
  </div>
</template>
