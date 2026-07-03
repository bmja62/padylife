<script setup lang="ts">
import type { IUserDataEvent, IUserNotificationEvent } from "~/services/NotificationService";

const { $notifyHub } = useNuxtApp()
const hubConnection = ref(null)
const authStore = useAuthStore()
const route = useRoute()
onMounted(() => {
  hubConnection.value = $notifyHub.getInstance()

  hubConnection.value.on('UserNotifications', (event: IUserNotificationEvent) => {
    useAlerts().notification(event.subject, event.description)
    // let newCount = authStore.getNotificationCount
    // newCount++
    // authStore.setNewNotificationCount(newCount)
  })
  hubConnection.value.on('UserData', (event: IUserDataEvent) => {
    authStore.setNewNotificationCount(event.unReadNotificationCount)
  })
})

// ----------------
const { $chatHub } = useNuxtApp()


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

// *****
import { ChatFilters } from "~~/types/chat";

</script>
<template>
  <LazyDashboardChatHeader class="hidden sm:flex bg-[#01ced1]"></LazyDashboardChatHeader>
  <div id="mainContainer" class="relative  hidden  h-[calc(100vh-80px)] sm:flex flex-col sm:flex-row ">
    <div class="w-full sm:w-[350px] border-r border-gray-200 bg-white flex flex-col">
      <LazyChatFilters v-model="selectedFilter"></LazyChatFilters>
      <div class="flex-1 overflow-y-auto px-2 [direction:ltr]">
        <div class="[direction:rtl]">
          <template v-if="conversationsList?.data?.length">
            <LazyChatConversationCard v-for="conversation in filteredConversations" :key="conversation.chat.chatId"
              @click="setSelectedRoom(conversation)" v-model="selectedRoom" :conversation="conversation">
            </LazyChatConversationCard>
          </template>
        </div>
      </div>
    </div>

    <div class="flex-1 flex flex-col bg-[#F7F8FE]">
      <div v-if="selectedRoom"
        class="w-full bg-white  overflow-y-scroll overflow-x-hidden border border-[#E0E4E8] rounded-2xl relative p-2">
        <LazyChatBody @closeSelectedConversation="closeSelectedConversation" v-if="selectedRoom" :key="selectedRoom"
          v-model="selectedRoom"></LazyChatBody>
      </div>
      <div v-else class="flex-1 flex flex-col items-center justify-center select-none gap-6">
        <NuxtImg src="/junks/NoChatImage.png" class="w-64 h-64 object-contain opacity-90" />
        <span class="text-gray-500 text-base font-medium">
          لطفاً یک گفت و گو را انتخاب کنید
        </span>
      </div>
    </div>
  </div>
  <main
    class="sm:min-h-[100lvh] min-h-[92lvh] max-h-[92svh] w-full mx-auto bg-[#F7F8FE] grid grid-cols-1 overflow-y-auto sm:hidden">
    <slot />
  </main>
  <div class="relative">
    <BaseBottomNav class="fixed w-full bottom-0 sm:hidden mx-auto"></BaseBottomNav>
  </div>
</template>
