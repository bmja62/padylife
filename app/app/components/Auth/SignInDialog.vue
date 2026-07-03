<script setup lang="ts">
const isRenderingSignInDialog = defineModel<boolean>();

const emailOrPhone = ref<string>("");
const password = ref<string>("");
const isRenderingPassword = ref<boolean>(false);

function toggleEye() {
  isRenderingPassword.value = !isRenderingPassword.value;
}

const authorize = useAuthorize();
</script>

<template>
  <UtilsDialogsBaseDialog
    v-model="isRenderingSignInDialog"
    dialog-id="signInDialog"
  >
    <template #title> ورود به پادی لایف </template>
    <div class="w-full flex flex-col gap-y-5">
      <UtilsInputsBaseInput
        v-model="emailOrPhone"
        name="emailOrPhone"
        placeholder="ایمیل / شماره تلفن"
      ></UtilsInputsBaseInput>
      <div>
        <UtilsInputsBaseInput
          v-model="password"
          :type="isRenderingPassword ? 'text' : 'password'"
          name="password"
          placeholder="رمز عبور"
        >
          <template #icon>
            <Icon
              :name="isRenderingPassword ? 'icon:eye-off' : 'icon:eye-on'"
              color="#8C8C8C"
              @click="toggleEye"
            />
          </template>
        </UtilsInputsBaseInput>
        <p class="text-black text-sm mt-2">فراموشی رمز عبور</p>
      </div>
      <button type="button" class="btn btn-primary text-[18px]">
        ورود به حساب کاربری
      </button>
      <div class="w-full flex items-center justify-center gap-x-2">
        <hr class="w-full border-[#949494]" />
        <span class="text-[#747679] text-sm"> یا </span>
        <hr class="w-full border-[#949494]" />
      </div>
      <LazySharedGoogleAuth></LazySharedGoogleAuth>

      <button
        type="button"
        class="w-full text-neutral text-center"
        @click="authorize.renderSignUp"
      >
        هنوز عضو نشده‌اید؟ ثبت‌نام کنید
      </button>
    </div>
  </UtilsDialogsBaseDialog>
</template>
