<script setup lang="ts">
import CustomDatePicker from "~/components/utils/CustomDatePicker.vue";
import * as Yup from "yup";
import {UploaderTypes} from "~/models/Enums/UplaoderTypes";

definePageMeta({
  layout: "dashboard",
});
useHead({
  title:'اطلاعات کاربری'
})

const {$api} = useNuxtApp()
const profileImageRef = useTemplateRef('profileImageRef')
const router = useRouter();
function goBack() {
  router.go(-1);
}

const userSchema = Yup.object({
  fullName: Yup.string().required("نام و نام خانوادگی اجباری است"),
  hight: Yup.string().required("قد اجباری است"),
  age: Yup.string().required("سن اجباری است"),
  wight: Yup.string().required("وزن اجباری است"),
  phoneNumber: Yup.string().required("شماره تلفن اجباری است"),
});
const authStore = useAuthStore()

async function updateUser() {
  useSpinner().renderSpinner()
  try {
    profileImageRef.value.exposeMedia()
    const response = await $api.users.updateUser(authStore.getUser)
    if (response.isSuccess) {
      useAlerts().success('اطلاعات شما با موفقیت ویرایش شد')
      authStore.fetchUser()
    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.log(error)
    if (error.statusCode === 400) {
      useAlerts().error(error.message)
    }
  } finally {
    useSpinner().hideSpinner()
  }
}
function setProfileImage(medias: []) {
  if (medias.length) {
    authStore.getUser.profileImage = medias[0]
  }
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">اطلاعات کاربری</p>
          </template>
        </BaseSpecialistHeader>
      </template>
      <UtilsFormWrapper :schema="userSchema" @submit="updateUser">
        <div class="flex flex-col gap-4 py-2">
          <LazyUtilsUploader
              id="profileImageRef"
              ref="profileImageRef"
              :default-media="authStore.getUser.profileImage"
              @setMedia="setProfileImage" :fileType="UploaderTypes.ProfileImage"
              title="آپلود عکس پروفایل *"
              custom-height="h-[100px]"></LazyUtilsUploader>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.fullName"
                name="fullName"
                placeholder="نام و نام خانوادگی"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <LazyUtilsPickersGenderPicker v-model="authStore.getUser.gender"></LazyUtilsPickersGenderPicker>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.age"
                name="age"
                placeholder="سن"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <CustomDatePicker v-model="authStore.getUser.birthdate" simple  placeholder=" تاریخ تولد" label="تاریخ تولد"></CustomDatePicker>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.hight"
                name="hight"
                placeholder="قد"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.wight"
                name="wight"
                placeholder="وزن"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.jobTitle"
                name="jobTitle"
                placeholder="عنوان شغلی"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <LazyUtilsPickersMaritialStatusPicker
                v-model="authStore.getUser.maritalStatus"
            ></LazyUtilsPickersMaritialStatusPicker>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.email"
                name="email"
                placeholder="ایمیل"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.phoneNumber"
                name="phoneNumber"
                placeholder="شماره تلفن"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>
          <DashboardInfoBox class="custom-info-box-shadow">
            <UtilsInputsBaseInput
                bordered
                v-model="authStore.getUser.instagramId"
                name="instagramId"
                placeholder="پیج اینستاگرام"
            ></UtilsInputsBaseInput>
          </DashboardInfoBox>

          <div class="w-full">
            <button type="submit" class="btn w-full bg-primary text-white">ویرایش اطلاعات</button>
          </div>
        </div>
      </UtilsFormWrapper>

    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-info-box-shadow {
  box-shadow: 0px 2px 6px 2px rgba(60, 64, 67, 0.15),
  0px 1px 2px 0px rgba(60, 64, 67, 0.3);
}
</style>
