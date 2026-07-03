<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {TicketStatusesColor, TicketStatusesPersian} from '@/models/Enums/TicketTypes'
import type {ITicket,} from '@/services/TicketService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {useAuthStore} from '@/stores/auth'

// Variables
const spinner = useSpinner()
const alert = useAlerts()
const route = useRoute()
const auth = useAuthStore()
const $api = inject<IApiProvider>('$api')
const isRenderingTicketCloseConfirmationModal = ref<boolean>(false)
const ticketDetails = ref<ITicket | null>(null)
const ticketAnswerPayload = ref({
  id: null,
  content: '',
})

// LifeCycles
onMounted(async () => {
  await getTicketDetails()
})

// Functions
function isMyResponse(type): boolean {
  return type !== 'UserResponse'
}

async function getTicketDetails() {
  try {
    spinner.showSpinner()

    const response = await $api?.tickets.getTicketDetails(route.params.id as string)
    ticketDetails.value = response.data.data
  } catch (error: unknown) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

async function sendTicketAnswer() {
  try {
    spinner.showSpinner()
    ticketAnswerPayload.value.id = ticketDetails.value.id
    const response = await $api?.tickets.sendTicketAnswer(ticketAnswerPayload.value)
    ticketAnswerPayload.value.content = ''
    getTicketDetails()
  } catch (error: unknown) {
    if (isAxiosError(error))

      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}

function renderCloseTicketConfirmation() {
  isRenderingTicketCloseConfirmationModal.value = true
}
</script>

<template>
  <VCard v-if="ticketDetails">
    <VCardText class="px-4">
      <VRow
        justify="space-between"
        align-content="center"
      >
        <VCol
          cols="auto"
          align-self="center"
        >
          <div class="d-flex align-items-center">
            <h2 class="text-h5 ml-2">
              {{ ticketDetails.title }}
            </h2>
          </div>
        </VCol>
        <VCol
          cols="auto"
          align-self="center"
        >
          <VChip
            v-if="ticketDetails.status"
            tonal
            :color="TicketStatusesColor[ticketDetails.status]"
          >
            {{ TicketStatusesPersian[ticketDetails.status] }}
          </VChip>

          <VBtn v-if="ticketDetails.status !== 'Closed'" density="compact"
                class="mx-1"
                @click="renderCloseTicketConfirmation"
                color="error"
          >بستن تیکت
          </VBtn>



        </VCol>

      </VRow>
      <VRow
        justify="space-between"
        align-content="center"
      >
        <VCol cols="auto">
          <VChip
            color="success"
            label
          >
            تاریخ ایجاد:
            {{ new Date(ticketDetails.createdAt).toLocaleDateString("fa-IR") }}
          </VChip>
        </VCol>
      </VRow>
      <VDivider class="my-6"/>
      <VRow class="px-4">
        <VCol
          cols="12"
          class="max-ticket-height"
        >
          <VRow
            v-for="detail in ticketDetails.ticketDetails"
            :key="detail.id"
            :justify="isMyResponse(detail.responseType) ? 'start' : 'end'"
          >
            <VCol cols="12" md="6">
              <VCard
                variant="flat"
                :color="
                  isMyResponse(detail.responseType) ? 'success' : 'warning'
                "
              >
                <VCardText>
                  <p
                    dir="auto"
                    :class="
                      isMyResponse(detail.responseType)
                        ? 'text-right'
                        : 'text-left'
                    "
                  >
                    {{ detail.content }}
                  </p>
                </VCardText>
              </VCard>
              <p
                class="text-medium-emphasis text-subtitle-2 mt-1"
                dir="auto"
                :class="
                  isMyResponse(detail.responseType) ? 'text-right' : 'text-left'
                "
              >
                {{ new Date(detail.createdAt).toLocaleDateString("fa-IR") }} -
                {{
                  new Date(detail.createdAt).toLocaleTimeString("fa-IR", {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                }}
              </p>
            </VCol>
          </VRow>
        </VCol>
      </VRow>
    </VCardText>
    <VRow>
      <VCol cols="12">
        <VCard color="grey-lighten-4">
          <VCardText>
            <VTextField
              v-model="ticketAnswerPayload.content"
              rounded="lg"
              append-inner-icon="mdi-send"
              label="پاسخ را وارد کنید"
              variant="solo"
              hide-details
              :disabled="ticketDetails.status === 'Closed'"
              single-line
              @keydown.enter="sendTicketAnswer"
              @click:append-inner="sendTicketAnswer"
            />
          </VCardText>
        </VCard>
      </VCol>
    </VRow>
    <CloseTicketConfirmationDialog
      v-model:dialogState="isRenderingTicketCloseConfirmationModal"
      @refetch="getTicketDetails"
    />
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
