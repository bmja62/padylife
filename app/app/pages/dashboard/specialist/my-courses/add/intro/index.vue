<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});

const router = useRouter();
function goBack() {
  router.go(-1);
}

function quitCreateCourse() {
  router.push("/dashboard/specialist/");
}

function goToAddExercise() {
  router.push("/dashboard/specialist/my-courses/add/exercise");
}

const doesHaveTitle = ref<boolean>(true);
const doesHaveDescription = ref<boolean>(true);
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper
      style="--wrapper-header-height: 80px"
      inner-class="!px-10"
    >
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">صفحه توضیحات اولیه</p>
          </template>
        </BaseSpecialistHeader>
      </template>
      <div class="w-full tabs-list flex items-center justify-start gap-x-9">
        <button type="button" class="text-[#00ABFB] text-sm font-medium">
          ایجاد
        </button>
        <button type="button" class="text-sm font-medium">پیش نمایش</button>
      </div>

      <hr class="my-4 border-[#D2D2D2]" />

      <div class="w-full flex flex-col gap-y-5">
        <div class="w-full flex items-center justify-between">
          <label for="imageOrVideo" class="text-[#3D3D3D] text-sm"
            >عکس یا ویدیو</label
          >
          <input
            id="imageOrVideo"
            type="checkbox"
            class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
          />
        </div>

        <div class="w-full space-y-4">
          <div class="w-full flex items-center justify-between">
            <label for="title" class="text-[#3D3D3D] text-sm">عنوان</label>
            <input
              id="title"
              v-model="doesHaveTitle"
              type="checkbox"
              class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
            />
          </div>
          <textarea
            v-show="doesHaveTitle"
            class="textarea textarea-bordered rounded-[4px] w-full focus:outline-none"
            placeholder="عنوان"
          ></textarea>
        </div>

        <div class="w-full space-y-4">
          <div class="w-full flex items-center justify-between">
            <label for="description" class="text-[#3D3D3D] text-sm"
              >توضیحات</label
            >
            <input
              id="description"
              v-model="doesHaveDescription"
              type="checkbox"
              class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
            />
          </div>
          <textarea
            v-show="doesHaveDescription"
            class="textarea textarea-bordered rounded-[4px] w-full focus:outline-none"
            placeholder="توضیحات"
          ></textarea>
        </div>

        <DashboardInfoBox class="custom-info-box-shadow">
          <template #title> تعداد مراحل </template>
          <template #value> 6 </template>
        </DashboardInfoBox>

        <DashboardInfoBox class="custom-info-box-shadow">
          <template #title> زمان تقریبی </template>
          <template #value> 9 دقیقه </template>
        </DashboardInfoBox>

        <DashboardInfoBox class="custom-info-box-shadow">
          <template #title> دکمه ورود </template>
          <template #value> شروع </template>
        </DashboardInfoBox>
      </div>
      <div class="w-full grid grid-cols-2 gap-x-5 mt-auto mb-28">
        <button
          type="button"
          class="w-full btn bg-white shadow-none border border-[#565E6D] !text-[#565E6D] bottom-0"
          @click="quitCreateCourse"
        >
          انصراف
        </button>
        <button
          type="button"
          class="w-full btn bg-[#00ABFB] bottom-0"
          @click="goToAddExercise"
        >
          بعدی
        </button>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-info-box-shadow {
  box-shadow: 0px 2px 6px 2px rgba(60, 64, 67, 0.15),
    0px 1px 2px 0px rgba(60, 64, 67, 0.3);
}
</style>
