<script setup lang="ts">
import { useTheme } from 'vuetify'
import { storeToRefs } from 'pinia'
import ScrollToTop from '@core/components/ScrollToTop.vue'
import { useThemeConfig } from '@core/composable/useThemeConfig'
import { hexToRgb } from '@layouts/utils'
import { useSpinnerStore } from '@/stores/spinner'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const spinnerStore = useSpinnerStore()

const { isRenderingSpinner } = storeToRefs(spinnerStore)

const { syncInitialLoaderTheme, syncVuetifyThemeWithTheme: syncConfigThemeWithVuetifyTheme, isAppRtl, handleSkinChanges } = useThemeConfig()

const { global } = useTheme()


// ℹ️ Sync current theme with initial loader theme
syncInitialLoaderTheme()
syncConfigThemeWithVuetifyTheme()
handleSkinChanges()
</script>

<template>
  <VLocaleProvider :rtl="isAppRtl">
    <!-- ℹ️ This is required to set the background color of active nav link based on currently active global theme's primary -->
    <VApp :style="`--v-global-theme-primary: ${hexToRgb(global.current.value.colors.primary)}`">
      <VOverlay
        :model-value="isRenderingSpinner"
        class="align-center justify-center"
      >
        <VProgressCircular
          color="warning"
          indeterminate
          size="80"
        />
      </VOverlay>
      <RouterView />
      <PwaInstallDialog />
      <!-- Here -->
      <ScrollToTop />
    </VApp>
  </VLocaleProvider>
</template>
