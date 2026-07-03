<script setup lang="ts">
import {ChatMessageStatus, type IChatInfo, type IChatMessage, type IMessageDTO} from "~~/types/chat";

const {$chatHub} = useNuxtApp()
const hubConnection = ref(null)
const messageDTO = ref<IMessageDTO>({
  roomId: null,
  encryptedContent: '',
  replyToMessageId: null,
  type: 1,
  encryptionMetadata: null
})
const selectedRoom: IChatInfo = defineModel()
const conversationFilters = ref({
  pageNumber: 1,
  count: 40
})
const emits = defineEmits<{
  (e:'closeSelectedConversation'):void
}>()
const chatBubblesRef = useTemplateRef('chatBubblesRef')
const conversation = ref<IChatMessage[]>([])
onMounted(async () => {
  try {
    hubConnection.value = await $chatHub.getInstance()
    await loadConversation()
    await listenerForReceiveMessage()
    await listenerForNewMessage()
    await listenerForMessageStatus()
  } catch (error) {
    console.error(error)
  } finally {
    await scrollToBottom()
  }
})


const isOnline = computed(() => {
  return selectedRoom.value.userInfo.isOnline
})

function scrollToBottom() {
  setTimeout(() => {
    chatBubblesRef.value[conversation.value.length - 1].$el.scrollIntoView({
      behavior: 'smooth',
      block: "start",
      inline: "start"
    })
  }, 100)
}

async function loadConversation() {
  conversation.value = await hubConnection.value.invoke("LoadMessagesAsync", selectedRoom.value.chat.chatId, conversationFilters.value.pageNumber, conversationFilters.value.count);
}

async function listenerForReceiveMessage() {
  hubConnection.value.on('ReceiveMessage', async (message: IChatMessage) => {
    await updateMessageStatus(message.messageId, ChatMessageStatus.Read)
    conversation.value.push(message)
    scrollToBottom()

  })
}

async function updateMessageStatus(messageId, status: ChatMessageStatus) {
  await hubConnection.value.invoke("UpdateMessageStatusAsync", messageId, status)

}

async function listenerForNewMessage() {
  hubConnection.value.on('NewMessage', (message: IChatMessage) => {
    conversation.value.push(message)
    scrollToBottom()
  })
}

async function listenerForMessageStatus() {
  await hubConnection.value.on('MessageStatusUpdated', (message: IChatMessage) => {
    const idx = conversation.value.findIndex(e => e.messageId === message.messageId)
    conversation.value[idx].status = message.status
  })
}

async function sendMessage() {
  if (messageDTO.value.encryptedContent) {
    messageDTO.value.roomId = selectedRoom.value.chat.chatId
    await hubConnection.value.invoke("SendMessageAsync", messageDTO.value)
    // conversation.value.push(messageDTO.value)
    messageDTO.value.encryptedContent = ''
  }
}

function closeConversation(){
  emits('closeSelectedConversation')
}
</script>

<template>
  <div class="w-full flex flex-col justify-start items-start">
    <div class="w-full p-3 flex border-b mb-3 items-center justify-between">
      <div class="relative ">
      <strong class="text-gray-800 text-base" style="overflow-wrap: anywhere">{{
          selectedRoom.userInfo.userFullName
        }}</strong>
        <div :class="{'!badge-success':isOnline}" class="badge absolute   badge-xs badge-primary tooltip"
           :data-tip="isOnline ? 'آنلاین' : 'آفلاین'"></div>
    </div>
      <div @click="closeConversation">
      <LazyIconsChevronLeftIcon class="w-4 h-4 sm:hidden"></LazyIconsChevronLeftIcon>
      </div>
    </div>
  <div class="w-full pr-1 flex flex-col gap-2" style="overflow-wrap: anywhere">
    <LazyChatBubble ref="chatBubblesRef" v-for="message in conversation" :key="message.messageId"
                    :message="message"></LazyChatBubble>
  </div>
  <div class="w-full sticky bottom-0 z-[1] right-0 px-1">
    <input
        v-model="messageDTO.encryptedContent"
        class="w-full bg-[#ECEFFF] rounded-2xl border border-[#E0E4E8] text-xs text-gray-700 pl-8 pr-2 py-2"
        placeholder="پیام خود را بنویسید"
    />
    <Icon
        @click="sendMessage"
        name="icon:send-v2-new"
        class="absolute left-2 top-2"
        size="20"
        color="#01CED1"
    />
  </div>
  </div>

</template>

<style scoped>

</style>