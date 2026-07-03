<script setup lang="ts">
const authStore = useAuthStore()
</script>

<template>
  <nuxt-link to="/dashboard/notifications" class="relative">
    <Icon name="icon:bell-new" color="white" size="20" />
    <div
    v-if="authStore.getNotificationCount"
        class="w-5 text-primary h-5 text-xs -top-2 -right-2 absolute bg-white rounded-full flex items-center justify-center">
      {{ authStore.getNotificationCount }}
    </div>
  </nuxt-link>
</template>

<style scoped>

</style>