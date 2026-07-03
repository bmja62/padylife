<template>
  <VForm
    class="w-100"
    ref="refVForm"
    @submit.prevent="validateData"
  >
    <VRow>
      <VCol cols="12" md="6">
        <NotificationTypePicker  v-model="notificationPayload.notificationType"></NotificationTypePicker>
      </VCol>

      <VCol cols="12" md="6">
        <VTextField
          v-model="notificationPayload.subject"
          required
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          label="عنوان"
        />
      </VCol>

      <VCol cols="12">
        <FroalaEditor
          v-model="notificationPayload.description"
          editor-placeholder="توضیحات"
        />
      </VCol>

      <VCol cols="6">
        <CustomSwitch
          v-model="notificationPayload.allusers"
          label="آیا برای همه کاربران ارسال شود ؟"
          id="customizer-menu-collapsed"
          :has-tooltip="false"
          class="ms-2"
        />

      </VCol>
      <VCol v-if="!notificationPayload.allusers" cols="12" md="6">
        <UserPicker chips multiple :return-object="false" v-model="notificationPayload.reciverIds"></UserPicker>
      </VCol>

      <VCol cols="12" class="mt-3">
        <div class="w-100 d-flex align-items-center justify-content-end">
          <VBtn color="success" type="submit" > ثبت</VBtn>
        </div>
      </VCol>
    </VRow>
  </VForm>
  <!--    <SendToAllConfirmationDialog @sendNotification="sendNewNotification">-->
  <!--    </SendToAllConfirmationDialog>-->
</template>

<script setup lang="ts">
import {INotificationPayload} from "@/services/NotificationsService";
import {useAuthStore} from "@/stores/auth";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {useSpinner} from "@/composables/spinner";
import {useAlerts} from "@/composables/alert";
import {VForm} from "vuetify/components/VForm";
import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";

const authStore = useAuthStore()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)
const notificationPayload = ref<INotificationPayload>({
  senderId: authStore.getUser.id,
  subject: '',
  description: '',
  allusers: true,
  reciverIds: [],
  notificationType: 0,
  isFromSystem: true
})
async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    sendNotification()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function sendNotification() {
  try {
    spinner.showSpinner()
    if (notificationPayload.value.allusers) {
      notificationPayload.value.reciverIds = null
    }
    const response = await $api?.notification.sendNotification(notificationPayload.value)
    if (response.data.isSuccess) {
      alert.success('اعلان با موفقیت ارسال شد')
      notificationPayload.value = {
        senderId: authStore.getUser.id,
        subject: '',
        description: '',
        allusers: true,
        reciverIds: [],
        notificationType: 0,
        isFromSystem: true
      }
    } else {
      alert.error(response.data.message)
    }
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>
