<script setup lang="ts">
import avatar1 from '@images/avatars/avatar-1.png'
import {useAuthStore} from '@/stores/auth'

// Variables
const auth = useAuthStore()

// Functions
function logOut() {
  auth.logout()
}
</script>

<template>
  <VBadge
    dot
    location="bottom right"
    offset-x="3"
    offset-y="3"
    bordered
    color="success"
  >
    <span v-if="auth.isLogged" class="custom-name">{{ auth.getUser.userName }}</span>

    <VAvatar
      class="cursor-pointer"
      color="primary"
      variant="tonal"
    >
      <VImg :src="avatar1"/>

      <!-- SECTION Menu -->
      <VMenu
        activator="parent"
        width="230"
        location="bottom end"
        offset="14px"
      >
        <VList>
          <!-- 👉 User Avatar & Name -->
          <!-- 👉 Logout -->
          <VListItem @click="logOut">
            <template #prepend>
              <VIcon
                class="me-2"
                icon="tabler-logout"
                size="22"
              />
            </template>

            <VListItemTitle>خروج</VListItemTitle>
          </VListItem>
        </VList>
      </VMenu>
      <!-- !SECTION -->
    </VAvatar>
  </VBadge>
</template>

<style>
.custom-name {
  padding-top: 11px;
  padding-left: 10px;
}
</style>
