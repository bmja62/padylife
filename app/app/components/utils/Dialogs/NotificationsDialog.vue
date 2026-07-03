<script setup lang="ts">
import type {IUserNotificationEvent} from "~/services/NotificationService";

const isRendering = defineModel()

interface IProps {
  selectedNotification: IUserNotificationEvent
}

const props: IProps = defineProps({
  selectedNotification: {
    type: Object as PropType<IUserNotificationEvent>
  }
})
</script>

<template>
  <LazyUtilsDialogsBaseDialog full-height dialog-id="notificationDialog" v-model="isRendering">
    <template #title>
      <span>{{ props.selectedNotification.subject }}</span>
    </template>
    <template #default>

      <bdi style="overflow-wrap: anywhere;" class="whitespace-pre-line" dir="ltr" v-html="props.selectedNotification.description"></bdi>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>