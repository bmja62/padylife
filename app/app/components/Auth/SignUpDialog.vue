<script setup lang="ts">
const isRenderingSignUpDialog = defineModel<boolean>();

const isUser = ref<boolean>(false);
const userName = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
const isRenderingPassword = ref<boolean>(false);

function toggleEye() {
  isRenderingPassword.value = !isRenderingPassword.value;
}

const authorize = useAuthorize();
</script>

<template>
  <UtilsDialogsBaseDialog
    v-model="isRenderingSignUpDialog"
    dialog-id="signUpDialog"
  >
    <template #title> ثبت‌نام در پادی لایف </template>
    <div class="w-full space-y-5">
      <div class="space-y-4">
        <UtilsCustomRadioButton
          v-model="isUser"
          name="isUser"
          :value="false"
          label-custom-class="text-black"
        >
          <template #label> متخصص هستم. </template>
        </UtilsCustomRadioButton>
        <UtilsCustomRadioButton
          v-model="isUser"
          name="isUser"
          :value="true"
          label-custom-class="text-black"
        >
          <template #label> کاربر هستم. </template>
        </UtilsCustomRadioButton>
      </div>
      <UtilsInputsBaseInput
        v-model="userName"
        name="userName"
        placeholder="نام کاربری"
      ></UtilsInputsBaseInput>

      <UtilsInputsBaseInput
        v-model="email"
        name="email"
        placeholder="ایمیل / شماره تلفن"
      ></UtilsInputsBaseInput>
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
      <button type="button" class="w-full btn btn-primary text-[18px]">
        ایجاد حساب کاربری
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
        @click="authorize.renderSignIn"
      >
        قبلا ثبت‌نام کرده‌اید؟ وارد شوید
      </button>
    </div>
  </UtilsDialogsBaseDialog>
</template>
