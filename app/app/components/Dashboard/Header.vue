<script setup lang="ts">
const authStore = useAuthStore()
</script>

<template>
  <header
      class="w-full flex items-center justify-between px-5 py-6 custom-pattern-bg-image sm:h-[6.75rem]"
  >
    <NuxtLink :to="useUtils().isSpecialist.value ? '/dashboard/specialist': '/dashboard/user'">
      <NuxtImg
          v-if="authStore?.getUser?.profileImage"
          :src="authStore.getUser.profileImage"
          class="w-16 h-16 rounded-full object-cover sm:hidden"
      />
      <NuxtImg
          v-else
          src="/common/no-image.png"
          class="w-16 h-16 rounded-full object-cover sm:hidden"
      />

    </NuxtLink>
    <div class="grid grid-cols-4">
<!--      <div-->
<!--        class="w-12 h-12 rounded-full pb-2 flex items-center justify-center relative"-->
<!--      >-->
<!--        <Icon name="icon:ring-wrench" size="70" />-->
<!--        <p-->
<!--          class="absolute top-1/2 left-1/3 -translate-y-1/3 translate-x-1/2 text-white"-->
<!--        >-->
<!--          1-->
<!--        </p>-->
<!--      </div>-->
      <div class="col-span-1">
        <nuxt-link to="/dashboard/calendar"
                   class="w-10 h-10 rounded-full flex items-center justify-center relative"
        >

          <LazyIconsCalendarIcon class="w-6 h-6 fill-white"></LazyIconsCalendarIcon>

        </nuxt-link>
      </div>
      <div class="col-span-1">
        <nuxt-link to="/dashboard/support"
            class="w-10 h-10 rounded-full flex items-center justify-center relative"
        >

          <LazyIconsSupportIcon class="w-6 h-6 fill-white"></LazyIconsSupportIcon>

        </nuxt-link>
      </div>
      <div class="col-span-1">
        <LazySharedPointsIdentifier></LazySharedPointsIdentifier>
      </div>
      <div class="col-span-1 flex items-center justify-center">
        
        <LazyDashboardNotificationIdentifier></LazyDashboardNotificationIdentifier>
      </div>

      <!--      <NuxtLink-->
      <!--        class="w-12 h-12 flex items-center justify-center bg-white rounded-full shadow-[0px_5px_7.5px_rgba(0,0,0,0.3),0px_5px_6px_rgba(50,50,93,0.25),inset_0px_-2px_3px_rgba(156,84,84,0.35)]"-->
      <!--        to="/dashboard/environment"-->
      <!--      >-->
      <!--        <NuxtImg src="/core/earth.png" class="w-7 h-auto object-contain" />-->
      <!--      </NuxtLink>-->
      <!--      <NuxtLink-->
      <!--        class="w-12 h-12 flex items-center justify-center bg-white rounded-full shadow-[0px_5px_7.5px_rgba(0,0,0,0.3),0px_5px_6px_rgba(50,50,93,0.25),inset_0px_-2px_3px_rgba(156,84,84,0.35)]"-->
      <!--        to="/dashboard/analyze"-->
      <!--      >-->
      <!--        <NuxtImg src="/core/heart.png" class="w-7 h-auto object-contain" />-->
      <!--      </NuxtLink>-->
      <!--      <NuxtLink-->
      <!--        class="w-12 h-12 flex items-center justify-center bg-white rounded-full shadow-[0px_5px_7.5px_rgba(0,0,0,0.3),0px_5px_6px_rgba(50,50,93,0.25),inset_0px_-2px_3px_rgba(156,84,84,0.35)]"-->
      <!--        to="/dashboard/user/points"-->
      <!--      >-->
      <!--        <NuxtImg src="/core/diamond.png" class="w-7 h-auto object-contain" />-->
      <!--      </NuxtLink>-->
    </div>
  </header>
</template>
