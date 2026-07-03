<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";

interface IProps {
  userRole: UserRole;
}

const authStore = useAuthStore()
const props = defineProps<IProps>();
const router = useRouter()
interface IMenu {
  title: string;
  link: string;
  icon?: string;
}

interface IMenuItems {
  user: IMenu[];
  specialist: IMenu[];
}

const { $api } = useNuxtApp<IApiProvider>()

const menuItems: IMenuItems = {
  user: [
    {
      title: "داشبورد",
      link: "/dashboard",
      icon: "icon:work",
    },
    {
      title: "برنامه‌های من",
      link: "/dashboard/user/my-plans",
      icon: "icon:golf",
    },
    {
      title: "برنامه‌ها",
      link: "/dashboard/user/all-plans",
      icon: "icon:course",
    },
    // {
    //   title: "احساسات ثبت شده من",
    //   link: "/dashboard/user/my-feelings",
    //   icon: "icon:silly-face",
    // },
    {
      title: "سفارشات من",
      link: "/dashboard/user/my-orders",
      icon: "icon:page",
    },
    {
      title: "امتیازات من",
      link: "/dashboard/user/points",
      icon: "icon:diamond",
    },
    {
      title: "آنالیز",
      link: "/dashboard/analyze",
      icon: "icon:heart-outline-new",
    },
    {
      title: "قوانین و مقررات",
      link: "/privacy-policies",
      icon: "icon:danger-v1",
    },
    {
      title: "تنظیمات",
      link: "/dashboard/user/settings",
      icon: "icon:setting",
    },
  ],
  specialist: [
    {
      title: "برنامه‌ها",
      link: "https://admin.padylife.com/",
      external: true,
      icon: "icon:golf",
    },
    {
      title: "گزارش دوره‌ها",
      link: "/dashboard/plans/plans-reports",
      icon: "icon:file-copy-2-line",
    },
    {
      title: "امتیازات من",
      link: "#",
      icon: "icon:diamond",
    },
    {
      title: "قوانین و مقررات",
      link: "/privacy-policies",
      icon: "icon:danger-v1",
    },
    {
      title: "تنظیمات",
      link: "/dashboard/specialist/my-information",
      icon: "icon:setting",
    },
  ],
};

defineExpose({
  closeSideBar,
});

const emits = defineEmits(["closeSideBar"]);

const isUserSidebar = computed(() => {
  return props.userRole == "user";
});

function closeSideBar() {
  emits("closeSideBar");
}

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
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }

}
</script>

<template>
  <div class="w-full h-screen max-h-screen bg-transparent fixed top-0 right-0 z-30 bg-black">
    <div class="w-full h-full relative flex flex-row justify-start items-center bg-[#171A1F]/60">
      <div class="sm:w-[20%] w-[70%] h-full bg-white flex flex-col rounded-tl-[4rem] overflow-hidden">
        <div class="w-full flex bg-primary flex-row justify-start items-center py-6 px-4 gap-2">
          <NuxtLink
            :to="useUtils().isSpecialist.value ? '/dashboard/specialist/my-information' : '/dashboard/user/settings/user-information'">
            <div class="h-[60px] w-[60px] rounded-full flex flex-row justify-center items-center relative">
              <Icon name="icon:user-edit" size="20" class="absolute [&_*]:stroke-white left-0 bottom-0" />
              <NuxtImg v-if="authStore?.getUser?.profileImage" :src="authStore.getUser.profileImage"
                class="w-full h-full object-cover rounded-full" />
              <NuxtImg v-else src="/common/no-image.png" class="w-full h-full object-cover rounded-full" />
            </div>
          </NuxtLink>
          <div class="flex flex-col gap-2">
            <span class="text-white">
              {{ authStore?.getUser?.fullName }}
            </span>
            <span class="text-white text-sm">
              {{ authStore?.getUserPoints?.availablePoints }} امتیاز
            </span>
          </div>
        </div>

        <div class="w-full h-full flex flex-col">
          <template v-for="(item, index) in menuItems[props.userRole]">
            <nuxt-link v-if="!item.external" :to="item.link" :class="[
              index !== menuItems[props.userRole].length - 1 ? 'border-b' : '',
            ]" class="w-full border-gray-300 flex flex-row justify-start items-end p-4 gap-x-4">
              <Icon v-if="item.icon" :name="item.icon" :color="isUserSidebar ? '#01CED1' : '#00ABFB'" :class="[
                isUserSidebar
                  ? '[&_*]:stroke-[#01CED1]'
                  : '[&_*]:stroke-[#00ABFB]',
              ]" size="22" />
              <span>
                {{ item.title }}
              </span>
            </nuxt-link>
            <a target="_blank" v-else :href="item.link" :class="[
              index !== menuItems[props.userRole].length - 1 ? 'border-b' : '',
            ]" class="w-full border-gray-300 flex flex-row justify-start items-end p-4 gap-x-4">
              <Icon v-if="item.icon" :name="item.icon" :color="isUserSidebar ? '#01CED1' : '#00ABFB'" :class="[
                isUserSidebar
                  ? '[&_*]:stroke-[#01CED1]'
                  : '[&_*]:stroke-[#00ABFB]',
              ]" size="22" />
              <span>
                {{ item.title }}
              </span>
            </a>
          </template>
          <div @click="logout"
            class="w-full border-t cursor-pointer border-gray-300 flex flex-row justify-start items-center p-4">
            <span>خروج از حساب کاربری</span>
          </div>
        </div>
      </div>
      <div class="sm:w-[80%] w-[30%] h-full" @click="closeSideBar"></div>
    </div>
  </div>
</template>

<style scoped>
test {
  color: #01CED1;
  background-color: #00abfb;
}
</style>
