<script setup lang="ts">
import * as Yup from "yup";
import type { IRegisterPayload } from "@/services/UsersService";
import { useAuthStore } from "~/stores/auth";
import type { ISignUpPlan } from "~/services/PlanService";
import { UserRoles } from "~/models/Enums/UserRoles";

useHead({
  title: 'ثبت نام'
})
const emailOrPhone = ref<string>("");
const isRenderingPassword = ref<boolean>(false);
const { $api } = useNuxtApp()
const registerPayload = ref<IRegisterPayload>({
  userName: "",
  phoneNumber: "",
  email: "",
  password: "",
  type: "User",
  verificationCode: ''
});

interface ILocalRegisterPayload
  extends Omit<IRegisterPayload, "phoneNumber" | "email"> {
  emailOrPhone: string;
}

const sendOtpSchema = Yup.object<IforgetPasswordPayload>({
  phoneNumber: Yup.string().nullable()
    .typeError('شماره موبایل وارد شده معتبر نمیباشد'),
});


// This schema is used for validating inputs before processing emailOrPhone
const localRegisterSchema = Yup.object<ILocalRegisterPayload>({
  userName: Yup.string().required("نام کاربری اجباری است"),
  password: Yup.string().required("رمز عبور اجباری است"),
  type: Yup.string().oneOf(["User", "Specialist"]),
});

const alerts = useAlerts();
const authStore = useAuthStore()
const signUpPlan = ref<ISignUpPlan | null>(null)
const isOtpSent = ref(false)
const otpInput = useTemplateRef('otpInput')
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

async function processEmail() {
  const emailSchema = Yup.string().email();
  const isValidEmail = await emailSchema.isValid(emailOrPhone.value);
  if (isValidEmail) {
    registerPayload.value.email = emailOrPhone.value;
    registerPayload.value.phoneNumber = undefined;
  } else {
    alerts.error("لطفا ایمیل را با فرمت درست وارد کنید");
  }
}

async function processPhoneNumber() {
  const phoneNumberSchema = Yup.string().min(
    11,
    "شماره همراه باید 11 رقم باشد"
  );
  const isValidNumber = await phoneNumberSchema.isValid(emailOrPhone.value);
  if (isValidNumber) {
    registerPayload.value.phoneNumber = emailOrPhone.value;
    registerPayload.value.email = undefined;
  } else {
    alerts.error("لطفا شماره همراه را با فرمت درست وارد کنید");
  }
}

async function processPhoneNumberOrEmail() {
  if (emailOrPhone.value) {
    if (emailOrPhone.value.includes("@")) {
      processEmail();
    } else {
      processPhoneNumber();
    }
  }
}

function setNumber(otp) {
  registerPayload.value.verificationCode = useUtils().convertNumbers2English(otp)
}

async function sendRegisterOtp() {
  if (registerPayload.value.phoneNumber) {
    useSpinner().renderSpinner()
    try {
      const response = await $api.users.sendOtpRegister(registerPayload.value.phoneNumber)
      if (response.isSuccess) {
        useAlerts().success('کد تایید به شماره موبایل شما ارسال شد')
        isOtpSent.value = true
      } else {
        alerts.error(response.message)
      }
    } catch (error) {

      console.error(error.message);
    } finally {
      useSpinner().hideSpinner()
    }
  } else {
    useAlerts().error('لطفا شماره موبایل خود را وارد کنید')
  }
}
async function sendRegisterRequest() {
  otpInput.value.makeJoinedNumbers()
  if (registerPayload.value.verificationCode) {
    try {
      const response = await $api.users.signUp(registerPayload.value)
      if (response.isSuccess) {
        if (registerPayload.value.type === UserRoles.Specialist)
          return alerts.success('ثبت نام شما با موفقیت انجام شد. حساب شما پس از تایید ادمین سایت فعال خواهد شد')
        router.push('/sign-in')
        await signIn()
      } else if (response.message === 'Fail') {
        alerts.error(response.data)
      } else {
        alerts.error(response.message)
      }
    } catch (error) {
      console.error(error);
    }
  } else {
    useAlerts().error('لطفا کد تایید ارسال شده به شماره موبایل خود را وارد کنید')
  }
}

async function signIn() {
  try {
    const response = await $api.users.signIn({
      username: registerPayload.value.userName,
      password: registerPayload.value.password
    })
    if (response.isSuccess) {
      if (!response.data.user.isActive)
        return alerts.error('حساب کاربری شما غیرفعال است. با پشتیبان سایت تماس حاصل فرمایید')
      await authStore.setUser(response.data)
      setTimeout(async () => {
        authStore.fetchUserPoints()
        // await processRoleCheck()
      }, 500)
    } else {
      alerts.error(response.data)
    }
  } catch (error) {
    console.error(error);
  }
}

// async function processRoleCheck() {
//   if (useUtils().isSpecialist.value) {
//     router.push("/dashboard/specialist");
//   } else {
//     setTimeout(async () => {
//       if (signUpPlan.value) {
//        await assignPlanToUser()
//       }
//       router.push(`/introduction/${authStore.getUser.userPlans[0].planId}/${authStore.getUser.userPlans[0].id}`);
//     }, 500)
//   }
// }

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
  }
}

async function validateRegisterPayload() {
  processPhoneNumberOrEmail();
  await sendRegisterRequest();
}

function toggleEye() {
  isRenderingPassword.value = !isRenderingPassword.value;
}

const router = useRouter();

function redirectToSignIn() {
  router.push("/sign-in");
}

</script>

<template>
  <div class="w-full h-screen flex flex-col justify-center gap-8 px-5">
    <div class="flex flex-col justify-center items-center h-screen gap-8 px-5 xl:flex-row xl:items-center">
      <div class="flex flex-col items-center w-full xl:w-1/2">
        <div class="w-full flex items-center justify-center mb-[8px]">
          <Icon name="icon:logo-typography-horizontal" size="50" color="#01CED1" />
        </div>

        <!-- <div
      class="w-16 h-16 rounded-full circle-bg absolute top-[20%] left-8"
    ></div> -->

        <div class="w-full max-w-md">
          <h3 class="text-2xl font-bold text-center my-8">ثبت‌نام در پادی لایف</h3>
          <UtilsFormWrapper v-if="!isOtpSent" :schema="sendOtpSchema" @submit="sendRegisterOtp">
            <div class="w-full space-y-5">
              <UtilsInputsBaseInput v-model="registerPayload.phoneNumber" name="phoneNumber" bordered type="tel"
                inputmode="numeric" placeholder="شماره تلفن"></UtilsInputsBaseInput>

              <button type="submit" class="w-full btn btn-primary text-[18px]">
                ارسال کد تایید
              </button>

            </div>
          </UtilsFormWrapper>

          <UtilsFormWrapper v-if="isOtpSent" :schema="localRegisterSchema" @submit="validateRegisterPayload">
            <div class="w-full space-y-5">
              <!-- <div class="space-y-4">
            <UtilsCustomRadioButton
              v-model="registerPayload.type"
              name="UserType"
              value="Specialist"
              label-custom-class="text-black"
              radio-custom-class="checked:bg-primary"
            >
              <template #label> متخصص هستم. </template>
</UtilsCustomRadioButton>
<UtilsCustomRadioButton v-model="registerPayload.type" name="UserType" value="User" label-custom-class="text-black"
  radio-custom-class="checked:bg-primary">
  <template #label> کاربر هستم. </template>
</UtilsCustomRadioButton>
</div> -->
              <UtilsInputsBaseInput v-model="registerPayload.userName" name="userName" bordered
                placeholder="نام کاربری">
              </UtilsInputsBaseInput>

              <UtilsInputsBaseInput v-model="registerPayload.password" :type="isRenderingPassword ? 'text' : 'password'"
                autocomplete="current-password" name="password" bordered placeholder="رمز عبور">
                <template #icon>
                  <button type="button" @click="toggleEye">
                    <Icon :name="isRenderingPassword ? 'icon:eye-off' : 'icon:eye-on'" color="#8C8C8C" />
                  </button>
                </template>
              </UtilsInputsBaseInput>
              <div class="w-full px-2">
                <div class="w-full flex items-center justify-between">
                  <span class="">کد تایید خود را وارد کنید</span>
                  <p @click="sendRegisterOtp" class="text-primary text-sm mt-2">ارسال مجدد کد</p>
                </div>
                <LazyUtilsOtpInput ref="otpInput" class="mt-3" @getNumber="setNumber" v-model="registerPayload.password"
                  :opt-count="6"></LazyUtilsOtpInput>
              </div>

              <button type="submit" class="w-full btn btn-primary text-[18px]">
                ایجاد حساب کاربری
              </button>


            </div>
          </UtilsFormWrapper>
          <div class="space-y-5 mt-3">
            <!-- <div class="w-full flex items-center justify-center gap-x-2">
          <hr class="w-full border-[#949494]"/>
          <span class="text-[#747679] text-sm"> یا </span>
          <hr class="w-full border-[#949494]"/>
        </div>
        <LazySharedGoogleAuth></LazySharedGoogleAuth> -->

            <button type="button" class="w-full text-neutral text-center" @click="redirectToSignIn">
              قبلا ثبت‌نام کرده‌اید؟ وارد شوید
            </button>
          </div>
        </div>
      </div>
      <div class="hidden xl:flex xl:w-1/2 items-center justify-center h-screen">
        <NuxtImg src="/junks/Picture1.png" class="h-full object-cover" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.custom-sign-up-form {
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