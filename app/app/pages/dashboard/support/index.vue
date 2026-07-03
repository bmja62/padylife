<script setup lang="ts">

import type {ITicketItem} from "~/services/TicketService";
import EmptyView from "~/components/utils/EmptyView.vue";

definePageMeta({
  layout: "dashboard",
  auth: true,
})
useHead({
  title: 'پشتیبانی'
})
const ticketsList = ref<ITicketItem[]>([])
const totalCount = ref(1)
const {$api} = useNuxtApp()
const ticketFilters = ref<IGlobalGridRequest>({
  pageNumber: 1,
  count: 10,
  search: ''
})

async function getMyTickets() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.ticket.getMyTickets(ticketFilters.value as IGlobalGridRequest)
    ticketsList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error);
  } finally {
    useSpinner().hideSpinner()
  }
}

function changePage(page) {
  ticketFilters.value.pageNumber = page
  getMyTickets()
}

await getMyTickets()
</script>

<template>
  <div class="w-full py-5  flex flex-col relative ">
    <div  v-if="ticketsList.length" class="w-11/12 mx-auto flex flex-col gap-4 flex-grow"> <!-- Added flex-grow -->
        <LazyDashboardTicketCard
            v-for="ticket in ticketsList" :key="ticket.id"
            :ticket="ticket"></LazyDashboardTicketCard>

      <div class="w-full flex items-center justify-center">
        <UtilsCustomPagination
            :page-number="ticketFilters.pageNumber"
            :count="ticketFilters.count"
            :total-count="totalCount"
            @change-page="changePage"
        />
      </div>
    </div>

    <div class="w-11/12 mx-auto sticky mb-5"> <!-- Changed bottom-0 to bottom-5 for spacing -->
      <nuxt-link to="/dashboard/support/create" class="btn w-full bg-primary text-white border-none">
        ایجاد یک تیکت جدید
      </nuxt-link>
    </div>
  </div>
</template>

<style scoped>

</style>