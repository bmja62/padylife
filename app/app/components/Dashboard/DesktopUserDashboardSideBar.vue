<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";

interface IProps {
  userRole: UserRole;
}

const props = defineProps<IProps>();
const authStore = useAuthStore()
const router = useRouter()

interface IMenu {
  title: string;
  link: string;
  icon?: string;
  external?: boolean;
}

interface IMenuItems {
  user: IMenu[];
  specialist: IMenu[];
}

const { $api } = useNuxtApp<IApiProvider>()

const menuItems: IMenuItems = {
  user: [
    { title: "داشبورد", link: "/dashboard", icon: "icon:work" },
    { title: "برنامه‌های من", link: "/dashboard/user/my-plans", icon: "icon:golf" },
    { title: "برنامه‌ها", link: "/dashboard/user/all-plans", icon: "icon:course" },
    { title: "سفارشات من", link: "/dashboard/user/my-orders", icon: "icon:page" },
    { title: "امتیازات من", link: "/dashboard/user/points", icon: "icon:diamond" },
    { title: "آنالیز", link: "/dashboard/analyze", icon: "icon:heart-outline-new" },
    { title: "افزودن برنامه", link: "/dashboard/plans/add", icon: "icon:plus" },
    { title: "قوانین و مقررات", link: "/privacy-policies", icon: "icon:danger-v1" },
    { title: "تنظیمات", link: "/dashboard/user/settings", icon: "icon:setting" },
  ],
  specialist: [
    { title: "برنامه‌ها", link: "https://admin.padylife.com/", icon: "icon:golf", external: true },
    { title: "گزارش دوره‌ها", link: "/dashboard/plans/plans-reports", icon: "icon:file-copy-2-line" },
    { title: "امتیازات من", link: "#", icon: "icon:diamond" },
    { title: "قوانین و مقررات", link: "/privacy-policies", icon: "icon:danger-v1" },
    { title: "تنظیمات", link: "/dashboard/specialist/my-information", icon: "icon:setting" },
  ],
};

const isUserSidebar = computed(() => props.userRole === "user");

async function logout() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.users.logout()
    if (response.isSuccess) {
      authStore.logout()
      router.push('/')
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
  <aside class="w-[300px] h-screen bg-white flex flex-col overflow-hidden border-l border-gray-300">

    <div class="w-full flex bg-primary flex-row justify-start items-center py-6 px-4 gap-2">
      <NuxtLink
        :to="isUserSidebar ? '/dashboard/user/settings/user-information' : '/dashboard/specialist/my-information'">
        <div class="h-[60px] w-[60px] rounded-full flex justify-center items-center relative">
          <Icon name="icon:user-edit" size="20" class="absolute [&_*]:stroke-white left-0 bottom-0" />
          <NuxtImg v-if="authStore?.getUser?.profileImage" :src="authStore.getUser.profileImage"
            class="w-full h-full object-cover rounded-full" />
          <NuxtImg v-else src="/common/no-image.png" class="w-full h-full object-cover rounded-full" />
        </div>
      </NuxtLink>

      <div class="flex flex-col gap-2">
        <span class="text-white">{{ authStore?.getUser?.fullName }}</span>
        <span class="text-white text-sm">
          {{ authStore?.getUserPoints?.availablePoints }} امتیاز
        </span>
      </div>
    </div>

    <div class="w-full flex-1 flex flex-col overflow-y-auto">
      <template v-for="(item, index) in menuItems[props.userRole]" :key="index">

        <NuxtLink v-if="!item.external" :to="item.link"
          class="w-full border-b border-gray-300 flex items-center p-4 gap-x-4">
          <Icon v-if="item.icon" :name="item.icon" :color="isUserSidebar ? '#01CED1' : '#00ABFB'"
            :class="isUserSidebar ? '[&_*]:stroke-[#01CED1]' : '[&_*]:stroke-[#00ABFB]'" size="22" />
          <span>{{ item.title }}</span>
        </NuxtLink>

        <a v-else :href="item.link" target="_blank"
          class="w-full border-b border-gray-300 flex items-center p-4 gap-x-4">
          <Icon v-if="item.icon" :name="item.icon" :color="isUserSidebar ? '#01CED1' : '#00ABFB'"
            :class="isUserSidebar ? '[&_*]:stroke-[#01CED1]' : '[&_*]:stroke-[#00ABFB]'" size="22" />
          <span>{{ item.title }}</span>
        </a>

      </template>

      <div @click="logout" class="w-full border-t cursor-pointer border-gray-300 flex p-4">
        <span>خروج از حساب کاربری</span>
      </div>
    </div>

  </aside>
</template>
