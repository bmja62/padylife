<script setup lang="ts">
const authStore = useAuthStore()
</script>

<template>
  <div class="w-full flex items-center justify-between p-7">
    <nuxt-link to="/dashboard/analyze"
      class="w-10 h-10 flex items-center justify-center  rounded-full"
    >
      <LazyIconsHeartIcon class="fill-white w-8 h-8"></LazyIconsHeartIcon>
    </nuxt-link>
    <nuxt-link to="/dashboard/user">
      <NuxtImg
          v-if="authStore?.getUser?.profileImage"
          :src="authStore.getUser.profileImage"
          class="w-10 h-10 rounded-full object-cover"
      />
      <NuxtImg
          v-else
          src="/common/no-image.png"
          class="w-10 h-10 rounded-full object-cover"
      />


    </nuxt-link>
    <LazySharedPointsIdentifier></LazySharedPointsIdentifier>
<!--    <nuxt-link to="/dashboard/user/points"-->
<!--      class="w-10 h-10 flex items-center justify-center bg-white rounded-full shadow-[0px_5px_7.5px_rgba(0,0,0,0.3),0px_5px_6px_rgba(50,50,93,0.25),inset_0px_-2px_3px_rgba(156,84,84,0.35)]"-->
<!--    >-->
<!--      <NuxtImg src="/core/diamond.png" class="w-7 h-auto object-contain" />-->
<!--    </nuxt-link>-->
  </div>
</template>
