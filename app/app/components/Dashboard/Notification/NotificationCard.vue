<script setup lang="ts">
import type {IUserNotificationEvent} from "~/services/NotificationService";

interface IProps {
  notification: IUserNotificationEvent
  activeTab: number
}

const props: IProps = defineProps({
  notification: {
    type: Object as PropType<IUserNotificationEvent>
  },
  activeTab: {
    type: Number as PropType<number>
  },
})
</script>

<template>
  <div class="w-full bg-white shadow rounded-2xl p-3 flex items-center gap-2">
    <div class="w-1/8">
    <Icon
        :name="props.activeTab === 1 ? 'icon:mail-fill' : 'icon:mail-empty'"
        color="#8C8C8C"
        class="w-5 h-5"
    />
    </div>
    <div class="border-r-2  w-7/8 border-gray-200 pr-3 flex flex-col gap-2">
      <span class="line-clamp-1">{{ props.notification.subject }}</span>
      <div class="flex items-center gap-2">
        <Icon name="icon:black-clock" size="20"/>
        <span>{{ useUtils().time_ago(new Date(props.notification.createdAt)) }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>