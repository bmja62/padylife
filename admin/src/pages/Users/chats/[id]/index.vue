<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {useAuthStore} from '@/stores/auth'
import {IGetChatParams, IMessage} from "@/services/ChatService";

// Variables
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()
const auth = useAuthStore()
const messageCard = ref(null)
const totalCount = ref(1)
const $api = inject<IApiProvider>('$api')
const messagesList = ref<IMessage []>([])
const messagesListFilters = ref<IGetChatParams>({
  roomId: route.params.id,
  pageNumber: 1,
  count: 10
})
// LifeCycles
onMounted(async () => {
  await getChatDetail()
})

// Functions

function goToLastMessage() {
  nextTick(() => {
    if (messageCard?.value?.length) {
      messageCard.value[messageCard.value.length - 1].$el.scrollIntoView({
        behavior: 'smooth',
        block: 'center',
        inline: 'center'
      })
    }
  })
}

async function getChatDetail(prepend) {
  try {
    spinner.showSpinner()

    const response = await $api?.chat.getUserChatById(messagesListFilters.value)
    if (response.data.data.data.length) {
      response.data.data.data.forEach((item) => {
        if (prepend) {
          messagesList.value.unshift(item)
        } else {
          messagesList.value.push(item)
          goToLastMessage()
        }
      })
    }
    totalCount.value = response.data.data.totalCount

  } catch (error: unknown) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function changePage() {
  messagesListFilters.value.pageNumber++
  getChatDetail(true)
}
</script>

<template>
  <VCard v-if="messagesList">
    <VCardText class="px-4">
      <VDivider class="my-6"/>
      <VRow class="px-4">
        <VCol
          cols="12"
          class="max-ticket-height"
        >
          <VCol v-if="totalCount> messagesList.length" cols="12" class="d-flex align-content-center justify-center">
            <VBtn color="primary" @click="changePage">بارگذاری بیشتر</VBtn>
          </VCol>
          <VRow
            v-for="(detail,idx) in messagesList"
            :key="detail.id"
            ref="messageCard"
            :justify="idx%2==0 ? 'start' : 'end'"
          >
            <VCol cols="12" md="6">
              <VCard
                variant="flat"
                :color="
                  idx%2==0 ? 'success' : 'warning'
                "
              >
                <VCardText>
                  <p
                    dir="auto"
                    :class="
                      idx%2==0
                        ? 'text-right'
                        : 'text-left'
                    "
                  >
                    {{ detail.encryptedContent }}
                  </p>
                </VCardText>
              </VCard>

            </VCol>
          </VRow>
        </VCol>
      </VRow>
    </VCardText>
    <!--    <CloseTicketConfirmationDialog-->
    <!--      v-model:dialogState="isRenderingTicketCloseConfirmationModal"-->
    <!--      @refetch="getChatDetail"-->
    <!--    />-->
  </VCard>
</template>

<style>
.v-field__append-inner {
  transform: rotate(180deg) !important;
}

.max-ticket-height {
  max-block-size: 60vh;
  overflow-y: auto !important;
}
</style>
