<template>
  <div class="w-full px-0 mx-0 min-h-screen rtl text-right">
    <div class="w-11/12 py-5 mx-auto">
      <div class="w-full ">
        <div class="flex h-screen items-center flex-col justify-center">
          <LazyIconsCheckIcon class="fill-green-500 !w-40 !h-40"/>
          <strong style="font-size: 40px" class="my-4">پرداخت موفق</strong>
          <span style="font-size: 20px" class="text-gray-400">پرداخت شما با موفقیت انجام شد</span>
          <nuxt-link to="/dashboard"
                     class="btn bg-primary text-white rounded border-none my-4">بازگشت به خانه
          </nuxt-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
useHead({
  title: 'پرداخت موفق'
})
definePageMeta({
  layout: "dashboard",
  auth: true

});
</script>

<style scoped>

</style>
