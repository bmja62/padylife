<script lang="ts" setup>
import { storeToRefs } from 'pinia'
import { useAlerts } from '@/composables/alert'
import { useAlertStore } from '@/stores/alerts'

import navItems from '@/navigation/vertical'
import { useThemeConfig } from '@core/composable/useThemeConfig'

// Components
import Footer from '@/layouts/components/Footer.vue'
import NavbarThemeSwitcher from '@/layouts/components/NavbarThemeSwitcher.vue'
import UserProfile from '@/layouts/components/UserProfile.vue'

// @layouts plugin
import { VerticalNavLayout } from '@layouts'

const { appRouteTransition, isLessThanOverlayNavBreakpoint } = useThemeConfig()
const alertStore = useAlertStore()
const alert = useAlerts()

const { isRenderingErrorAlert, isRenderingSuccessAlert, alertMessage } = storeToRefs(alertStore)
const { width: windowWidth } = useWindowSize()

function closeAlert() {
  alert.close()
}
</script>

<template>
  <VerticalNavLayout :nav-items="navItems">
    <VAlert
      v-if="isRenderingErrorAlert || isRenderingSuccessAlert"
      class="custom-alert"
      :title="isRenderingErrorAlert ? 'مشکلی پیش آمد' : 'عملیات موفق'"
      :type="isRenderingSuccessAlert ? 'success' : 'error'"
      @click.stop="closeAlert"
    >
      {{ alertMessage }}
    </VAlert>
    <!-- 👉 navbar -->
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center">
        <IconBtn
          v-if="isLessThanOverlayNavBreakpoint(windowWidth)"
          id="vertical-nav-toggle-btn"
          class="ms-n3"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon
            size="26"
            icon="tabler-menu-2"
          />
        </IconBtn>

        <NavbarThemeSwitcher />

        <VSpacer />

        <!-- <NavBarI18n /> -->

        <!-- {{ userData ? userData.roles[0] : '-' }} -->

        <UserProfile />
      </div>
    </template>

    <!-- 👉 Pages -->
    <RouterView v-slot="{ Component }">
      <Transition
        :name="appRouteTransition"
        mode="out-in"
      >

        <Component :is="Component" />
      </Transition>
    </RouterView>

    <!-- 👉 Footer -->
    <template #footer>
      <Footer />
    </template>

    <!-- 👉 Customizer -->
    <!-- <TheCustomizer /> -->
  </VerticalNavLayout>
</template>
