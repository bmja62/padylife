<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {INotificationsGet, IUserNotificationEvent} from "~/services/NotificationService";

definePageMeta({
  layout: "dashboard",
  auth: true

});
useHead({
  title: 'اعلانات'
})
const authStore = useAuthStore()
const {$api} = useNuxtApp<IApiProvider>()
const notificationsListFilters = ref<IGetnotificationFilters>({
  userId: authStore.getUser.id,
  pageNumber: 1,
  count: 50,
})
const notificationsList = ref<INotificationsGet>(null)
const totalCount = ref(null)
const isRenderingNotificationDialog = ref(false)
const selectedNotification = ref<IUserNotificationEvent>(null)
const activeTab = ref(1)
const tabsMap = {
  1: 'unReadNotification',
  2: 'readNotification',
}
async function getAllNotifications() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.notification.getAllNotificationsForUI(notificationsListFilters.value)
    notificationsList.value = response.data.data.data
    totalCount.value = response.data.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getAllNotifications()

function changePage(page: number) {
  notificationsListFilters.value.pageNumber = page
  getAllNotifications()
}

function openNotificationDialog(notification: IUserNotificationEvent) {
  isRenderingNotificationDialog.value = true
  selectedNotification.value = JSON.parse(JSON.stringify(notification))
  readNotification()
}

async function readNotification() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.notification.markAsRead({
      notificationId: selectedNotification.value.id,
      userId: authStore.getUser.id
    })
    if (response.isSuccess) {
      getAllNotifications()
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

const getCurrentNotifcationList = computed(() => {
  if (notificationsList?.value)
    return notificationsList?.value[tabsMap[activeTab.value]]
})
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>اعلانات</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start  gap-y-4"
      >
        <div role="tablist" class="tabs w-full tabs-bordered">
          <a role="tab" class="tab" :class="{'!border-b-primary':activeTab === 1}" @click="activeTab = 1">خوانده
            نشده</a>
          <a role="tab" class="tab" :class="{'!border-b-primary':activeTab === 2}" @click="activeTab = 2">خوانده شده</a>
        </div>
        <div class="w-full flex flex-col gap-y-4">

          <template v-if="getCurrentNotifcationList?.length">
            <LazyDashboardNotificationCard
                @click="openNotificationDialog(notification)"
                v-for="(notification, index) in getCurrentNotifcationList"
                :key="index"
                :activeTab="activeTab"
                :notification="notification"
            >
            </LazyDashboardNotificationCard>
<!--            <div class="w-full flex items-center justify-center">-->
<!--              <UtilsCustomPagination-->
<!--                  :page-number="notificationsListFilters.pageNumber"-->
<!--                  :count="notificationsListFilters.count"-->
<!--                  :total-count="totalCount"-->
<!--                  @change-page="changePage"-->
<!--              />-->
<!--            </div>-->
          </template>
          <span v-else>  اعلانی یافت نشد</span>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsNotificationsDialog v-model="isRenderingNotificationDialog"
                                         :selected-notification="selectedNotification"></LazyUtilsDialogsNotificationsDialog>
  </div>
</template>

<style scoped></style>
