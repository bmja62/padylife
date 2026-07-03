<script setup lang="ts">
import { LazyIconsBookIcon } from '#components';

const img = useImage();
const authStore = useAuthStore()
const isRenderingChoicesDialog = ref(false)
const bottomNavBackground = computed(() => {
  const imgUrl = img("/core/bottom-nav.png");
  return { backgroundImage: `url('${imgUrl}')` };
});
</script>

<template>
  <div v-if="authStore.isLogged"
    class=" h-[8lvh] bg-no-repeat bg-center bg-cover px-8 flex items-center justify-between z-20"
    :style="bottomNavBackground">
    <div class="w-1/3 h-full grid grid-cols-2 items-center justify-center">
      <NuxtLink to="/dashboard" active-class="link-active" class="fill-gray-400">
        <LazyBaseBottomNavIconsHomeIcon class="w-5 h-5"></LazyBaseBottomNavIconsHomeIcon>
      </NuxtLink>
      <NuxtLink to="/dashboard/user/settings" active-class="link-active" >
        <LazyBaseBottomNavIconsRobotIcon class="w-5 h-5 fill-gray-400"></LazyBaseBottomNavIconsRobotIcon>
      </NuxtLink>
    </div>
    <NuxtLink to="/dashboard/plans/add">
      <div class="w-14 h-14 cursor-pointer rounded-full bg-[#01CED1] mb-14 flex items-center justify-center">
        <Icon name="icon:plus" size="25" color="#ffffff" />
      </div>
    </NuxtLink>
    <div class="w-1/3 h-full grid grid-cols-2 items-center justify-items-end">
      <NuxtLink to="/dashboard/user/my-plans" class="" active-class="link-active">
        <LazyIconsBookIcon class="w-5 h-5 fill-gray-400" />
      </NuxtLink>
      <NuxtLink :to="useUtils().isSpecialist.value ? '/dashboard/specialist' : '/dashboard/user'"
        active-class="link-active" class="fill-gray-400">
        <LazyBaseBottomNavIconsUserIcon class="w-5 h-5"></LazyBaseBottomNavIconsUserIcon>
      </NuxtLink>
    </div>
    <LazyUtilsDialogsChoicesDialog v-model="isRenderingChoicesDialog"></LazyUtilsDialogsChoicesDialog>
  </div>
</template>
<style>
.link-active {
  @apply [&_*]:fill-primary !important;
}
</style>