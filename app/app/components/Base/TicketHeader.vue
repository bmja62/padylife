<script setup lang="ts">
import {type ITicketItem, TicketTypesPersian} from "~/services/TicketService";

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
  <div class="w-full bg-white z-50 py-2 px-4 sticky top-0 flex items-center gap-3 shadow border-b border-gray-300">
    <nuxt-link to="/dashboard/support">
      <LazyIconsChevronLeftIcon class="rotate-180 w-4 h-4 fill-gray-400"></LazyIconsChevronLeftIcon>
    </nuxt-link>
    <img src="/logo.png" class="w-10 h-10 rounded-full object-contain" alt="">
    <div class="flex flex-col ">
      <strong class="line-clamp-1">پشتیبانی پادی لایف</strong>
      <small class="line-clamp-1 text-gray-400">{{ TicketTypesPersian[ticket.ticketType] }}</small>
    </div>

  </div>
</template>

<style scoped>

</style>