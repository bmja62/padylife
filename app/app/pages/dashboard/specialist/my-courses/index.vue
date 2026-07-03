<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});

interface IMyCourses {
  title: string;
  imageUrl: string;
}

const pendingCourses = ref<IMyCourses[]>([
  {
    title: "غذای سالم",
    imageUrl: "/junks/dummy-light.png",
  },
  {
    title: "میان وعده",
    imageUrl: "/junks/dummy-light.png",
  },
]);

const createdCourses = ref<IMyCourses[]>([
  {
    title: "غذای سالم",
    imageUrl: "/junks/dummy-white.png",
  },
  {
    title: "میان وعده",
    imageUrl: "/junks/dummy-white.png",
  },
  {
    title: "تغذیه",
    imageUrl: "/junks/dummy-white.png",
  },
]);

const router = useRouter();
function goBack() {
  router.go(-1);
}

function redirectToCreateNewCourse() {
  router.push("/dashboard/specialist/my-courses/add");
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">برنامه‌های من</p>
          </template>
        </BaseSpecialistHeader>
      </template>
      <div class="w-full space-y-6">
<!--        <UtilsStatsCard class="flex-col" @click="redirectToCreateNewCourse">-->
<!--          <template #title>-->
<!--            <p class="text-black text-sm">ساخت دوره جدید</p>-->
<!--          </template>-->
<!--          <template #icon>-->
<!--            <NuxtImg-->
<!--              src="/junks/page.png"-->
<!--              class="object-contain w-14 h-auto mb-2"-->
<!--            />-->
<!--          </template>-->
<!--        </UtilsStatsCard>-->

        <div class="w-full space-y-4">
          <h4 class="text-[#303030]">برنامه‌های در حال ساخت</h4>
          <div class="grid grid-cols-3 gap-x-4">
            <DashboardSpecialistCourseCard
              v-for="(course, index) in pendingCourses"
              :key="index"
              class="bg-white custom-course-card-shadow"
            >
              <NuxtImg
                class="object-contain w-12 h-12"
                :src="course.imageUrl"
              />
              <p class="text-black">
                {{ course.title }}
              </p>
            </DashboardSpecialistCourseCard>
          </div>
        </div>

        <div class="w-full space-y-4 mt-2">
          <h4 class="text-[#303030]">دوره‌های ساخته‌شده</h4>
          <div class="grid grid-cols-3 gap-x-4">
            <DashboardSpecialistCourseCard
              v-for="(course, index) in createdCourses"
              :key="index"
              class="bg-[#F2F2F2]"
            >
              <NuxtImg
                class="object-contain w-12 h-12"
                :src="course.imageUrl"
              />
              <p class="text-[#616161]">
                {{ course.title }}
              </p>
            </DashboardSpecialistCourseCard>
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-course-card-shadow {
  box-shadow: 0px 2px 6px 2px rgba(60, 64, 67, 0.15),
    0px 1px 2px 0px rgba(60, 64, 67, 0.3);
}
</style>
