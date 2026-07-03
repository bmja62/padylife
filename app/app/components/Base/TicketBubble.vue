<script setup lang="ts">

import type {ITicketDetail} from "~/services/TicketService";

interface IProps {
  ticket: ITicketDetail
}

const props: IProps = defineProps({
  ticket: {
    type: Object as PropType<ITicketDetail>
  }
})
const authStore = useAuthStore()
const isMe = computed(() => {
  return props.ticket.responseType === 'UserResponse'
})
</script>

<template>
  <div v-if="ticket" class="chat chat-end" :class="{'!chat-start':isMe}">
    <div class="chat-image avatar">
      <div class="w-10 rounded-full">
        <img
            alt=""
            :src="isMe ? authStore?.getUser?.profileImage ? authStore?.getUser?.profileImage : '/common/no-image.png'  : '/logo.png'"
        />
      </div>
    </div>

    <div class="chat-bubble !rounded-b-xl"
         :class="{'bg-[#01ced1] text-white':isMe,'bg-[#ffff] text-[#212121]':!isMe}">{{ ticket.content }}
    </div>
    <div class="chat-footer opacity-50"> {{
        new Date(ticket.createdAt).toLocaleDateString('fa-IR', {
          hour: 'numeric',
          minute: 'numeric'
        })
      }}
    </div>
  </div>

</template>

<style scoped>

</style>