<script setup lang="ts">
import {TicketType} from "~/services/TicketService";

const router = useRouter()
definePageMeta({
  auth: false,
  hasHeader: false,
  hasFooter: false
})
useHead({
  title: 'پشتیبانی'
})
const {$api} = useNuxtApp()
const createTicketPayload = ref({
  title: "",
  content: "",
  ticketType: ""
})
const staticTicketQuestions = ref([
  {
    id: 1,
    content: 'نحوه پرداخت به چه صورت است؟',
    type: TicketType.Financial
  },
  {
    id: 2,
    content: 'قصد دارم به صورت مستقیم به کارشناس متصل شوم و سوالم را مطرح کنم',
    type: TicketType.Expert
  },
  {
    id: 3,
    content: 'انتقاد و یا پیشنهادی دارم',
    type: TicketType.Suggestion
  }
])

async function setPayloadFromStatics(item) {
  createTicketPayload.value.title = item.content
  createTicketPayload.value.content = item.content
  createTicketPayload.value.ticketType = item.type
  await createTicket()
}

async function setCustomTicketPayload() {
  createTicketPayload.value.title = createTicketPayload.value.content
  createTicketPayload.value.ticketType = TicketType.Other
  await createTicket()
}


async function createTicket() {
  if (createTicketPayload.value.content)
    try {
      useSpinner().renderSpinner()
      const response = await $api.ticket.create(createTicketPayload.value)
      if (response.isSuccess) {
        router.push('/dashboard/support')
      } else {
        useAlerts().error(response.message)
      }
    } catch (error) {
      console.error(error);
    } finally {
      useSpinner().hideSpinner()
    }
}
</script>

<template>
  <div class="w-full  min-h-screen relative ">
    <div class="w-full bg-white z-50 py-2 px-4 sticky top-0 flex items-center gap-3 shadow border-b border-gray-300">
      <nuxt-link to="/dashboard/support">
        <LazyIconsChevronLeftIcon class="rotate-180 w-4 h-4 fill-gray-400"></LazyIconsChevronLeftIcon>
      </nuxt-link>
      <img src="/logo.png" class="w-10 h-10  rounded-full object-contain" alt="">
      <div class="flex flex-col ">
        <strong class="line-clamp-1">پشتیبانی پادی لایف</strong>
      </div>
    </div>
    <div class="w-11/12 mx-auto min-h-screen flex flex-col py-3 gap-2">
      <div class="chat chat-end">
        <div class="chat-image avatar">
          <div class="w-10 rounded-full">
            <img
                alt=""
                class="!object-contain"
                src="/logo.png"
            />
          </div>
        </div>

        <div class="chat-bubble bg-primary  flex flex-col gap-2">
          <strong class="text-white">
            چطور می‌تونم کمکتون کنم؟
          </strong>
          <div class="flex flex-col gap-2">
            <div v-for="item in staticTicketQuestions" @click="setPayloadFromStatics(item)"
                 class="bg-white text-black p-2 rounded-xl">
              <small class="font-bold">{{ item.content }}</small>
            </div>
          </div>
        </div>
        <div class="chat-footer opacity-50"> {{

            new Date(Date.now()).toLocaleDateString('fa-IR', {
              hour: 'numeric',
              minute: 'numeric'
            })
          }}
        </div>
      </div>

    </div>
    <div class=" w-full z-50 sticky bottom-2 px-2">
      <div class="w-full bg-white flex items-center rounded-full gap-1 px-4 border border-primary">
        <input type="text"
               v-model.trim="createTicketPayload.content"
               class="w-11/12 h-full ring-none  outline-none">
        <div class="w-1/12" @click="setCustomTicketPayload">
          <LazyIconsSendIcon class="stroke-primary"></LazyIconsSendIcon>
        </div>
      </div>

    </div>
  </div>

</template>

<style scoped>

</style>