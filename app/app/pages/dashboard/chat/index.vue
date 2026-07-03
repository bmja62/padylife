<script setup lang="ts">
import {ChatFilters, type IChatInfo, type IGenericConversationsList} from "~~/types/chat";

definePageMeta({
  layout: "chat-layout",
  auth: true,
});
useHead({
  title: 'مکالمات'
})
const {$chatHub} = useNuxtApp()
const hubConnection = ref(null)
const route = useRoute()
const conversationsList = ref<IGenericConversationsList<IChatInfo[]>>([])
const selectedFilter = ref<ChatFilters>(1)
const selectedRoom = ref<IChatInfo>(null)
const conversationsFilters = ref({
  pageNumber: 1,
  count: 10,
  search: ''
})
// if (route.query.roomId) {
//   const userWantToTalk = conversationsList.value.filter((conversation: INewConversation) => {
//     return conversation.chatId === +route.query.roomId
//   })
//   await changeConversation(userWantToTalk[0], +route.query.roomId)
// }
// }
// catch
// (e)
// {
//   console.error(e)
// }
// })
onBeforeUnmount(async () => {
  await $chatHub.closeInstance()
})
onMounted(async () => {
  try {
    hubConnection.value = await $chatHub.getInstance()
    // TODO think of something for this
    setTimeout(async () => {
      await loadConversations()
      await listenerForRecieveMessage()
    }, 500)
  } catch (error) {
    console.error(error)
  }
})

async function loadConversations() {
  conversationsList.value = await hubConnection.value.invoke('LoadUserChatsAsync', conversationsFilters.value.pageNumber, conversationsFilters.value.count, conversationsFilters.value.search)
}

async function listenerForRecieveMessage() {
  hubConnection.value.on('ReceiveMessage', async () => {
    // await loadConversations()
  })
}

async function setSelectedRoom(conversation: IChatInfo) {
  if (selectedRoom.value) {
    await leaveRoom()
  }
  selectedRoom.value = conversation
  hubConnection.value.invoke("JoinRoom", selectedRoom.value.chat.chatId);
}

async function leaveRoom() {
  hubConnection.value.invoke("LeaveRoom", selectedRoom.value.chat.chatId);

}

const filteredConversations = computed(() => {
  if (conversationsList?.value?.data?.length) {
    selectedRoom.value = null
    if (selectedFilter.value === ChatFilters.Companion) {
      return conversationsList.value.data.filter(e => !e.chat.isExpert)
    } else if (selectedFilter.value === ChatFilters.Specialist) {
      return conversationsList.value.data.filter(e => e.chat.isExpert)
    }

  }
})


function closeSelectedConversation() {
  selectedRoom.value = null
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <DashboardChatHeader/>
    <div
        class="w-full min-h-[calc(100vh-10.1vh)] max-h-[calc(100vh-10.1vh)] h-full bg-[#F7F8FE] px-5  pt-6 rounded-t-[32px] flex flex-col"
    >
      <div class="h-auto">
        <div v-if="!selectedRoom" class="w-full">
          <LazyChatFilters v-model="selectedFilter"></LazyChatFilters>
          <template v-if="conversationsList?.data?.length">

            <LazyChatConversationCard v-for="conversation in filteredConversations" :key="conversation.chat.chatId"
                                      @click="setSelectedRoom(conversation)"
                                      v-model="selectedRoom"
                                      :conversation="conversation"></LazyChatConversationCard>
          </template>
        </div>

        <div
            v-if="selectedRoom"

            class="w-full bg-white max-h-[calc(100vh-15vh)] min-h-[calc(100vh-15vh)] overflow-y-scroll overflow-x-hidden border border-[#E0E4E8] rounded-2xl relative p-2"
        >
          <LazyChatBody @closeSelectedConversation="closeSelectedConversation" v-if="selectedRoom" :key="selectedRoom"
                        v-model="selectedRoom"></LazyChatBody>
          <div v-else>
            <span class="text-gray-400">یک مکالمه را انتخاب کنید.</span>
          </div>
        </div>

      </div>

    </div>
  </div>
</template>

<style scoped></style>
