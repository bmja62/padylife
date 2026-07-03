<script setup lang="ts">
useHead({
  title: "صفحه اصلی",
});

const slides = ref([
  {
    image: '/common/intro.jpg',
    text: "به پادی لایف خوش آمدید! راهنمای هوشمند شما در مسیر توسعه فردی و رشد شخصی. با ما قدم به قدم به بهترین نسخه از خودت تبدیل شو.",
    isLastStep: false,
  },
  {
    image: '/common/intro2.jpg',
    text: "کشف توانایی‌های تو با پادی لایف! تمرین‌های کاربردی، چالش‌های انگیزشی و ابزارهای پیشرفته برای رشد مهارت‌های فردی. همین امروز شروع کن",
    isLastStep: false,
  },
  {
    image: '/common/intro3.jpg',
    text: "همراه همیشگی تو در مسیر پیشرفت! با پادی لایف برنامه‌ریزی کن، یاد بگیر و رشد کن. آینده بهتری بساز",
    isLastStep: true,
  },
]);

const containerRef = ref(null);
useSwiper(containerRef, {
  pagination: {
    clickable: true,
  },
});
const router = useRouter()
const authStore = useAuthStore()
onBeforeMount(() => {
  console.log(authStore.isFirstVisit)
  if (!authStore.isFirstVisit) {
    router.push('/sign-in')
  } else {
    authStore.setFirstVisit()
  }
})

</script>

<template>
  <div class="w-full h-full">
    <BaseLaunchOverlay></BaseLaunchOverlay>
    <ClientOnly>
      <div class="w-full flex items-center justify-center mt-[34px]">
        <Icon name="icon:logo-typography-horizontal" size="50" color="#01CED1" />
      </div>
      <swiper-container ref="containerRef" dir="ltr"
        class="h-[calc(100svh-150px)] custom-swiper w-full px-5 [--swiper-pagination-bullet-width:20px] [--swiper-pagination-bullet-border-radius:15px] [--swiper-pagination-color:#01CED1]">
        <swiper-slide v-for="(slide, idx) in slides" :key="idx" class="w-full">
          <div class="w-full h-[90%] flex flex-col items-center justify-between">
            <div>
              <div class="w-full flex items-center justify-center mt-16">
                <NuxtImg :src="slide.image" class="w-full h-[300px] rounded-xl" />
              </div>
              <p dir="rtl" class="mt-3 text-justify">
                {{ slide.text }}
              </p>
            </div>
            <div v-if="slide.isLastStep" class="w-full flex flex-col gap-y-6 items-center justify-center mt-auto">
              <NuxtLink to="/sign-up" class="w-full btn btn-primary">
                ساخت حساب کاربری
              </NuxtLink>
              <NuxtLink to="/sign-in" class="w-full text-neutral text-center">
                قبلا ثبت‌نام کرده‌اید؟ وارد شوید
              </NuxtLink>
            </div>
          </div>
        </swiper-slide>

      </swiper-container>
      <nuxt-link to="/sign-up" class="w-full flex items-center justify-center ">
        <span class="text-primary">رد کردن</span>
      </nuxt-link>
    </ClientOnly>
  </div>
</template>
<style>
swiper-container::part(bullet-active) {
  width: 50px;
}
</style>
