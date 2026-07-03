<script setup lang="ts">
const authorize = useAuthorize();
const spinnerStore = useSpinnerStore();
const alertsStore = useAlertStore();
</script>

<template>
  <div class="w-full h-full">
    <NuxtLayout>
      <BaseSpinnerPage v-show="spinnerStore.isLoading"></BaseSpinnerPage>
      <teleport to="body">
        <UtilsAlertCustomAlert v-show="alertsStore.isRenderingAlertDialog"/>
      </teleport>

      <AuthSignInDialog
        v-model="authorize.isRenderingSignInDialog.value"
      ></AuthSignInDialog>

      <AuthSignUpDialog
        v-model="authorize.isRenderingSignUpDialog.value"
      ></AuthSignUpDialog>

      <NuxtPage />
    </NuxtLayout>
  </div>
</template>
