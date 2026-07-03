<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});

const router = useRouter();

function goBack() {
  router.go(-1);
}

function redirectToIntroPage() {
  router.push("/dashboard/specialist/my-courses/add/intro");
}

function redirectToExercisePage() {
  router.push("/dashboard/specialist/my-courses/add/exercise");
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">ساخت برنامه جدید</p>
          </template>
        </BaseSpecialistHeader>
      </template>
      <div class="w-full flex flex-col gap-y-3">
        <DashboardSpecialistCreateCourseAddCourseDetailsBox
          class="custom-create-course-box-shadow"
          @click="redirectToIntroPage"
        >
          صفحه توضیحات اولیه
        </DashboardSpecialistCreateCourseAddCourseDetailsBox>

        <DashboardSpecialistCreateCourseAddCourseDetailsBox
          class="custom-create-course-box-shadow"
          @click="redirectToExercisePage"
        >
          افزودن تمرین
        </DashboardSpecialistCreateCourseAddCourseDetailsBox>

        <DashboardSpecialistCreateCourseAddCourseDetailsBox
          class="custom-create-course-box-shadow"
        >
          صفحه پایان
        </DashboardSpecialistCreateCourseAddCourseDetailsBox>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-create-course-box-shadow {
  box-shadow: 0px 1px 2px 0px rgba(0, 0, 0, 0.25),
    0px 2px 6px 0px rgba(0, 0, 0, 0.25);
}
</style>
