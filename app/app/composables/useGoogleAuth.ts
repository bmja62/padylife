import type {IGoogleCredentials} from "~/services/UsersService";

export const useGoogleAuth = () => {
  const config = useRuntimeConfig()
  const router = useRouter()
  const route = useRoute()
  const authStore = useAuthStore()
  const {$api} = useNuxtApp()
  const signUpPlan = ref(null)
  const initializeGoogleAuth = () => {
    if (process.client) {
      // Load Google Identity Services library
      const script = document.createElement('script')
      script.src = 'https://accounts.google.com/gsi/client'
      script.async = true
      script.defer = true
      document.head.appendChild(script)
      return new Promise((resolve) => {
        script.onload = () => {
          if (window.google) {
            window.google.accounts.id.initialize({
              client_id: config.public.googleClientId,
              callback: handleGoogleResponse,
              auto_select: false,
              cancel_on_tap_outside: true
            })
            resolve(true)
          }
        }
      })
    }
  }

  const handleGoogleResponse = async (response: IGoogleCredentials) => {
    try {
      useSpinner().renderSpinner()
      const res = await $api.users.sendGoogleToken(response.credential)
      if (res.isSuccess) {
        if (!res.data.data.user.isActive) {
          return useAlerts().error('حساب کاربری شما غیرفعال است. با پشتیبان سایت تماس حاصل فرمایید')
        }
        await authStore.setUser(res.data.data)
        await getSignUpPlan()
        setTimeout(() => {
          processRoleCheck()
          authStore.fetchUserPoints()
        }, 1000)
      } else {
        useAlerts().error(res.message)
      }
    } catch (error) {
      console.error('Google login error:', error)
      useAlerts().error('خطا در ورود با گوگل')
    } finally {
      useSpinner().hideSpinner()
    }
  }

  async function processRoleCheck() {
    if (useUtils().isSpecialist.value) {
      // isRenderingWelcomeBackDialog.value = true
    } else {
      if (authStore?.getUser?.userPlans?.length) {
        await getNextUnAnsweredQuestion()
      } else {
        await assignPlanToUser()
      }
    }
  }

  async function assignPlanToUser() {
    try {
      const response = await $api.plan.assignPlanToUser({
        planId: signUpPlan.value.id,
        userId: authStore.getUser.id
      })
      if (response.isSuccess) {
        await authStore.fetchUser()
      } else {
        alerts.error(response.data)
      }
    } catch (error) {
      console.error(error);
    } finally {
      await getNextUnAnsweredQuestion()
    }
  }

  async function getSignUpPlan() {
    try {
      useSpinner().renderSpinner()
      const response = await $api.plan.getSignUpPlan();
      signUpPlan.value = response.data
    } catch (e) {
      console.log(e)
    } finally {
      useSpinner().hideSpinner()
    }
  }


  async function getNextUnAnsweredQuestion() {
    try {
      const response = await $api.plan.getNextUnAnsweredQuestion({
        planId: authStore.getUser.userPlans[0].planId,
        userId: authStore.getUser.id
      })
      if (response.isSuccess && response.data === undefined) {
        goToDashboard()
      } else if (response.isSuccess && response.data) {
        router.push(`/introduction/${authStore.getUser.userPlans[0].planId}/${authStore.getUser.userPlans[0].id}`);
      } else {
        alerts.error(response.message)
      }
    } catch (error) {
      if (error.statusCode === 400) {
        alerts.error(error.message)
      }
      console.error(error.message);
    }
  }

  function goToDashboard() {
    if (useUtils().isSpecialist.value) {
      router.push("/dashboard/specialist");
    } else {
      if (route.query.redirectUrl) {
        router.push(route.query.redirectUrl);
      } else {
        router.push("/dashboard");
      }
    }

  }

  const signInWithGoogle = () => {
    console.log(authStore.setUser())
    if (process.client && window.google) {
      window.google.accounts.id.prompt()
    }
  }

  return {
    initializeGoogleAuth,
    signInWithGoogle,
    handleGoogleResponse
  }
}

