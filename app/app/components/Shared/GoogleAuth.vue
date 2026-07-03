<script setup lang=ts>
const { initializeGoogleAuth, signInWithGoogle } = useGoogleAuth()

async function googleLogin() {
  try {
    await initializeGoogleAuth()
    signInWithGoogle()
  } catch (error) {
    console.error('Google login error:', error)
    useAlerts().error('خطا در ورود با گوگل')
  }
}
</script>
<template>
  <button type="button" class="btn border-primary w-full !text-black bg-white/45" @click="googleLogin">
    <Icon name="icon:google-auth-colored" />
    ورود با اکانت گوگل
  </button>
</template>