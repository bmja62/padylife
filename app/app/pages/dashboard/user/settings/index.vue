<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
});
useHead({
  title:'تنظیمات'
})

const router = useRouter();
function goBack() {
  router.go(-1);
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">تنظیمات</p>
          </template>
        </BaseSpecialistHeader>
      </template>

      <div class="flex flex-col gap-4">
        <NuxtLink
          to="/dashboard/user/settings/user-information"
          class="w-full flex flex-row justify-between items-center p-4 setting-box-shadow bg-white rounded-full"
        >
          <div class="flex flex-row justify-start items-center gap-x-2">
            <Icon name="icon:user-v3" class="[&_*]:fill-[#4E4E4E]" size="15" />
            <strong class="text-gray-700 text-sm">اطلاعات کاربری</strong>
          </div>
          <Icon name="icon:chevron-left" size="15" />
        </NuxtLink>
        <!-- <div
          class="w-full flex flex-row justify-between items-center p-4 setting-box-shadow bg-white rounded-full"
        >
          <div class="flex flex-row justify-start items-center gap-x-2">
            <Icon name="icon:bell-new" class="[&_*]:fill-[#4E4E4E]" size="15" />
            <strong class="text-gray-700 text-sm">اعلانات</strong>
          </div>
          <input
            type="checkbox"
            class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
          />
        </div> -->
        <NuxtLink
          to="/dashboard/user/addresses"
          class="w-full flex flex-row justify-between items-center p-4 setting-box-shadow bg-white rounded-full"
        >
          <div class="flex flex-row justify-start items-center gap-x-2">
            <Icon name="icon:location-new" color="#4E4E4E" size="15" />
            <strong class="text-gray-700 text-sm">آدرس‌ها</strong>
          </div>
          <Icon name="icon:chevron-left" size="15" />
        </NuxtLink>
<!--        <NuxtLink-->
<!--          to="#"-->
<!--          class="w-full flex flex-row justify-between items-center p-4 setting-box-shadow bg-white rounded-full"-->
<!--        >-->
<!--          <div class="flex flex-row justify-start items-center gap-x-2">-->
<!--            <Icon-->
<!--              name="icon:security"-->
<!--              class="[&_*]:stroke-[#4E4E4E]"-->
<!--              size="15"-->
<!--            />-->
<!--            <strong class="text-gray-700 text-sm">حریم شخصی و امنیت</strong>-->
<!--          </div>-->
<!--          <Icon name="icon:chevron-left" size="15" />-->
<!--        </NuxtLink>-->
<!--        <NuxtLink-->
<!--          to="#"-->
<!--          class="w-full flex flex-row justify-between items-center p-4 setting-box-shadow bg-white rounded-full"-->
<!--        >-->
<!--          <div class="flex flex-row justify-start items-center gap-x-2">-->
<!--            <Icon-->
<!--              name="icon:user-delete"-->
<!--              class="[&_*]:fill-[#4E4E4E]"-->
<!--              size="15"-->
<!--            />-->
<!--            <strong class="text-gray-700 text-sm">حذف اکانت کاربری</strong>-->
<!--          </div>-->
<!--          <Icon name="icon:chevron-left" size="15" />-->
<!--        </NuxtLink>-->
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.setting-box-shadow {
  box-shadow: 0px 2px 6px 2px rgba(60, 64, 67, 0.15),
    0px 1px 2px 0px rgba(60, 64, 67, 0.3);
}
</style>
