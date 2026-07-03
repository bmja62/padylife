<script setup lang="ts">
import * as Yup from "yup";
import type { ILoginPayload } from "@/services/UsersService";
import type { ISignUpPlan } from "~/services/PlanService";

useHead({
  title: 'ورود'
})
const loginPayload = ref<ILoginPayload>({
  username: "",
  password: "",
});
const authStore = useAuthStore()
const isRenderingPassword = ref<boolean>(false);
const { $api } = useNuxtApp()
const isRenderingWelcomeBackDialog = ref(false)
const alerts = useAlerts()
const signUpPlan = ref<ISignUpPlan | null>(null)
const route = useRoute()
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

getSignUpPlan()

function toggleEye() {
  isRenderingPassword.value = !isRenderingPassword.value;
}

const router = useRouter();

function redirectToSignUp() {
  router.push("/sign-up");
}

function redirectToIntroduction() {
  router.push("/introduction");
}


async function sendSignInRequest() {
  useSpinner().renderSpinner()
  try {
    const response = await $api.users.signIn(loginPayload.value)
    if (response.isSuccess) {
      if (!response.data.user.isActive)
        return alerts.error('حساب کاربری شما غیرفعال است. با پشتیبان سایت تماس حاصل فرمایید')
      await authStore.setUser(response.data)
      setTimeout(() => {
        processRoleCheck()
        authStore.fetchUserPoints()
      }, 500)
    } else {
      alerts.error(response.data)
    }
  } catch (error) {
    if (error.statusCode === 400) {
      alerts.error(error.message)
    }
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}



async function processRoleCheck() {
  if (useUtils().isSpecialist.value) {
    isRenderingWelcomeBackDialog.value = true
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

async function getNextUnAnsweredQuestion() {
  router.push('/dashboard')
  // try {
  //   const response = await $api.plan.getNextUnAnsweredQuestion({
  //     planId: authStore.getUser.userPlans[0].planId,
  //     userId: authStore.getUser.id
  //   })
  //   if (response.isSuccess && response.data === undefined) {
  //     isRenderingWelcomeBackDialog.value = true
  //   } else if (response.isSuccess && response.data) {
  //     router.push(`/introduction/${authStore.getUser.userPlans[0].planId}/${authStore.getUser.userPlans[0].id}`);
  //   } else {
  //     alerts.error(response.message)
  //   }
  // } catch (error) {
  //   if (error.statusCode === 400) {
  //     alerts.error(error.message)
  //   }
  //   console.error(error.message);
  // }
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

const signInSchema = Yup.object<ILoginPayload>({
  userName: Yup.string().required("نام کاربری اجباری است"),
  password: Yup.string().required("رمز عبور اجباری است"),
});
</script>


<template>

  <div class="w-full h-screen flex flex-col justify-center gap-8 px-5">
    <div class="flex flex-col justify-center items-center h-screen gap-8 px-5 xl:flex-row xl:items-center">
      <div class="flex flex-col items-center w-full xl:w-1/2">
        <div class="w-full flex items-center justify-center mb-[8px]">
          <Icon name="icon:logo-typography-horizontal" size="50" color="#01CED1" />
        </div>

        <!--    <div-->
        <!--      class="w-28 h-28 rounded-full circle-bg absolute top-[20%] left-8"-->
        <!--    ></div>-->
        <!-- <div
      class="w-16 h-16 rounded-full circle-bg absolute top-[36%] right-8"
    ></div> -->

        <div class="w-full max-w-md ">
          <h3 class="text-2xl font-bold text-center my-10">ورود به پادی لایف</h3>
          <UtilsFormWrapper :schema="signInSchema" @submit="sendSignInRequest">
            <div class="w-full flex flex-col gap-y-5">
              <UtilsInputsBaseInput v-model="loginPayload.username" name="userName" bordered placeholder="نام کاربری"
                @keydown.enter="sendSignInRequest"></UtilsInputsBaseInput>
              <div>
                <UtilsInputsBaseInput v-model="loginPayload.password" :type="isRenderingPassword ? 'text' : 'password'"
                  name="password" bordered placeholder="رمز عبور" @keydown.enter="sendSignInRequest">
                  <template #icon>
                    <Icon :name="isRenderingPassword ? 'icon:eye-off' : 'icon:eye-on'" color="#8C8C8C"
                      @click="toggleEye" />
                  </template>
                </UtilsInputsBaseInput>
                <div class="w-full flex items-center justify-between">
                  <nuxt-link to="/forget-password" class="text-black text-sm mt-2">فراموشی رمز عبور</nuxt-link>
                  <nuxt-link to="/privacy-policies" class="text-primary text-sm mt-2">قوانین و حریم خصوصی</nuxt-link>
                </div>
              </div>
              <button type="submit" class="btn btn-primary text-[18px]">
                ورود به حساب کاربری
              </button>
              <!-- <div class="w-full flex items-center justify-center gap-x-2">
            <hr class="w-full border-black" />
            <span class="text-black text-sm"> یا </span>
            <hr class="w-full border-black" />
          </div>
         <LazySharedGoogleAuth></LazySharedGoogleAuth> -->
              <button type="button" class="w-full text-neutral text-center" @click="redirectToSignUp">
                هنوز عضو نشده‌اید؟ ثبت‌نام کنید
              </button>
            </div>
          </UtilsFormWrapper>
          <LazyUtilsDialogsWelcomeBackDialog @continue="goToDashboard" v-model="isRenderingWelcomeBackDialog">
          </LazyUtilsDialogsWelcomeBackDialog>
        </div>
      </div>

      <div class="hidden xl:flex sm:w-1/2 items-center justify-center h-screen">
        <NuxtImg src="/junks/Picture1.png" class="h-full object-cover" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.custom-sign-in-form {
  border-radius: 20px 20px 0px 0px;
  border: 1px solid rgba(255, 255, 255, 0.3);

  background: linear-gradient(140deg,
      rgba(232, 232, 232, 0.24) 1.8%,
      rgba(255, 92, 97, 0.32) 90.75%);

  backdrop-filter: blur(20px);
}

/* .circle-bg {
  background: linear-gradient(131deg, #BED4BE -3.66%, #01CED1 103.66%);
} */
</style>