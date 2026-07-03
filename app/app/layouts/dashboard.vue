<script setup lang="ts">
import type { IUserDataEvent, IUserNotificationEvent } from "~/services/NotificationService";

const { $notifyHub } = useNuxtApp()
const hubConnection = ref(null)
const authStore = useAuthStore()
onMounted(() => {
  hubConnection.value = $notifyHub.getInstance()

  hubConnection.value.on('UserNotifications', (event: IUserNotificationEvent) => {
    useAlerts().notification(event.subject, event.description)
    // let newCount = authStore.getNotificationCount
    // newCount++
    // authStore.setNewNotificationCount(newCount)
  })
  hubConnection.value.on('UserData', (event: IUserDataEvent) => {
    authStore.setNewNotificationCount(event.unReadNotificationCount)
  })
})


interface IProps {
  userRole?: UserRole;
}


const props = withDefaults(defineProps<IProps>(), {
  userRole: "user",
});
</script>
<template>
  <div id="mainContainer" class="relative   h-full">
    <div class="flex flex-row min-h-screen">
      <aside class="hidden sm:block sm:w-[300px] ">
        <LazyDashboardDesktopUserDashboardSideBar :user-role="props.userRole">
        </LazyDashboardDesktopUserDashboardSideBar>
      </aside>
      <main class="sm:min-h-[100lvh] min-h-[92lvh] max-h-[92svh] w-full mx-auto bg-[#F7F8FE] grid grid-cols-1 overflow-y-auto">
        <slot />
      </main>
    </div>
    <div class="relative">
      <BaseBottomNav class="fixed w-full bottom-0 sm:hidden mx-auto"></BaseBottomNav>
    </div>
  </div>
</template>
