<script setup lang="ts">
import {ChatMessageStatus, type IChatMessage} from "~~/types/chat";

interface IProps {
  message: IChatMessage
}

const props: IProps = defineProps({
  message: {
    type: Object as PropType<IChatMessage>
  }
})

const authStore = useAuthStore()
const isMe = computed(() => {
  return props.message.senderId === authStore.getUser.id
})
</script>

<template>
  <div :class="{'chat-start w-full':isMe,'chat-end w-full':!isMe}"
       class="chat  flex !flex-col">

    <div :class="{'bg-[#01ced1] text-white':isMe,'bg-[#ffff] text-[#212121]':!isMe}"
         class="chat-bubble">{{ props.message.encryptedContent }}
    </div>
    <div class="flex items-center gap-1">

      <div class="w-full flex items-center gap-1 mt-1"
           :class="{'justify-start':isMe,'justify-end':!isMe}">
        <small class="text-gray-400 text-[10px]">
          {{ new Date(props.message.createdAt).toLocaleTimeString('fa-IR', {hour: 'numeric', minute: 'numeric'}) }} -
          {{ new Date(props.message.createdAt).toLocaleDateString('fa-IR') }}
        </small>
      </div>
      <div v-if="isMe" class="flex items-center relative">
        <Icon v-if=" props.message.status === ChatMessageStatus.Sent" name="icon:check-chat" size="10"
              class="[&_*]:fill-primary"></Icon>
        <Icon v-if=" props.message.status === ChatMessageStatus.Read" name="icon:double-check-chat" size="10"
              class=" [&_*]:fill-primary absolute right-1"></Icon>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>