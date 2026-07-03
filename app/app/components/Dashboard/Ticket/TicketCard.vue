<script setup lang="ts">

import {type ITicketItem, TicketStatus, TicketTypesPersian} from "~/services/TicketService";

interface IProps {
  ticket: ITicketItem
}

const props: IProps = defineProps({
  ticket: {
    type: Object as PropType<ITicketItem>
  }
})
</script>

<template>
  <nuxt-link :to="`/dashboard/support/detail/${ticket?.id}`" class="w-full flex bg-white p-3 rounded-xl shadow justify-between">
    <div class="flex items-center gap-2">
      <img src="/logo.png" class="w-10 h-10 rounded-full object-contain" alt="">
      <div class="flex flex-col ">
        <strong class="line-clamp-1">{{ TicketTypesPersian[ticket.ticketType] }}</strong>
        <small class="line-clamp-1 text-gray-400">{{ ticket.title }}</small>
      </div>
    </div>
    <div class="flex flex-col justify-center items-center ">
      <small class="line-clamp-1">{{
          new Date(ticket.createdAt).toLocaleTimeString('fa-IR', {
            hour: 'numeric',
            minute: 'numeric'
          })
        }}</small>
      <div v-if="ticket?.status === TicketStatus.WaitingForUser"
           class=" w-5 h-5 flex items-center justify-center bg-primary text-white  rounded-full">
        1
      </div>
    </div>
  </nuxt-link>

</template>

<style scoped>

</style>