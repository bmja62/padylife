<script setup lang="ts">
const authStore = useAuthStore()
</script>

<template>
  <div v-if="authStore?.getUser" class="relative">
    <NuxtImg
        v-if="authStore?.getUser?.iconUrl"
        :src="authStore.getUser.iconUrl"
        class="w-[100px] absolute h-[100px]  -bottom-[10px] object-cover rounded-full"
    />
    <NuxtImg
        v-if="authStore?.getUser?.profileImage"
        :src="authStore.getUser.profileImage"
        class="w-[80px] h-[80px] object-cover rounded-full"
    />
    <NuxtImg
        v-else
        src="/common/no-image.png"
        class="w-[80px] h-[80px] object-cover rounded-full"
    />
  </div>

</template>

<style scoped>

</style>