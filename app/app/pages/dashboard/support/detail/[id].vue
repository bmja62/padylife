<script setup lang="ts">
import {type ITicketItem, type ITicketReplyPayload, TicketStatus} from "~/services/TicketService";

const route = useRoute()
definePageMeta({
  layout: "dashboard",
  auth: true,
})
useHead({
  title: 'پشتیبانی'
})
const ticketDetail = ref<ITicketItem>(null)
const ticketReplyPayload = ref<ITicketReplyPayload>({
  id: null,
  content: ''
})
const {$api} = useNuxtApp()

async function getTicketDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.ticket.getTicketDetail(route.params.id)
    ticketDetail.value = response.data
  } catch (error) {
    console.error(error);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function reply() {
  if (ticketReplyPayload.value.content)
    try {
      useSpinner().renderSpinner()

      ticketReplyPayload.value.id = route.params.id
      const response = await $api.ticket.reply(ticketReplyPayload.value as ITicketReplyPayload)
      if (response.isSuccess) {
        ticketReplyPayload.value = {
          id: null,
          content: ''
        }
        await getTicketDetail()
      } else {
        useAlerts().error(response.message)
      }
    } catch (error) {
      console.error(error);
    } finally {
      useSpinner().hideSpinner()
    }
}

await getTicketDetail()
</script>

<template>
  <div class="w-full  min-h-screen relative ">
    <LazyBaseTicketHeader :ticket="ticketDetail"></LazyBaseTicketHeader>
    <div class="w-11/12 mx-auto min-h-screen flex flex-col py-3 gap-2">
      <template v-if="ticketDetail.ticketDetails.length">
        <ClientOnly>
          <LazyBaseTicketBubble v-for="ticket in ticketDetail.ticketDetails" :ticket="ticket"
                                :key="ticket.id"></LazyBaseTicketBubble>
        </ClientOnly>
      </template>
    </div>
    <div class=" w-full z-50 sticky  bottom-10 px-2
        ">
      <div v-if="ticketDetail.status !== TicketStatus.Closed"
           class="w-full bg-white flex items-center rounded-full gap-1 px-4 border border-primary">
        <input type="text" @keydown.enter="reply" v-model.trim="ticketReplyPayload.content"
               class="w-11/12 h-full ring-none  outline-none">
        <div class="w-1/12" @click="reply">
          <LazyIconsSendIcon class="stroke-primary"></LazyIconsSendIcon>
        </div>
      </div>
      <div v-else
           class="w-full flex items-center rounded-full bg-red-500 text-white flex items-center justify-center p-3">
        بسته شده
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>