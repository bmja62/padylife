<script setup lang="ts">
import * as Yup from "yup";
import type { IForgetPasswordPayload } from "@/services/UsersService";

useHead({
  title: 'ورود'
})
const forgetPasswordPayload = ref<IForgetPasswordPayload>({
  phoneNumber: '',
  resetCode: "",
  newPassword: "",
});
const authStore = useAuthStore()
const { $api } = useNuxtApp()
const alerts = useAlerts()
const isRenderingPassword = ref(false)
const isOtpSent = ref(false)
const route = useRoute()
const router = useRouter();

function toggleEye() {
  isRenderingPassword.value = !isRenderingPassword.value;
}



async function changePasswordByOtp() {
  useSpinner().renderSpinner()
  try {
    const response = await $api.users.setNewPassword(forgetPasswordPayload.value)
    if (response.isSuccess) {
      useAlerts().success('رمزعبور شما با موفقیت تغییر یافت')
      router.push('/sign-in')
    } else {
      alerts.error(response.message)
    }
  } catch (error) {

    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

async function sendOtpForgetPassword() {
  if (forgetPasswordPayload.value.phoneNumber) {
    useSpinner().renderSpinner()
    try {
      const response = await $api.users.sendOtpForgetPassword(forgetPasswordPayload.value.phoneNumber)
      if (response.isSuccess) {
        useAlerts().success('کد تایید به شماره موبایل شما ارسال شد')
        isOtpSent.value = true
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
  } else {
    useAlerts().error('لطفا شماره موبایل خود را وارد کنید')

  }
}

const sendOtpSchema = Yup.object<IforgetPasswordPayload>({
  phoneNumber: Yup.string().nullable()
});
const changePasswordSchema = Yup.object<IforgetPasswordPayload>({
  resetCode: Yup.string().required("این فیلد اجباری است"),
  newPassword: Yup.string().required("این فیلد اجباری است"),
});
</script>


<template>
  <div class="w-full h-screen flex flex-col justify-center gap-8 px-5">
    <div class="flex flex-col justify-center items-center h-screen gap-8 px-5 xl:flex-row xl:items-center">
      <div class="flex flex-col items-center w-full xl:w-1/2">
        <div class="w-full flex items-center justify-center mb-[8px]">
          <Icon name="icon:logo-typography-horizontal" size="50" color="#01CED1" />
        </div>
        <!-- <div
        class="w-16 h-16 rounded-full circle-bg absolute top-[36%] right-8"
        ></div> -->
        <div class="w-full max-w-md">
          <h3 class="text-2xl font-bold text-center my-8">فراموشی رمزعبور</h3>
          <UtilsFormWrapper v-if="!isOtpSent" :schema="sendOtpSchema" @submit="sendOtpForgetPassword">
            <div class="w-full flex flex-col gap-y-5 mt-20">
              <UtilsInputsBaseInput v-model="forgetPasswordPayload.phoneNumber" name="phoneNumber" bordered type="tel"
                inputmode="numeric" placeholder="شماره موبایل خود را وارد کنید"></UtilsInputsBaseInput>
              <button type="submit" class="btn btn-primary text-[18px]">
                ارسال کد تایید
              </button>
              <nuxt-link to="/sign-in" class="w-full text-neutral text-center">
                بازگشت به صفحه ورود
              </nuxt-link>
            </div>
          </UtilsFormWrapper>


          <UtilsFormWrapper v-if="isOtpSent" :schema="changePasswordSchema" @submit="changePasswordByOtp">
            <div class="w-full flex flex-col gap-y-5 mt-20">
              <UtilsInputsBaseInput v-model="forgetPasswordPayload.resetCode" name="resetCode" bordered type="tel"
                inputmode="numeric" placeholder="کد ارسال شده به شماره موبایل خود را وارد کنید"></UtilsInputsBaseInput>
              <div>
                <UtilsInputsBaseInput v-model="forgetPasswordPayload.newPassword"
                  :type="isRenderingPassword ? 'text' : 'password'" name="newPassword" bordered
                  placeholder="رمز عبور جدید را وارد کنید">
                  <template #icon>
                    <Icon :name="isRenderingPassword ? 'icon:eye-off' : 'icon:eye-on'" color="#8C8C8C"
                      @click="toggleEye" />
                  </template>
                </UtilsInputsBaseInput>
                <div class="w-full flex items-center justify-between">
                  <p @click="sendOtpForgetPassword" class="text-black text-sm mt-2">ارسال مجدد کد</p>
                </div>
              </div>
              <button type="submit" class="btn btn-primary text-[18px]">
                ویرایش رمزعبور
              </button>

              <nuxt-link to="/sign-in" class="w-full text-neutral text-center">
                بازگشت به صفحه ورود
              </nuxt-link>
            </div>
          </UtilsFormWrapper>
        </div>
      </div>
      <div class="hidden xl:flex xl:w-1/2 items-center justify-center h-screen">
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
  background: linear-gradient(131deg, #BED4BE  -3.66%, #01CED1 103.66%);
} */
</style>
